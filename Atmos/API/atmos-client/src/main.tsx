import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { Atmos } from "./app";

createRoot(document.getElementById("root")!).render(
  // <StrictMode>
  <Atmos />
  // </StrictMode>
);
