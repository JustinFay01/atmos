import type { DashboardUpdate, HourReading } from "@/types";
import { buildConnection } from "./connection-builder";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";

let connection: signalR.HubConnection | null = null;

export const startDashboardConnection = () => {
  const DASHBOARD_URL = import.meta.env.VITE_BASE_URL + "/v1/dashboard";
  const DASHBOARD_UPDATE = "ReceiveDashboardUpdate";
  const HOURLY_UPDATE = "ReceiveHourlyUpdate";
  if (connection) {
    return connection;
  }

  connection = buildConnection(DASHBOARD_URL);

  const { setConnectionStatus, setHasReceivedUpdate } =
    useConnectionStore.getState();
  const { addUpdate, setHourUpdate } = useDashboardStore.getState();

  setConnectionStatus("connecting");

  connection.on(DASHBOARD_UPDATE, (data: DashboardUpdate) => {
    setConnectionStatus("connected");
    addUpdate(data);
    setHasReceivedUpdate(true);
  });

  connection.on(HOURLY_UPDATE, (data: HourReading[]) => {
    setConnectionStatus("connected");
    setHourUpdate(data);
    setHasReceivedUpdate(true);
    console.log("Received hourly update:", data);
  });

  connection.onreconnecting(() => setConnectionStatus("reconnecting"));
  connection.onreconnected(() => setConnectionStatus("connected"));
  connection.onclose(() => setConnectionStatus("disconnected"));

  connection
    .start()
    .then(() => setConnectionStatus("connected"))
    .catch((err) => {
      console.error("Failed to connect:", err);
      setConnectionStatus("error");
    });

  return connection;
};

export const stopDashboardConnection = () => {
  if (connection) {
    connection.stop().then(() => {
      connection = null;
      useConnectionStore.getState().setConnectionStatus("disconnected");
    });
  }
};
