import * as Sentry from "@sentry/react";

if (import.meta.env.VITE_SENTRY_ENABLED === "false") {
  console.warn("Sentry is not enabled. Sentry will not be initialized.");
} else {
  console.info("Initializing Sentry for error tracking...");
  Sentry.init({
    dsn: import.meta.env.VITE_SENTRY_DSN,
    sendDefaultPii: true,
    integrations: [],
  });
}
