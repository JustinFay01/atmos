import { AtmosProvider } from "./provider";
import { AtmosRouter } from "./router";

export const Atmos = () => {
  return (
    <AtmosProvider>
      <AtmosRouter />
    </AtmosProvider>
  );
};
