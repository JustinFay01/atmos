import { paths } from "@/config/path";
import { Dashboard } from "@/features/dashboard/dashboard";
import { createBrowserRouter, RouterProvider } from "react-router";

const createRouter = () =>
  createBrowserRouter([
    {
      path: paths.home.path,
      element: <Dashboard />,
    },
  ]);

export const AtmosRouter = () => {
  const router = createRouter();

  return <RouterProvider router={router} />;
};
