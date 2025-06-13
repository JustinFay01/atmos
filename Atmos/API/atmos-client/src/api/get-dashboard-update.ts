import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";

type Reading = {
  temperature: number;
  humidity: number;
  dewPoint: number;
};

type Metric = {
  date: string;
  value: number;
};

type MetricAggregate = {
  currentValue: Metric;
};

type DashBoardUpdate = {
  latestReading: Reading;
  temperature: MetricAggregate;
  humidity: MetricAggregate;
  dewPoint: MetricAggregate;
};

export const useSubscribeToDashboardUpdates = () => {
  const queryClient = useQueryClient();
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5276/dashboard") // Must be HTTP, not WS
      .configureLogging(LogLevel.Information)
      .build();
    connection.on("ReceiveDashboardUpdate", (reading: DashBoardUpdate) => {
      console.log(
        "📡 Received Dashboard Update at " + new Date().toISOString()
      );
      const queryKey = [reading.latestReading];
      queryClient.setQueryData(queryKey, reading);
    });

    connection.start().catch((err) => {
      console.error("❌ Error starting connection:", err);
    });

    return () => {
      connection.stop().catch((err) => {
        console.error("❌ Error stopping connection:", err);
      });
    };
  }, [queryClient]);
};
