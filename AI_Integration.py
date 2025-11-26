from openai import OpenAI
from dotenv import load_dotenv
import os
import time

client = OpenAI(
    base_url= os.getenv("AZURE_OPENAI_ENDPOINT"),
    api_key=os.getenv("AZURE_OPENAI_API_KEY")
)

input_text="Platzhalter"

conversation = [
    {"role": "system", "content": "Hilf dem Nutzer einen Lernplan zu erstellen und"
                                                    "seine Fragen zu Themen zu beantworten"}
]

def ask_gpt(user_input):
    # Add user input to the conversation
    conversation.append({"role": "user", "content": user_input})
    try:
        response = client.responses.create(
            model="gpt-5-chat",
            instructions="Help the User with his studying",
            input=conversation
        )
    except Exception as e:
        print("API error:", e)
        return None

    reply = response.output_text
    conversation.append({"role": "assistant", "content": reply})
    print(response.output_text)
    return 1

def upload_pdf(pdf_path):
    file = client.files.create(
        file=open(pdf_path, "rb"), #reading + binary
        purpose="assistants"
    )
    conversation.append({"role": "user", "content": file})
    file_id = file.id
    conversation.append({
        "role": "user",
        "content": [
            {"type": "input_text", "text": "Hier ist meine PDF."},
            {"type": "input_file", "file_id": file_id}
        ]
    })
    conversation.append({"role": "system", "content": "Nutze die PDF als Grundlage für Zusammenfassungen und den Lernplan"})

if __name__ == "__main__":
    while True:
        user = input("Du: ")
        if user.strip().lower() in {"quit", "exit"}:
            break
        ask_gpt(user)
        time.sleep(0.1)