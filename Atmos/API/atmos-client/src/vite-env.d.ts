/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_BASE_URL: string;
  readonly VITE_SENTRY_DSN: string;
  readonly VITE_SENTRY_ENABLED: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
