import { create } from "zustand";

type ConnectionStatus =
  | "connecting"
  | "connected"
  | "reconnecting"
  | "disconnected"
  | "error";

type ConnectionStore = {
  status: ConnectionStatus;
  hasReceivedUpdate: boolean;
  setConnectionStatus: (status: ConnectionStatus) => void;
  setHasReceivedUpdate: (hasReceivedUpdate: boolean) => void;
};

export const useConnectionStore = create<ConnectionStore>((set) => ({
  status: "connecting",
  hasReceivedUpdate: false,
  setConnectionStatus: (status: ConnectionStatus) =>
    set((state) => ({
      ...state,
      status,
    })),
  setHasReceivedUpdate: (hasReceivedUpdate: boolean) =>
    set((state) => ({
      ...state,
      hasReceivedUpdate,
    })),
}));
