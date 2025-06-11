import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

// Replace with your SignalR hub URL
const connection = new HubConnectionBuilder()
  .withUrl("http://localhost:5276/dashboard") // Must be HTTP, not WS
  .configureLogging(LogLevel.Information)
  .build();

// Subscribe to server-pushed method
connection.on("ReceiveDashboardUpdate", (reading: any) => {
  console.log("📡 Received Dashboard Update:");
  console.log(JSON.stringify(reading, null, 2));
});

async function start() {
  try {
    await connection.start();
    console.log("✅ Connected to SignalR hub");

    // Keep running indefinitely
    process.stdin.resume();
  } catch (err) {
    console.error("❌ Connection failed:", err);
  }
}

start();
