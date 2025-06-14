import type { DashboardUpdate } from "@/types";
import { create } from "zustand";

type DashboardStore = {
  latestUpdate: DashboardUpdate | null;
  updateData: (data: DashboardUpdate) => void;
};

export const useDashboardStore = create<DashboardStore>((set) => ({
  latestUpdate: null,
  updateData: (data: DashboardUpdate) =>
    set((state) => ({
      ...state,
      latestUpdate: data,
    })),
}));
