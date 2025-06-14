import { queryConfig } from "@/lib/react-query";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";

import theme from "./theme";
import { useEffect } from "react";
import {
  startDashboardConnection,
  stopDashboardConnection,
} from "@/api/real-time/dashboard-connection";

const appQueryClient = new QueryClient({
  defaultOptions: queryConfig,
});

export const AtmosProvider = ({ children }: React.PropsWithChildren) => {
  useEffect(() => {
    console.log("Starting dashboard connection...");
    startDashboardConnection();
    return () => {
      console.log("Stopping dashboard connection...");
      stopDashboardConnection();
    };
  }, []);

  return (
    <QueryClientProvider client={appQueryClient}>
      <CssBaseline />
      <ReactQueryDevtools initialIsOpen={false} />
      <ThemeProvider theme={theme}>{children}</ThemeProvider>
    </QueryClientProvider>
  );
};
