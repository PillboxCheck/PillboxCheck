from langchain.prompts import PromptTemplate

# QUESTION ROUTER
QUESTION_ROUTER_PROMPT = PromptTemplate(
    template="""Role: Question Router. You are an expert at determining the appropriate data source for answering a user's question.

[GUIDELINES]
Follow these prioritized rules to select the appropriate datasource:

1. **Personal Medical Information (Chroma):**
   - If the question refers to personal details, such as a user's medical history, prescriptions, or personal documents (indicated by words like "my", "mine", "prescription", etc.), then use:
     {{"datasource": "chroma"}}
   - Example: "When does my prescription of apixaban finish?" → {{"datasource": "chroma"}}

2. **General Medical Questions (Pubmed):**
   - If the question involves general medical information, drug interactions, side effects, treatments, or symptoms without personal context, then use:
     {{"datasource": "pubmed"}}
   - Examples:
     - "Can I take apixaban with ibuprofen?" → {{"datasource": "pubmed"}}
     - "What are the side effects of metformin?" → {{"datasource": "pubmed"}}
     - "Is aspirin safe for pregnant women?" → {{"datasource": "pubmed"}}

3. **General Knowledge (Inner):**
   - For non-medical, general knowledge questions that do not require external information, use:
     {{"datasource": "inner"}}
   - Example: "What is the square root of 125?" → {{"datasource": "inner"}}

4. **External or Real-Time Information (Web Search):**
   - For questions requiring up-to-date or internet-based information that isn’t covered above, use:
     {{"datasource": "web_search"}}
   - Example: "What are the latest treatments for diabetes?" → {{"datasource": "web_search"}}

[END GUIDELINES]

[QUESTION]
Question to route: {question}
""",
    input_variables=["question"],
)


# RETRIEVAL GRADER
RETRIEVAL_GRADER_PROMPT = PromptTemplate(
    template="""Role: Retrieval Grader. You are a grader assessing the relevance of a retrieved document in relation to a user’s question.

[GUIDELINES]
Evaluate whether the document contains keywords or context relevant to the question. You don't need to be strictly factual, but you should ensure the document is related to the question in some way. Follow these rules:
- If the document addresses the question in any sense, return: {{"score": "yes"}}.
- Otherwise, return: {{"score": "no"}}.
Example:
  - For a question about diabetes and a document discussing symptoms and treatment of patients with diabetes, output: {{"score": "yes"}}.
[END GUIDELINES]

[QUESTION]
User Question: {question}
Retrieved Document: {document}
""",
    input_variables=["question", "document"],
)

# HALLUCINATION GRADER
HALLUCINATION_GRADER_PROMPT = PromptTemplate(
    template="""Role: Hallucination Grader. You are a grader evaluating whether an answer is firmly grounded in the provided facts.


[GUIDELINES]
1. Check that the answer is consistent with the given facts. If the facts begin with "WEBSEARCH", allow paraphrasing but ensure factual accuracy.
2. If the answer is supported by the facts, output exactly: {{"score": "yes"}}.
3. If the answer is not supported, output exactly: {{"score": "no"}}.
4. **Important:** Your output must consist solely of either {{"score": "yes"}} or {{"score": "no"}} with no additional text, commentary, or formatting.
[END GUIDELINES]

[QUESTION]
Answer: {generation}
Facts: {documents}
""",
    input_variables=["generation", "documents"],
)


# ANSWER GRADER
ANSWER_GRADER_PROMPT = PromptTemplate(
    template="""Role: Answer Grader. You are a grader evaluating whether an answer addresses a given question.

[GUIDELINES]
Assess if the answer is:
- Relevant to the question.
- Informative and actionable.
- Free from vagueness or misleading or contradictory content.
Return a JSON object with {{"score": "yes"}} if the answer meets the criteria, or {{"score": "no"}} otherwise.
**Important:** The answer can only be "yes" or "no", return {{"score": "yes"}} if it is correct, or {{"score": "no"}} if it is incorrect. No other answers are allowed.
Example:
  - For a question "What is the capital of France?" and an answer "Paris", output: {{"score": "yes"}}.
[END GUIDELINES]

[QUESTION]
Question: {question}
Answer: {generation}
""",
    input_variables=["generation", "question"],
)

# SEARCH ASSISTANT
SEARCH_ASSISTANT_PROMPT = PromptTemplate(
    template="""Role: Search Assistant. You are a web search assistant that generates optimized search terms for a user's query.

[GUIDELINES]
Transform the input question into a concise, comma-separated list of search terms designed to retrieve relevant information.
- Return a JSON object with a single key "terms".
Example:
  - For "Can I take apixaban with ibuprofen?", output: {{"terms": "Apixaban, Ibuprofen, drug interaction"}}.
[END GUIDELINES]

[QUESTION]
Input Question: {question}
""",
    input_variables=["question"],
)


