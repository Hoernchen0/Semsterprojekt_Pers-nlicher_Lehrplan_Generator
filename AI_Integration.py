from openai import OpenAI
from dotenv import load_dotenv
import os
import time as time_module
from pydantic import BaseModel
from typing import List
from dict2xml import dict2xml
from datetime import time, datetime


client = OpenAI(
    base_url= os.getenv("AZURE_OPENAI_ENDPOINT"),
    api_key=os.getenv("AZURE_OPENAI_API_KEY")
)

input_text="Platzhalter"

conversation = [
    {"role": "system", "content": "Hilf dem Nutzer einen Lernplan zu erstellen und"
                                                    "seine Fragen zu Themen zu beantworten. "
                                  "Du sollst im nicht einfach im Chat den Lernplan ausgeben,..."
                                  "Du kannst den Nutzer auch fragen ob du im selbst von dir erstellte Praxisübungen"
                                  "erstellen sollst. Du kannst ihn auch fragen welche Themen er besonders "
                                  "intensiv wiederholen will, das ist sehr wichtig für die richtige Zeitaufteilung"
                                  "im Lernplan. Für die Erstellung ist auch wichtig zu fragen zu welchen Zeiten"
                                  "der Nutzer lernen möchte. Plane die Themen über die 2 Wochen so, "
                                  "dass Wiederholungen für schwierige Themen vorgesehen sind. Lege zuerst die Themen an,"
                                  " die für das Verständnis der anderen Themen am wichtigsten sind"}
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


class Task(BaseModel):
    title: str
    start_time: str
    end_time: str
    description: str


class DayPlan(BaseModel):
    day: str
    tasks: List[Task]

class StudyPlan(BaseModel):
    topic: str
    days: List[DayPlan]

def create_xml():
    response = client.beta.chat.completions.parse(
        model="gpt-5-chat",
        messages=[{"role": "system", "content": "Erstelle einen Lernplan und gib ihn NUR als XML zurück. day soll das Format"
                                                "%d.%m.%Y haben und start_time und end_time das Format: %H:%M"},
        *conversation],
        response_format=StudyPlan
    )
    study_plan = response.choices[0].message.parsed

    study_plan_dict = study_plan.model_dump()
    xml_string = dict2xml(study_plan_dict, wrap="Lernplan", indent="  ")
    print(xml_string)



if __name__ == "__main__":
    while True:
        message = input("Du: ").strip()
        if not message:  # leere Eingaben überspringen
            continue

        lower_msg = message.lower()
        if lower_msg in {"quit", "exit"}:
            break
        elif "xml" in lower_msg:
            create_xml()
            continue
        else:
            ask_gpt(message)
            time_module.sleep(0.1)