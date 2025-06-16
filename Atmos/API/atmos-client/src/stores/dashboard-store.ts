import type { DashboardUpdate } from "@/types";
import { create } from "zustand";

type DashboardStore = {
  latestUpdate: DashboardUpdate | null;
  recentUpdates: DashboardUpdate[];
  addUpdate: (data: DashboardUpdate) => void;
  clearUpdates: () => void;
};

export const useDashboardStore = create<DashboardStore>((set) => ({
  recentUpdates: [],
  latestUpdate: null,
  addUpdate: (data: DashboardUpdate) =>
    set((state) => ({
      recentUpdates: [...state.recentUpdates, data].slice(-1000), // Keep only the last 10 updates
      latestUpdate: data,
    })),
  clearUpdates: () => set({ recentUpdates: [], latestUpdate: null }),
}));