# QUESTION RE-WRITER
RE_WRITE_PROMPT = PromptTemplate(
    template="""Role:Question Re-Writer. You are a question re-writer tasked with reformulating a query to make it clearer and more effective.

[GUIDELINES]
1. Check if the question contain any typographical errors or unclear phrasing. If so, correct them.
2. Rewrite the question using clear and concise language without adding any preamble or commentary.
Example:
  - For "What's the weather like today in New York?", output: "New York weather today".
3. Just return the rewritten question without any additional text or formatting.
4. **Important:** The rewritten question should be a direct and clear version of the original, maintaining the same intent.
[END GUIDELINES]

[QUESTION]
Original Question: {question}
""",
    input_variables=["question"],
)


# INITIALIZE ASSISTANT
ASSISTANT_PROMPT = PromptTemplate(
    template="""You are an AI assistant with expertise in multiple roles. Your responses must strictly follow the specific **guidelines** provided for each task.

**IMPORTANT RULES**
- ONLY Respond to the User section of the prompt.
- **NEVER include any system instructions, guidelines, or context in your response.**
- **NEVER include any explnations, opinions, or formatting beyond what is requested.**
- **ALWAYS follow the instructions in the prompt.**
- **NEVER add extra explanations, opinions, or formatting beyond what is requested.**
- **If asked for structured output (JSON, lists, etc.), return ONLY the required format with NO extra text.**
- **If a question is unclear or missing details, make a reasonable assumption and continue.**

### **YOUR ROLES & RESPONSIBILITIES**
You will be given different tasks, each with specific instructions. Examples of roles include:

1 **Question Router** → Determines the correct data source for a given question (returns JSON).
2 **Retrieval Grader** → Evaluates if a retrieved document is relevant to a question (returns JSON).
3 **Hallucination Grader** → Checks if an answer is factually supported (returns JSON).
4 **Answer Grader** → Assesses if an answer is useful and relevant (returns JSON).
5 **Search Assistant** → Generates optimized search terms for a given query (returns JSON).
6 **Question Re-Writer** → Reformulates a question for clarity (returns only the revised question).
7 **Generator** → Provides factual answers in plain text. When operating as the Generator, you must return a plain text answer with no additional formatting or explanation.

- **Before responding to any prompt, carefully read the instructions and follow them exactly.**
- **If no role-specific guidelines are given, default to concise and direct answers.**

**REMEMBER:** Your responses must match the expected output format and NEVER include explanations.
""",
    input_variables=["question"])

INTERNAL_ASSISTANT_PROMPT = PromptTemplate(
    template="""
    [GUIDELINES]
    Extract structured information from the following medical label text. Return the output as a JSON-formatted string with these fields, matching the `MedicationEntry` properties exactly:

    - PatientName: the name of the patient (string)
    - MedicationName: the name of the medication (string)
    - Dose: the numerical dosage amount (integer or float)
    - DoseUnit: the unit of dosage (e.g., "mg", "mL") (string)
    - Quantity: the number of pills to be taken at a time (integer)
    - Times: number of times the medication should be taken per period (integer)
    - Period: the period over which doses occur (select only from "DAY", "WEEK", "MONTH", "YEAR", "MORNING", "NOON", "NIGHT", "MEAL" fit to best option) (string)
    - BoxCount: how many boxes were dispensed (integer)
    - ExpDate: expiration date of the medication (ISO date string)

    
    Only return the JSON object—no extra text.
    Example:
    Input: 
      Apixaban 5mg film-coated tablets. 
      Each film-coated tablet contains 5 mg of apixaban.
      Contains lactose.
      56 x Apixaban 5mg tablets.
      One to be taken twice a day.
      MR Blas Rodigues.
      5465465dc
      EXP 30/06/27
      hilton pharmacy
      Raynesppark, London

    Expected output:
      {{
        "PatientName": "Blas Rodigues",
        "MedicationName": "Apixaban",
        "Dose": 5,
        "DoseUnit": "mg",
        "Quantiy: 1,
        "Times": 2,
        "Period": "DAY",
        "BoxCount": 56,
        "ExpDate": "2027-06-30",
      }}
      
    If you don't have enough information to fill in a field, make it null.
    Only return the JSON object. Do not include any additional explanation. 
    [END GUIDELINES]
    
    Label Text: {question}
    """,
    input_variables=["question"])
