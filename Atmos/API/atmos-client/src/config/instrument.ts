import * as Sentry from "@sentry/react";

if (!import.meta.env.VITE_SENTRY_ENABLED) {
  console.warn("Sentry is not enabled. Sentry will not be initialized.");
} else {
  Sentry.init({
    dsn: import.meta.env.VITE_SENTRY_DSN,
    sendDefaultPii: true,
    integrations: [],
  });
}
