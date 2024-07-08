import * as signalR from "@microsoft/signalr";

const txtUsername: HTMLInputElement = document.getElementById(
  "txtUsername"
) as HTMLInputElement;
const txtPassword: HTMLInputElement = document.getElementById(
  "txtPassword"
) as HTMLInputElement;
const btnLogin: HTMLButtonElement = document.getElementById(
  "btnLogin"
) as HTMLButtonElement;

const divLogin: HTMLDivElement = document.getElementById(
  "divLogin"
) as HTMLDivElement;

const lblUsername: HTMLLabelElement = document.getElementById(
  "lblUsername"
) as HTMLLabelElement;
const txtMessage: HTMLInputElement = document.getElementById(
  "txtMessage"
) as HTMLInputElement;
const txtToUser: HTMLInputElement = document.getElementById(
  "txtToUser"
) as HTMLInputElement;
const btnSend: HTMLButtonElement = document.getElementById(
  "btnSend"
) as HTMLButtonElement;

const txtToGroup = document.getElementById("txtToGroup") as HTMLInputElement;
const btnJoinGroup = document.getElementById(
  "btnJoinGroup"
) as HTMLButtonElement;
const btnLeaveGroup = document.getElementById(
  "btnLeaveGroup"
) as HTMLButtonElement;

const lblStatus = document.getElementById("lblStatus") as HTMLLabelElement;

const divChat: HTMLDivElement = document.getElementById(
  "divChat"
) as HTMLDivElement;

divChat.style.display = "none";
btnSend.disabled = true;
btnLeaveGroup.disabled = true;
btnLeaveGroup.style.display = "none";

btnLogin.addEventListener("click", login);
let connection: signalR.HubConnection = null;
async function login() {
  const username = txtUsername.value;
  const password = txtPassword.value;

  if (username && password) {
    try {
      // Use the Fetch API to login
      const response = await fetch("https://localhost:7159/account/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      const json = await response.json();

      localStorage.setItem("token", json.token);
      localStorage.setItem("username", username);
      txtUsername.value = "";
      txtPassword.value = "";
      lblUsername.textContent = username;
      divLogin.style.display = "none";
      divChat.style.display = "block";
      txtMessage.focus();

      // Start the SignalR connection
      connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7159/chatHub", {
          transport:
            signalR.HttpTransportType.WebSockets |
            signalR.HttpTransportType.LongPolling,
          accessTokenFactory: () => {
            var localToken = localStorage.getItem("token");
            // You can add logic to check if the token is valid or expired
            return localToken;
          },
        })
        .configureLogging(signalR.LogLevel.Debug)
        .withAutomaticReconnect()
        //.withAutomaticReconnect([0, 5, 20])
        .withStatefulReconnect({ bufferSize: 200000 })
        .build();
      // The following configuration must match the configuration in the server project
      // connection.keepAliveIntervalInMilliseconds = 10000;
      // connection.serverTimeoutInMilliseconds = 20000;
      connection.on("ReceiveMessage", (username: string, message: string) => {
        const li = document.createElement("li");
        li.setAttribute("class", "list-group-item");
        li.textContent = `${username}: ${message}`;
        const messageList = document.getElementById("messages");
        messageList.appendChild(li);
        messageList.scrollTop = messageList.scrollHeight;
      });

      connection.on("UserConnected", (username: string) => {
        const li = document.createElement("li");
        li.setAttribute("class", "list-group-item");
        li.textContent = `${username} connected`;
        const messageList = document.getElementById("messages");
        messageList.appendChild(li);
        messageList.scrollTop = messageList.scrollHeight;
      });
      connection.on("UserDisconnected", (username: string) => {
        const li = document.createElement("li");
        li.setAttribute("class", "list-group-item");
        li.textContent = `${username} disconnected`;
        const messageList = document.getElementById("messages");
        messageList.appendChild(li);
        messageList.scrollTop = messageList.scrollHeight;
      });

      connection.onclose(() => {
        lblStatus.textContent = "Disconnected.";
        btnSend.disabled = true;
        btnJoinGroup.disabled = true;
        btnLeaveGroup.disabled = true;
        btnJoinGroup.style.display = "none";
        btnLeaveGroup.style.display = "none";
        txtToGroup.readOnly = false;
      });
      connection.onreconnecting((error) => {
        lblStatus.textContent = `${error} Reconnecting...`;
        btnSend.disabled = true;
        btnJoinGroup.disabled = true;
        btnLeaveGroup.disabled = true;
        btnJoinGroup.style.display = "none";
        btnLeaveGroup.style.display = "none";
        txtToGroup.readOnly = false;
      });
      connection.onreconnected((connectionId) => {
        lblStatus.textContent = `Connected. ${connectionId}`;
        btnSend.disabled = false;
        btnJoinGroup.disabled = false;
        btnLeaveGroup.disabled = true;
        btnJoinGroup.style.display = "inline";
        btnLeaveGroup.style.display = "none";
        txtToGroup.readOnly = false;
      });

      await connection.start();
      lblStatus.textContent = `Connected. ${connection.connectionId}`;
      btnSend.disabled = false;
    } catch (err) {
      lblStatus.textContent = `Disconnected. ${err.toString()}`;
      console.error(err.toString());
    }
  }
}

// const connection = new signalR.HubConnectionBuilder()
//   .withUrl("https://localhost:7159/chatHub")
//   .build();

txtMessage.addEventListener("keyup", (event) => {
  if (event.key === "Enter") {
    sendMessage();
  }
});

btnSend.addEventListener("click", sendMessage);

function sendMessage() {
  // If the txtToUser field is not empty, send the message to the user
  if (txtToGroup.value && txtToGroup.readOnly === true) {
    connection
      .invoke(
        "SendMessageToGroup",
        lblUsername.textContent,
        txtToGroup.value,
        txtMessage.value
      )
      .catch((err) => console.error(err.toString()))
      .then(() => (txtMessage.value = ""));
  } else if (txtToUser.value) {
    connection
      .invoke(
        "SendMessageToUser",
        lblUsername.textContent,
        txtToUser.value,
        txtMessage.value
      )
      .catch((err) => console.error(err.toString()))
      .then(() => (txtMessage.value = ""));
  } else {
    connection
      .invoke("SendMessage", lblUsername.textContent, txtMessage.value)
      .catch((err) => console.error(err.toString()))
      .then(() => (txtMessage.value = ""));
  }
}

btnJoinGroup.addEventListener("click", joinGroup);
btnLeaveGroup.addEventListener("click", leaveGroup);

function joinGroup() {
  if (txtToGroup.value) {
    connection
      .invoke("AddToGroup", lblUsername.textContent, txtToGroup.value)
      .catch((err) => console.error(err.toString()))
      .then(() => {
        btnJoinGroup.disabled = true;
        btnJoinGroup.style.display = "none";
        btnLeaveGroup.disabled = false;
        btnLeaveGroup.style.display = "inline";
        txtToGroup.readOnly = true;
      });
  }
}

function leaveGroup() {
  if (txtToGroup.value) {
    connection
      .invoke("RemoveFromGroup", lblUsername.textContent, txtToGroup.value)
      .catch((err) => console.error(err.toString()))
      .then(() => {
        btnJoinGroup.disabled = false;
        btnJoinGroup.style.display = "inline";
        btnLeaveGroup.disabled = true;
        btnLeaveGroup.style.display = "none";
        txtToGroup.readOnly = false;
      });
  }
}
