
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.document_loaders import TextLoader
from langchain_chroma import Chroma
from utils import *
from socketprompts import *
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

from chatSocket import ChatSocket
from langchain_core.callbacks import CallbackManager, StreamingStdOutCallbackHandler
import socket
import json
import os
import argparse


args_parser = argparse.ArgumentParser(description="RAG Socket")
args_parser.add_argument("--port", type=int, default=12345, help="Port to listen on")
args_parser.add_argument(
        "--pubmed",
        type=lambda x: x.lower() in ['true', '1', 'yes'],
        default=False,
        help="Boolean flag (True/False) to use PubMed retriever",
    )
args = args_parser.parse_args()
# Make connections 
llm = ChatSocket(port=12345)

parser_runnable = RunnableLambda(parse_huggingface_to_json)
print("-----Load embbeding-----")
embeddings_model = HuggingFaceEmbeddings(model_name="ibm-granite/granite-embedding-30m-english", 
                                        cache_folder=".\\cached_embedding")


# Add to vectorDB
try:
    vectorstore = Chroma(
        collection_name="rag-chroma",
        embedding_function=embeddings_model,
        persist_directory=".\\mychromadb",
    )
    print("Chroma created")
    #vectorstore.persist()
    print("Chroma persisted")
    load_dir(vectorstore, ".\\patient_info")
    print("Chroma added documents")
    # vectorstore = Chroma.from_documents(
    #     documents=doc_splits,
    #     collection_name="rag-chroma",
    #     embedding=embeddings_model,
    #     persist_directory=".\\mychromadb"
    # )
    # vectorstore.persist()
    print("-----VectorDB-----")
except Exception as exception:
    print(exception)
    assert False
chroma_retriever = vectorstore.as_retriever()
print("-----Loaded vectorDB-----")
callback_manager = CallbackManager([StreamingStdOutCallbackHandler()])
print("-----Callbackmanager -----")
pubmed_retriver_online = PubMedRetriever(top_k_results=3)
path_local = ".\\LocalRepository"
pubmed_chroma = Chroma( collection_name="pubmed-chroma",
        embedding_function=embeddings_model,
        persist_directory=".\\pubmedChromadb",)
#pubmed_chroma.persist()
load_dir(pubmed_chroma, path_local)
if args.pubmed:
    pubmed_retriver = pubmed_retriver_online
else:
    pubmed_retriver = pubmed_chroma.as_retriever()
print(type(pubmed_retriver))
print("-----Pubmed retriever -----")
# Router
print("--------------------Router-------------------------")
question_router = QUESTION_ROUTER_PROMPT | llm | parser_runnable #

# Retrieval Grader
print("----------------------Retrival Grader------------------")
retrieval_grader = RETRIEVAL_GRADER_PROMPT | llm | parser_runnable 

# Chain
print("----------------------rag chain------------------")
prompt = hub.pull("rlm/rag-prompt")
print(prompt)
print("-------------------------------------------------")
print(prompt.messages[0].prompt.template)
prompt.messages[0].prompt.template = "Role: Generator. " + prompt.messages[0].prompt.template
print("-------------------------------------------------")
print(prompt)
rag_chain = prompt | llm | StrOutputParser()

# Hallucination Grader
print("--------------------------Hallucination------------------------")
hallucination_grader = HALLUCINATION_GRADER_PROMPT | llm | parser_runnable 

### Answer Grader
print("----------------------Answer Grader------------------")
answer_grader = ANSWER_GRADER_PROMPT | llm | parser_runnable 

### Search Assitant
print("----------------------Search Assitant------------------")
search_assistant = SEARCH_ASSISTANT_PROMPT | llm | parser_runnable

### Question Re-writer
print("----------------------Question ReWriter------------------")
question_rewriter = RE_WRITE_PROMPT | llm | StrOutputParser()

