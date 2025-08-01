import "./config/instrument";
import { createRoot } from "react-dom/client";
import { Atmos } from "./app";
import * as Sentry from "@sentry/react";

const container = document.getElementById("root") as HTMLElement;
const root = createRoot(container, {
  // Callback called when an error is thrown and not caught by an ErrorBoundary.
  onUncaughtError: Sentry.reactErrorHandler((error, errorInfo) => {
    console.warn("Uncaught error", error, errorInfo.componentStack);
  }),
  // Callback called when React catches an error in an ErrorBoundary.
  onCaughtError: Sentry.reactErrorHandler(),
  // Callback called when React automatically recovers from errors.
  onRecoverableError: Sentry.reactErrorHandler(),
});
root.render(<Atmos />);
