
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.document_loaders import WebBaseLoader
from langchain_community.document_loaders import TextLoader
from langchain_community.vectorstores import Chroma

from prompts import *
from langchain_community.retrievers import PubMedRetriever
from langchain import hub
from langchain_core.output_parsers import StrOutputParser
from langchain_community.tools import DuckDuckGoSearchRun

from typing import List

from typing_extensions import TypedDict

from langgraph.graph import END, StateGraph, START

from pprint import pprint

from langchain_huggingface import HuggingFaceEmbeddings
from parser_adapter import parse_huggingface_to_json
from langchain.schema.runnable import RunnableLambda


from langchain_core.callbacks import CallbackManager, StreamingStdOutCallbackHandler
from langchain_community.chat_models import ChatLlamaCpp
import multiprocessing

from langchain_huggingface import HuggingFacePipeline, ChatHuggingFace

import os

cpp = True
if cpp:
    local_model=os.getcwd()+"\\model\\granite-3.2-8b-instruct-Q3_K_L.gguf"
    #print(local_model)
    llm = ChatLlamaCpp(
    temperature=0.5,
    model_path=local_model,
    n_ctx=100000,
    n_gpu_layers=8,
    n_batch=300,  # Should be between 1 and n_ctx, consider the amount of VRAM in your GPU.
    max_tokens=512,
    n_threads=multiprocessing.cpu_count() - 1,
    repeat_penalty=1.5,
    top_p=0.5,
    verbose=False,
    )

else: 
    model = HuggingFacePipeline.from_model_id(
        model_id="ibm-granite/granite-3.2-2b-instruct",
        task="text-generation",
        pipeline_kwargs=dict(
            max_new_tokens=1024,
            do_sample=False,
            repetition_penalty=1.03,
        ),
    )
    llm = ChatHuggingFace(llm=model)

parser_runnable = RunnableLambda(parse_huggingface_to_json)
print("-----Load embbeding-----")
embeddings_model = HuggingFaceEmbeddings(model_name="ibm-granite/granite-embedding-30m-english")

# urls = []
# docs = [WebBaseLoader(url).load() for url in urls]
# docs_list = [item for sublist in docs for item in sublist]

path = ".\\patient_info"
docs= []
for file in os.listdir(path):
    if file.endswith("txt"):
        #print(file)
        docs += [doc for doc in TextLoader(".\\patient_info\\"+file).load()]
text_splitter = RecursiveCharacterTextSplitter.from_tiktoken_encoder(
    chunk_size=250, chunk_overlap=0
)
doc_splits = text_splitter.split_documents(docs)

# Add to vectorDB
vectorstore = Chroma.from_documents(
    documents=doc_splits,
    collection_name="rag-chroma",
    embedding=embeddings_model,
    persist_directory="./Chromadb"
)
chroma_retriever = vectorstore.as_retriever()
callback_manager = CallbackManager([StreamingStdOutCallbackHandler()])

pubmed_retriver = PubMedRetriever(top_k_results=5)

# Router
print("--------------------Router-------------------------")
question_router = QUESTION_ROUTER_PROMPT | llm | parser_runnable #

# Retrieval Grader
print("----------------------Retrival Grader------------------")
retrieval_grader = RETRIEVAL_GRADER_PROMPT | llm | parser_runnable 

# Chain
print("----------------------rag chain------------------")
prompt = hub.pull("rlm/rag-prompt")
rag_chain = prompt | llm | StrOutputParser()

# Hallucination Grader
print("--------------------------Hallucination------------------------")
hallucination_grader = HALLUCINATION_GRADER_PROMPT | llm | parser_runnable 

### Answer Grader
print("----------------------Answer Grader------------------")
answer_grader = ANSWER_GRADER_PROMPT | llm | parser_runnable 

### Search Assitant
print("----------------------Search Assitant------------------")
search_assistant = SEARCH_ASSITANT_PROMPT | llm | parser_runnable

### Question Re-writer
print("----------------------Question ReWriter------------------")
question_rewriter = RE_WRITE_PROMPT | llm | StrOutputParser()

# Search Tool
web_search_tool = DuckDuckGoSearchRun()

# State definition
class GraphState(TypedDict):
    """
    Represents the state of our graph.

    Attributes:
        question: question
        generation: LLM generation
        documents: list of documents
    """

    question: str
    generation: str
    documents: List[str]
    db: str

