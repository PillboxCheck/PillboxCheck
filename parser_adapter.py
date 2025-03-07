import re
import json
from langchain.schema import AIMessage

def parse_huggingface_to_json(ai_message: AIMessage) -> dict:
    """
    Extracts and parses JSON from an AIMessage returned by a Hugging Face model.
    """
    # Ensure we extract text from AIMessage
    hf_output = ai_message.content if isinstance(ai_message, AIMessage) else str(ai_message)
    print(hf_output)
    # Extract JSON response from the text output
    match = re.search(r"\{\s*(\"score\"|\"datasource\"|\"terms\")\s*:\s*\"([^\"]+)\"\s*\}", hf_output)
    if match:
        json_data = match.group(0)
        try:
            #return f"\`\`\`\n{str(json.loads(json_data))}\n\`\`\`"
            return json.loads(json_data)  # Return as a dict for JsonOutputParser
        except json.JSONDecodeError:
            print(json_data)
            return {"error": "Invalid JSON extracted"}

    return {"error": "No JSON found in Hugging Face output"}
