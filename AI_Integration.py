from openai import OpenAI
from dotenv import load_dotenv
import os
import time

client = OpenAI()

input_text="Platzhalter"

conversation = [
    {"role": "system", "content": "You are a friendly, concise AI assistant."}
]

def ask_gpt(user_input):
    # Add user input to the conversation
    conversation.append({"role": "user", "content": user_input})
    conversation_text = "\n".join(
        [f"{m['role']}: {m['content']}" for m in conversation]
    )
    try:
        response = client.responses.create(
            model="gpt-5-mini",
            instructions="Help the User with his studying",
            input=conversation_text
        )
    except Exception as e:
        print("API error:", e)
        return None

    reply = response.output_text
    conversation.append({"role": "assistant", "content": reply})
    print(response.output_text)
    return 1

if __name__ == "__main__":
    while True:
        user = input("Du: ")
        if user.strip().lower() in {"quit", "exit"}:
            break
        ask_gpt(user)
        time.sleep(0.1)