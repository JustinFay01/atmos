import type { DashboardUpdate, HourReading as HourReading } from "@/types";
import { create } from "zustand";

const MAX_UPDATES = 100;

type DashboardStore = {
  latestUpdate: DashboardUpdate | null;
  recentUpdates: DashboardUpdate[];
  hourUpdates: (HourReading | null)[];
  setHourUpdate: (data: HourReading[]) => void;
  addUpdate: (data: DashboardUpdate) => void;
  clearUpdates: () => void;
};

export const useDashboardStore = create<DashboardStore>((set) => ({
  recentUpdates: [],
  latestUpdate: null,
  hourUpdates: [],
  setHourUpdate: (data: HourReading[]) =>
    set(() => {
      return { hourUpdates: data };
    }),
  addUpdate: (data: DashboardUpdate) =>
    set((state) => ({
      recentUpdates: [...state.recentUpdates, data].slice(-MAX_UPDATES),
      latestUpdate: data,
    })),
  clearUpdates: () => set({ recentUpdates: [], latestUpdate: null }),
}));
