import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";

import { BaseLayout } from "@/ui/layout/blocks";
import { Grid } from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { CurrentWeatherContent } from "./components/current-weather/current-weather-content";
import { DashboardHeader } from "./dashboard-header";

export const Dashboard = () => {
  const dashboardStore = useDashboardStore();
  const connectionStore = useConnectionStore((state) => state.status);

  const readings = useQuery({
    queryKey: readingKeys.all,
    queryFn: () => getReadingAggregates(),
    enabled: false,
  });

  return (
    <BaseLayout>
      <DashboardHeader status={connectionStore} />
      <Grid container spacing={2} sx={{ marginLeft: 2, marginRight: 2 }}>
        <Grid size={4}>
          <CurrentWeatherContent
            temperature={
              dashboardStore.latestUpdate?.temperature.currentValue.value
            }
            humidity={dashboardStore.latestUpdate?.humidity.currentValue.value}
            dewPoint={dashboardStore.latestUpdate?.dewPoint.currentValue.value}
          />
        </Grid>
      </Grid>
    </BaseLayout>
  );
};