# Nodes
def retrieve(state):
    """
    Retrieve documents

    Args:
        state (dict): The current graph state

    Returns:
        state (dict): New key added to state, documents, that contains retrieved documents
    """
    print("---RETRIEVE---")
    question = state["question"]
    if state["db"] == "pubmed":
    # Retrieval
        documents = pubmed_retriver.invoke(question)
        #print(documents)
    else:
        documents = chroma_retriever.invoke(question)
        

    return {"documents": documents, "question": question, "db":state["db"]}


def generate(state):
    """
    Generate answer

    Args:
        state (dict): The current graph state

    Returns:
        state (dict): New key added to state, generation, that contains LLM generation
    """
    print("---GENERATE---")
    question = state["question"]
    documents = state["documents"]
    if isinstance(documents,str) and documents == "###FAILED###":
        documents = ""
    # RAG generation
    generation = rag_chain.invoke({"context": documents, "question": question})
    return {"documents": documents, "question": question, "generation": generation}

def uncod_generate(state):
    """
    Generate answer

    Args:
        state (dict): The current graph state

    Returns:
        state (dict): New key added to state, generation, that contains LLM generation
    """
    print("---GENERATE---")
    question = state["question"]
    # RAG generation
    generation = rag_chain.invoke({"context": "", "question": question})
    return { "question": question, "generation": generation} 


def grade_documents(state):
    """
    Determines whether the retrieved documents are relevant to the question.

    Args:
        state (dict): The current graph state

    Returns:
        state (dict): Updates documents key with only filtered relevant documents
    """

    print("---CHECK DOCUMENT RELEVANCE TO QUESTION---")
    question = state["question"]
    documents = state["documents"]

    # Score each doc
    filtered_docs = []
    for d in documents:
        score = retrieval_grader.invoke(
            {"question": question, "document": d.page_content}
        )
        grade = score["score"]
        if grade == "yes":
            print("---GRADE: DOCUMENT RELEVANT---")
            filtered_docs.append(d)
        else:
            print("---GRADE: DOCUMENT NOT RELEVANT---")
            continue
    return {"documents": filtered_docs, "question": question,"db":state["db"]}


def transform_query(state):
    """
    Transform the query to produce a better question.

    Args:
        state (dict): The current graph state

    Returns:
        state (dict): Updates question key with a re-phrased question
    """

    print("---TRANSFORM QUERY---")
    question = state["question"]
    documents = state["documents"]

    # Re-write question
    better_question = question_rewriter.invoke({"question": question})
    return {"documents": documents, "question": better_question}


def web_search(state):
    """
    Web search based on the re-phrased question.

    Args:
        state (dict): The current graph state

    Returns:
        state (dict): Updates documents key with appended web results
    """

    print("---WEB SEARCH---")
    question = state["question"]
    required_search = search_assistant.invoke({"question":question})
    #print(required_search)
    terms = required_search['terms'].split(",")
    #print(terms)
    # Web search
    sout ="WEBSEARCH"
    try:
        docs = [web_search_tool.invoke({"query": t}) for t in terms]
        s = "\n".join(docs)
        web_results = sout + " \n " + s
    except:
        web_results = "###FAILED###"
    return {"documents": web_results, "question": question}


# Edges
def route_question(state):
    """
    Route question to web search or RAG.

    Args:
        state (dict): The current graph state

    Returns:
        str: Next node to call
    """
    print("---ROUTE QUESTION---")
    question = state["question"]
    source = question_router.invoke({"question": question, "documents":docs})
    if source["datasource"] == "web_search":
        print("---ROUTE QUESTION TO WEB SEARCH---")
        return "web_search"
    elif source["datasource"] == "chroma":
        print("---ROUTE QUESTION TO RAG---")
        return "vectorstore"
    elif source["datasource"] == "pubmed":
        print("---ROUTE QUESTION TO RAG -> Pubmed---")
        return "pubmed"
    else:
        print("---ROUTE QUESTION TO INNER---")
        return "inner"


def decide_to_generate(state):
    """
    Determines whether to generate an answer, or re-generate a question.

    Args:
        state (dict): The current graph state

    Returns:
        str: Binary decision for next node to call
    """

    print("---ASSESS GRADED DOCUMENTS---")
    state["question"]
    filtered_documents = state["documents"]

    if not filtered_documents:
        # All documents have been filtered check_relevance
        # We will re-generate a new query
        if state["db"] == "chroma":
            print(
                "---DECISION: ALL DOCUMENTS ARE NOT RELEVANT TO QUESTION, TRANSFORM QUERY---"
            )
            return "no_info"
        else:
            print(
                "---DECISION: ALL DOCUMENTS ARE NOT RELEVANT TO QUESTION, TRANSFORM QUERY---"
            )
            return "transform_query"
    else:
        # We have relevant documents, so generate answer
        print("---DECISION: GENERATE---")
        return "generate"


