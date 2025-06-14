import * as signalR from "@microsoft/signalr";

export const buildConnection = (hubUrl: string): signalR.HubConnection => {
  return new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .configureLogging(signalR.LogLevel.Information)
    .withAutomaticReconnect()
    .withHubProtocol(new signalR.JsonHubProtocol())
    .build();
};

export default buildConnection;
