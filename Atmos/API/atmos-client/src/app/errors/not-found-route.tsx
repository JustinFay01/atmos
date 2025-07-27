// not-found.tsx
import { ErrorLayout } from "./error-layout";

export const NotFoundRoute = () => {
  return <ErrorLayout code="404" message="Page not found. Are you lost?" />;
};