def grade_generation_v_documents_and_question(state):
    """
    Determines whether the generation is grounded in the document and answers question.

    Args:
        state (dict): The current graph state

    Returns:
        str: Decision for next node to call
    """

    print("---CHECK HALLUCINATIONS---")
    question = state["question"]
    documents = state["documents"]
    generation = state["generation"]
    if isinstance(documents,str) and documents == "###FAILED###":
        grade = "yes"
    else:
        score = hallucination_grader.invoke(
        {"documents": documents, "generation": generation}
        )
        grade = score["score"]
    # Check hallucination
    if grade == "yes":
        print("---DECISION: GENERATION IS GROUNDED IN DOCUMENTS---")
        # Check question-answering
        print("---GRADE GENERATION vs QUESTION---")
        score = answer_grader.invoke({"question": question, "generation": generation})
        grade = score["score"]
        if grade == "yes":
            print("---DECISION: GENERATION ADDRESSES QUESTION---")
            return "useful"
        else:
            print("---DECISION: GENERATION DOES NOT ADDRESS QUESTION---")
            return "not useful"
    else:
        pprint("---DECISION: GENERATION IS NOT GROUNDED IN DOCUMENTS, RE-TRY---")
        return "not supported"

def pub_state_dict(state):
    question = state["question"]
    return {"question":question, "db":"pubmed"}

def chroma_state_dict(state):
    question = state["question"]
    return {"question":question, "db":"chroma"}

def output(state):
    question = state["question"]
    #documents = state["documents"]
    generation = state["generation"]
    return {"question": question, "generation": generation}

def no_info(state):
    question = state["question"]
    message = "I am sorry but I could not find any information relevant to this request on your data"
    return {"question": question, "generation": message}

workflow = StateGraph(GraphState)

# Define the nodes
workflow.add_node("web_search", web_search)  # web search
workflow.add_node("retrieve", retrieve)  # retrieve
workflow.add_node("grade_documents", grade_documents)  # grade documents
workflow.add_node("generate", generate)  # generatae
workflow.add_node("transform_query", transform_query)  # transform_query
workflow.add_node("uncod_generate", uncod_generate)
workflow.add_node("pub_state_dict", pub_state_dict)
workflow.add_node("chroma_state_dict", chroma_state_dict)
workflow.add_node("output",output)
workflow.add_node("no_info",no_info)
# Build graph
workflow.add_conditional_edges(
    START,
    route_question,
    {
        "web_search": "web_search",
        "vectorstore": "chroma_state_dict",
        "pubmed": "pub_state_dict",
        "inner":"uncod_generate"
    },
)
workflow.add_edge("web_search", "generate")
workflow.add_edge("retrieve", "grade_documents")
workflow.add_edge("chroma_state_dict", "retrieve")
workflow.add_edge("pub_state_dict", "retrieve")
workflow.add_conditional_edges(
    "grade_documents",
    decide_to_generate,
    {   
        "transform_query": "transform_query",
        "generate": "generate",
        "no_info":"no_info",
    },
)
workflow.add_conditional_edges("transform_query", route_question,
    {
        "web_search": "web_search",
        "vectorstore": "chroma_state_dict",
        "pubmed": "pub_state_dict",
        "inner":"uncod_generate"
    },
)
workflow.add_conditional_edges(
    "generate",
    grade_generation_v_documents_and_question,
    {
        "not supported": "generate",
        "useful": "output",
        "not useful": "transform_query",
    },
)
workflow.add_edge("no_info","output")
workflow.add_edge("uncod_generate","output")
workflow.add_edge("output",END)

# Compile
app = workflow.compile()


# Run
#inputs = {"question": "What is the AlphaCodium paper about?"} # web_search
inputs = {"question": "Can i take ibuprofene with Apixaban?"} # Pubmed
#inputs = {"question": "When do I need to stop my apixaban?"} # chroma
#inputs = {"question": "What colour is the sky?"} # Inner question
q = input("please enter your question or EXIT() to close: ")
while q != "EXIT()":
    inputs = {"question": q}
    for output in app.stream(inputs):
        for key, value in output.items():
            # Node
            pprint(f"Node '{key}':")
            # Optional: print full state at each node
            # pprint.pprint(value["keys"], indent=2, width=80, depth=None)
        pprint("\n---\n")

    # Final generation
    pprint(value["generation"])
    #print(output)
    q = input("please enter your question: ")
exit()