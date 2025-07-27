import { paths } from "@/config/path";
import { Dashboard } from "@/features/dashboard/dashboard";
import { createBrowserRouter, RouterProvider } from "react-router";
import { NotFoundRoute } from "./errors/not-found-route";

const createRouter = () =>
  createBrowserRouter([
    {
      path: paths.home.path,
      element: <Dashboard />,
    },
    {
      path: "*",
      element: <NotFoundRoute />,
    },
  ]);

export const AtmosRouter = () => {
  const router = createRouter();

  return <RouterProvider router={router} />;
};
