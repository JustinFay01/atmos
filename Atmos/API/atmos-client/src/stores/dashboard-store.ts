import type { DashboardUpdate } from "@/types";
import { create } from "zustand";

const MAX_UPDATES = 100;

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
      recentUpdates: [...state.recentUpdates, data].slice(-MAX_UPDATES),
      latestUpdate: data,
    })),
  clearUpdates: () => set({ recentUpdates: [], latestUpdate: null }),
}));
