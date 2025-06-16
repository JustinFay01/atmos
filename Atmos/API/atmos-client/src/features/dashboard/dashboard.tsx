import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";
import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Button, Card, Typography } from "@mui/material";
import { useQuery } from "@tanstack/react-query";

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
      </FlexColumn>
      <FlexColumn alignItems="center" sx={{ padding: 2 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Atmos
        </Typography>

        <FlexRow spacing={2}>
          <Card sx={{ padding: 2 }}>
            <FlexColumn alignItems="center">
              <Typography variant="h6" component="h2">
                Temp
              </Typography>
              <Typography variant="body1" component="p">
                {dashboardStore?.latestUpdate?.temperature.currentValue.value}°F
              </Typography>
              <Typography variant="body2" component="p">
                Min: {dashboardStore?.latestUpdate?.temperature.minValue.value}
                °F
              </Typography>
              <Typography variant="body2" component="p">
                Max: {dashboardStore?.latestUpdate?.temperature.maxValue.value}
                °F
              </Typography>
            </FlexColumn>
          </Card>
          <Card sx={{ padding: 2 }}>
            <FlexColumn alignItems="center">
              <Typography variant="h6" component="h2">
                Humidity
              </Typography>
              <Typography variant="body1" component="p">
                {dashboardStore?.latestUpdate?.humidity.currentValue.value}%
              </Typography>
            </FlexColumn>
          </Card>
          <Card sx={{ padding: 2 }}>
            <FlexColumn alignItems="center">
              <Typography variant="h6" component="h2">
                Dew Point
              </Typography>
              <Typography variant="body1" component="p">
                {dashboardStore?.latestUpdate?.dewPoint.currentValue.value}°C
              </Typography>
            </FlexColumn>
          </Card>
        </FlexRow>
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
