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
import useDynamicIcon from "@/hooks/use-dynamic-icon";

const appQueryClient = new QueryClient({
  defaultOptions: queryConfig,
});

export const AtmosProvider = ({ children }: React.PropsWithChildren) => {
  useDynamicIcon();
  useEffect(() => {
    startDashboardConnection();
    return () => {
      stopDashboardConnection();
    };
  }, []);

  return (
    <QueryClientProvider client={appQueryClient}>
      <ReactQueryDevtools initialIsOpen={false} />
      <ThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </ThemeProvider>
    </QueryClientProvider>
  );
};
