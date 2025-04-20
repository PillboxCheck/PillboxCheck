from imgtopdf import image_to_pdf
from langchain_community.document_loaders import PyPDFLoader, TextLoader, JSONLoader
import os



def load_file(vectorstore, file:str):

    # Initialize the Chroma instance
    #vectorstore = Chroma(collection_name=db, persist_directory=chroma_path)
    temp =False
    # New data to insert (replace with your own)
    if file.endswith(("txt","md")): 
            loader = TextLoader(file)
    elif file.endswith("json"):
           loader = JSONLoader(file)
    elif file.endswith("pdf"):
            loader = PyPDFLoader(file)
    else:
            return
    
    doc = loader.load()
    vectorstore.add_documents(doc)
   

def load_dir(vectorstore, root_dir):
    """
    Recursively load every file under root_dir into the vectorstore
    by calling load_file(vectorstore, filepath).
    """
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for fname in filenames:
            full_path = os.path.join(dirpath, fname)
            load_file(vectorstore, full_path)
