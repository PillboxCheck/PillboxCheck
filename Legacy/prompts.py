from langchain.prompts import PromptTemplate


# QUESTION ROUTER
QUESTION_ROUTER_PROMPT = PromptTemplate( 
    template="""You are an expert at determining the appropriate data source for answering a user’s question. \n
    
    Choose from the following options based on the type of question:\n
    
    - Use `"inner"` for general knowledge questions that do not require external information.  
      **Example:** "What is the square root of 125?" → `{{"datasource": "inner"}}`  

    - Use `"pubmed"` for any question related to **medicine, drug interactions, side effects, treatments, symptoms, or medical recommendations**.  
      **Examples:**  
        - "Can I take apixaban with ibuprofen?" → `{{"datasource": "pubmed"}}`  
        - "What are the side effects of metformin?" → `{{"datasource": "pubmed"}}`  
        - "Is aspirin safe for pregnant women?" → `{{"datasource": "pubmed"}}`  

    - Use `"chroma"` for questions related to the user's personal medical history, prescriptions, letters, or any other user-specific details.  
      **Example:** "When do I stop taking my medication A?" → `{{"datasource": "chroma"}}`  

    - Otherwise, use `"web_search"` for questions that require external, real-time, or broader internet-based information.  
      **Example:** "What are the latest treatments for diabetes?" → `{{"datasource": "web_search"}}`  

    Return a JSON object with a single key `"datasource"` and no explanation or preamble. \n  
    **Question to route:** {question}  
    
    **Example Output:**  
    `{{"datasource": "chroma"}}`
    """,
    input_variables=["question"],
)


# Retrieval Grader

RETRIEVAL_GRADER_PROMPT = PromptTemplate(
    template="""You are a grader assessing relevance of a retrieved document to a user question. \n 
    Here is the retrieved document: \n\n {document} \n\n
    Here is the user question: {question} \n
    If the document contains keywords related to the user question, grade it as relevant. \n
    It does not need to be a stringent test. The goal is to filter out erroneous retrievals. \n
    Give a binary score 'yes' or 'no' score to indicate whether the document is relevant to the question. \n
    Provide the binary score as a JSON with a single key 'score' and no premable or explanation.
    Make sure the answer follows the format JSON and with only 'yes' or 'no' and the key 'score' must be used.""",
    input_variables=["question", "document"],
)

# Hallucination Grader

HALLUCINATION_GRADER_PROMPT = PromptTemplate(
    template="""You are a grader assessing whether an answer is grounded in / supported by a set of facts.

    Facts:
    -------  
    {documents}  
    -------  

    Answer: {generation}  

    Special Rule: If the facts start with "WEBSEARCH," consider the entire string and allow for rewording, paraphrasing, or inferred meaning. Be more lenient in assessing strict phrase matching but ensure the answer remains factually consistent.  

    **IMPORTANT** Return a JSON object with a single key "score" and a value of either "yes" or "no" (**NO** additional text or explanations).  

    Example Output:
    {{"score": "yes"}}  
    """,
    input_variables=["generation", "documents"],
)


# Answer Grader
ANSWER_GRADER_PROMPT = PromptTemplate(
    template="""You are a grader evaluating whether an answer is useful in addressing a given question.

    Question:  
    -------  
    {question}  
    -------  

    Answer:  
    -------  
    {generation}  
    -------  

    Criteria for usefulness:  
    - The answer is relevant to the question.  
    - The answer provides helpful, informative, or actionable content.  
    - The answer is not misleading, vague, or off-topic.  

    Return a JSON object with a single key "score" and a value of either "yes" or "no" (no additional text or explanations).  

    Example Output:  
    {{"score": "yes"}}  
    """,
    input_variables=["generation", "question"],
)

# Search Assitant
SEARCH_ASSITANT_PROMPT = PromptTemplate(
    template="""You are a web search assitant that takes an input question and gives a list of search terms required to answer this question. \n 
     be short and precise. IMPORTANT You must answer ONLY Json format with a single key 'terms' and value a list of terms separated by a ','. \n
     Example: Can i take apixaban with ibuprofene? then you reply in JSON with key 'terms' and value 'Apixaban, Ibuprofene, relationship between apixaban and ibuprofene'.
     Here is the initial question: \n\n {question}. \n
     Example Output:  
    {{"terms": "Apixaban, Ibuprofene, relationship between apixaban and ibuprofene"}}  
    """,
    input_variables=["generation", "question"],
)

# Question Re-writer
RE_WRITE_PROMPT = PromptTemplate(
    template="""You are a question re-writer that converts an input question to a better version that is optimized \n 
     Look at the initial and formulate an improved question. \n
     Here is the initial question: \n\n {question}. Improved question with no preamble: \n """,
    input_variables=["generation", "question"],
)


