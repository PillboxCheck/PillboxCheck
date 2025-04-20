import cv2
import numpy as np
import pytesseract
from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import A4
from PIL import Image

pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"  


def preprocess_document(image_path):
    # Read image
    img = cv2.imread(image_path, cv2.IMREAD_COLOR)
    
    custom_config = r'--psm 0' 
    orientation = pytesseract.image_to_osd(img, config=custom_config)
    print(orientation)
    
    # Check rotation angle from OSD result
    angle = int(orientation.split("\n")[2].split(":")[1].strip()) 
    
    # Rotate image if needed
    if angle != 0:
        # Rotate by the detected angle
        #print(angle)
        img = rotate_image(img, -angle)
    
    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    
    # Apply threshold to get a scanned effect
    _, thresh = cv2.threshold(gray, 150, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)
    
    # Save the processed image temporarily (just for text extraction)
    processed_path = "processed_document.png"
    cv2.imwrite(processed_path, thresh)
    
    return processed_path

def rotate_image(img, angle):
    # Rotate the image by the specified angle and avoid cutting it
    (h, w) = img.shape[:2]
    center = (w // 2, h // 2)
    
    # Get the rotation matrix
    rotation_matrix = cv2.getRotationMatrix2D(center, angle, 1.0)
    
    # Calculate the bounding box to avoid cutting off the image
    abs_cos = abs(rotation_matrix[0, 0])
    abs_sin = abs(rotation_matrix[0, 1])
    
    new_w = int(h * abs_sin + w * abs_cos)
    new_h = int(h * abs_cos + w * abs_sin)
    
    # Adjust the rotation matrix to account for the new image dimensions
    rotation_matrix[0, 2] += (new_w / 2) - center[0]
    rotation_matrix[1, 2] += (new_h / 2) - center[1]
    
    # Perform the rotation with the new bounding box
    rotated_img = cv2.warpAffine(img, rotation_matrix, (new_w, new_h))
    
    return rotated_img


def extract_text(image_path):
    # Use Tesseract to extract text from the image
    text = pytesseract.image_to_string(Image.open(image_path))
    return text

def image_to_pdf(image_path, pdf_path):
    processed_image = preprocess_document(image_path) 
    extracted_text = extract_text(processed_image) 
    
    # Create PDF
    c = canvas.Canvas(pdf_path, pagesize=A4)
    
    # Add extracted text as selectable text (without background image)
    c.setFont("Helvetica", 12)
    text_lines = extracted_text.split("\n")
    y_position = height - 20
    for line in text_lines:
        c.drawString(10, y_position, line)
        y_position -= 15
    
    c.save()
    print(f"PDF saved at: {pdf_path}")

if __name__ == '__main__':
    image_path =r"C:\Users\luis_\Downloads\rotate.jpg" 
    pdf_path = "document_text_only.pdf"
    image_to_pdf(image_path, pdf_path)
