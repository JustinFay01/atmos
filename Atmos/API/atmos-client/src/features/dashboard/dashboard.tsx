import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";
import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Button, Card, Typography } from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { LiveClock } from "./components/live-clock";
import { Dial } from "./components/dial";
import { LineChart } from "@mui/x-charts/LineChart";

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
      <FlexColumn alignItems="center" sx={{ padding: 2 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Dashboard
        </Typography>
        <Typography variant="body1" component="p">
          Connection Status: {connectionStore}
        </Typography>
        <LiveClock />
      </FlexColumn>
      <FlexColumn alignItems="center" sx={{ padding: 2 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Atmos
        </Typography>

        <FlexRow spacing={2}>
          <Dial
            value={
              dashboardStore?.latestUpdate?.temperature.currentValue.value ?? 0
            }
            unit="°F"
            label="Temperature"
          />
          <Dial
            value={
              dashboardStore?.latestUpdate?.humidity.currentValue.value ?? 0
            }
            unit="%"
            label="Humidity"
          />
          <Dial
            value={
              dashboardStore?.latestUpdate?.dewPoint.currentValue.value ?? 0
            }
            unit="°F"
            label="Dew Point"
          />
        </FlexRow>
        <LineChart
          width={600}
          height={300}
          xAxis={[
            {
              dataKey: "timestamp",
              valueFormatter: (value: number) => {
                const date = new Date(value);
                const minutes = date.getMinutes().toString().padStart(2, "0");
                const seconds = date.getSeconds().toString().padStart(2, "0");
                return `${minutes}:${seconds}`;
              },
            },
          ]}
          series={[
            {
              dataKey: "value",
            },
          ]}
          dataset={dashboardStore?.recentUpdates.reduce((acc, update) => {
            acc.push({
              timestamp: new Date(update.temperature.currentValue.timestamp),
              value: update.temperature.currentValue.value,
            });
            return acc;
          }, [] as { timestamp: Date; value: number }[])}
        />
        <LineChart
          title="Recent Temperature Updates"
          series={[
            {
              data: dashboardStore?.recentUpdates.reduce((acc, update) => {
                const value = update.temperature.currentValue.value;
                acc.push(value);
                return acc;
              }, [] as number[]),
            },
          ]}
          height={300}
        />
        <FlexColumn alignItems="center" sx={{ marginTop: 2 }}>
          <Card sx={{ padding: 2, width: "100%" }}>
            <FlexRow
              spacing={2}
              justifyContent="space-between"
              alignItems="center"
            >
              <Typography variant="h6" component="h2" gutterBottom>
                Historical Data
              </Typography>
              <Button
                variant="outlined"
                color="primary"
                disabled={readings.isLoading}
                onClick={() => {
                  readings.refetch();
                }}
              >
                View
              </Button>
            </FlexRow>
            <Typography variant="body1" component="p">
              {readings.isLoading
                ? "Loading historical data..."
                : readings.data?.map((reading) => (
                    <div key={reading.timestamp}>
                      <strong>
                        {new Date(reading.timestamp).toLocaleString()}
                      </strong>
                      : Temp: {reading.temperatureMin}°F -{" "}
                      {reading.temperatureMax}°F, Humidity:{" "}
                      {reading.humidityMin}% - {reading.humidityMax}%, Dew
                      Point: {reading.dewPointMin}°C - {reading.dewPointMax}°C
                    </div>
                  )) || "No historical data available."}
            </Typography>
          </Card>
        </FlexColumn>
      </FlexColumn>
    </BaseLayout>
  );
};
