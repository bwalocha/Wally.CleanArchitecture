import * as signalR from "@microsoft/signalr";
import "./css/main.css";

const divMessages: HTMLDivElement = document.querySelector("#divMessages");
const tbMessage: HTMLInputElement = document.querySelector("#tbMessage");
const btnSend: HTMLButtonElement = document.querySelector("#btnSend");
const username = (new Date().getTime()).toString();

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hub")
	.configureLogging(signalR.LogLevel.Trace)
    .build();

connection.on("NewEventAsync", (username: string, message: string) => {
  const m = document.createElement("div");

  m.innerHTML = `<div class="message-author">${username}</div><div>${message}</div>`;

  divMessages.appendChild(m);
  divMessages.scrollTop = divMessages.scrollHeight;
});

connection.start().catch((err) => document.write(err));

tbMessage.addEventListener("keyup", (e: KeyboardEvent) => {
  if (e.key === "Enter") {
    send();
  }
});

btnSend.addEventListener("click", send);

function send() {
	connection.invoke("SendEventAsync", username, tbMessage.value, null)
	.then(() => (tbMessage.value = ""))
	.catch(error => console.error(error.toString()));
}