print("----------------------DuckDuckGo------------------")
# Search Tool
web_search_tool = DuckDuckGoSearchRun()
print("-----------------------Internal Label--------------------------")
internal_assistant_label = INTERNAL_MEDSCAN_PROMPT | llm | StrOutputParser()
print("-----------------------Internal Label--------------------------")
internal_assistant_event = INTERNAL_EVENTSCAN_PROMPT | llm | StrOutputParser()

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
    
    if documents:
        user_info = "[USER INFORMATION]\n"+ pdf_to_string(".\\SummaryReport.pdf") +"\n[END USER INFORMATION]\n"
        if not user_info in documents:
            documents = [user_info]+documents
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
    if "documents" in state.keys():
        documents = state["documents"]
    else:
        documents = ""

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
        print(web_results)
    except:
        web_results = "###FAILED###"
        print(web_results)
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
    source = question_router.invoke({"question": question, "documents":""})
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


def internal_label_process(state):
    question = state["question"]
    generation = internal_assistant_label.invoke({"question": question})
    return {"question": question, "generation": generation}

def internal_event_process(state):
    question = state["question"]
    generation = internal_assistant_event.invoke({"question": question})
    return {"question": question, "generation": generation}


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
# build the graph
workflow.add_edge(START, "transform_query")

workflow.add_conditional_edges(
    "transform_query",
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

# define the second workflow for Generation without RAG
workflow2 = StateGraph(GraphState)
workflow2.add_node("uncod_generate",uncod_generate)
workflow2.add_node("output",output)
workflow2.add_edge(START,"uncod_generate")
workflow2.add_edge("uncod_generate","output")
workflow2.add_edge("output",END)
app2 = workflow2.compile()

# define internal assistant workflow
workflow3 = StateGraph(GraphState)
workflow3.add_node("internal_label_process",internal_label_process)
workflow3.add_node("output",output)
workflow3.add_edge(START,"internal_label_process")
workflow3.add_edge("internal_label_process","output")
workflow3.add_edge("output",END)
app_internal = workflow3.compile()

# define internal assistant for Event
workflow4 = StateGraph(GraphState)
workflow4.add_node("internal_event_process",internal_event_process)
workflow4.add_node("output",output)
workflow4.add_edge(START,"internal_event_process")
workflow4.add_edge("internal_event_process","output")
workflow4.add_edge("output",END)
app_event = workflow4.compile()


# Run
#HOSTING

HOST = "127.0.0.1"  # Listen on localhost
PORT = 52712        # Choose any free port
attempts = 5
# Create client socket
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

while attempts > 0:
    # Connect to the server
    try:
        client_socket.connect((HOST, PORT))
        print(f"Connected to server at {HOST}:{PORT}")
        break
    except Exception as e:
        attempts -= 1
        print(f"Connection attempt failed {e}. {attempts} attempts left.")


while True:
    try:
        data = client_socket.recv(1024).decode('utf-8')
      
        if not data:
            print("Connection closed by the client.")
            break
        try:
            json_data = json.loads(data)
            pprint(f"Received JSON data: {json_data}")
        except json.JSONDecodeError as jde:
            print(f"Failed to parse JSON: {jde}")
            client_socket.sendall(b"Please try again")
            continue  # Optionally skip processing this iteration

        
        question = json_data.get("question", data)  # Fallback to raw data if no question is provided rewriter will analyze the question
        inputs = {"question": question}
        
        if not json_data.get("Internal",False):
            if json_data.get("RAGBool",True):
                for output in app.stream(inputs):
                    for key, value in output.items():
                        pprint(f"Node '{key}':")

                response = value.get("generation", "No generation found")
                print(f"Response: {response}")
            else:
                print("No RAG")
                for output in app2.stream(inputs):
                    for key, value in output.items():
                        pprint(f"Node '{key}':")
                response = value.get("generation", "No generation found")
                print(f"Response: {response}")
        else:
            print("internal")
            if not json_data.get("Event", False):
                for output in app_internal.stream(inputs):
                    for key, value in output.items():
                        pprint(f"Node '{key}':")
                response = value.get("generation", "No generation found")
                print(f"Response: {response}")
            else:
                pathfile = inputs["question"]
                print(pathfile)
                inputs["question"] = pdf_to_string(pathfile)
                for output in app_event.stream(inputs):
                    for key, value in output.items():
                        pprint(f"Node '{key}':")
                response = value.get("generation", "No generation found")
                print(f"Response: {response}")                

        client_socket.sendall(response.encode('utf-8'))
        #print("sent")
    except Exception as e:
        print(json.dumps({"error": str(e)}), flush=True)
        break


# Close the client socket
client_socket.close()
print("Client socket closed.")


