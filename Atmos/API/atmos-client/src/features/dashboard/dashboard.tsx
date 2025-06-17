import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";

import { BaseLayout } from "@/ui/layout/blocks";
import { Grid, Tab, Tabs } from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { CurrentWeatherContent } from "./components/current-weather/current-weather-content";
import { DashboardHeader } from "./dashboard-header";

export const Dashboard = () => {
  const dashboardStore = useDashboardStore();
  const connectionStore = useConnectionStore((state) => state.status);
  const [selectedIndex, setSelectedIndex] = useState(0);

  const readings = useQuery({
    queryKey: readingKeys.all,
    queryFn: () => getReadingAggregates(),
  });

  return (
    <BaseLayout>
      <DashboardHeader status={connectionStore} />
      <Tabs
        value={selectedIndex}
        onChange={(_, newValue) => setSelectedIndex(newValue)}
      >
        <Tab label="Current Weather" />
        <Tab label="Historical Data" />
      </Tabs>
      <Grid container spacing={2} sx={{ margin: 2 }}>
        {selectedIndex === 0 && (
          <Grid size={4}>
            <CurrentWeatherContent
              temperature={
                dashboardStore.latestUpdate?.temperature.currentValue.value
              }
              humidity={
                dashboardStore.latestUpdate?.humidity.currentValue.value
              }
              dewPoint={
                dashboardStore.latestUpdate?.dewPoint.currentValue.value
              }
            />
          </Grid>
        )}
        {selectedIndex === 1 && (
          <Grid size={12} sx={{ height: "80vh" }}>
            <DataGrid
              sx={{
                background: `linear-gradient(140deg, #292953, #292970)`,
                minHeight: 375,
                borderRadius: 5,
              }}
              rows={readings.data || []}
              columns={[
                { field: "timestamp", headerName: "Timestamp", width: 180 },
                {
                  field: "temperature",
                  headerName: "Temperature (°F)",
                  width: 150,
                },
                { field: "humidity", headerName: "Humidity (%)", width: 150 },
                { field: "dewPoint", headerName: "Dew Point (°F)", width: 150 },
                {
                  field: "temperatureMinTime",
                  headerName: "Temperature Min Time",
                  width: 180,
                },
                {
                  field: "temperatureMin",
                  headerName: "Temperature Min (°F)",
                  width: 150,
                },
                {
                  field: "temperatureMaxTime",
                  headerName: "Temperature Max Time",
                  width: 180,
                },
                {
                  field: "temperatureMax",
                  headerName: "Temperature Max (°F)",
                  width: 150,
                },
                {
                  field: "humidityMinTime",
                  headerName: "Humidity Min Time",
                  width: 180,
                },
                {
                  field: "humidityMin",
                  headerName: "Humidity Min (%)",
                  width: 150,
                },
                {
                  field: "humidityMaxTime",
                  headerName: "Humidity Max Time",
                  width: 180,
                },
                {
                  field: "humidityMax",
                  headerName: "Humidity Max (%)",
                  width: 150,
                },
                {
                  field: "dewPointMinTime",
                  headerName: "Dew Point Min Time",
                  width: 180,
                },
                {
                  field: "dewPointMin",
                  headerName: "Dew Point Min (°F)",
                  width: 150,
                },
                {
                  field: "dewPointMaxTime",
                  headerName: "Dew Point Max Time",
                  width: 180,
                },
                {
                  field: "dewPointMax",
                  headerName: "Dew Point Max (°F)",
                  width: 150,
                },
              ]}
              loading={readings.isLoading}
              pageSizeOptions={[10, 25, 50, 100]}
              onRowClick={(params) => {
                //dashboardStore.setSelectedReading(params.row);
                console.log("Row clicked:", params.row);
              }}
            />
          </Grid>
        )}
      </Grid>
    </BaseLayout>
  );
};
