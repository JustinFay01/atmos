import { queryConfig } from "@/lib/react-query";
import { ThemeProvider } from "@mui/material";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import theme from "./theme";

const appQueryClient = new QueryClient({
  defaultOptions: queryConfig,
});

export const AtmosProvider = ({ children }: React.PropsWithChildren) => {
  return (
    <QueryClientProvider client={appQueryClient}>
      <ThemeProvider theme={theme}>{children}</ThemeProvider>
    </QueryClientProvider>
  );
};
