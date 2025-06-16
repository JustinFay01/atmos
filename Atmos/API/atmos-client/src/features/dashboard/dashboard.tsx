import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";

import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Button, Card, Typography } from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { CurrentWeatherCard } from "./components/current-weather-card";
import { DashboardHeader } from "./dashboard-header";

import StaticHumiditySvg from "@/assets/humidity-static.svg";
import HumiditySvg from "@/assets/humidity.svg";
import StaticMistSvg from "@/assets/mist-static.svg";
import MistSvg from "@/assets/mist.svg";
import StaticThermometerSvg from "@/assets/thermometer-static.svg";
import ThermometerSvg from "@/assets/thermometer.svg";

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
        <DashboardHeader status={connectionStore} />
        <FlexColumn
          width={"100%"}
          spacing={1}
          alignItems="start"
          sx={{ marginTop: 2 }}
        >
          <CurrentWeatherCard
            label="Temperature"
            unit="°F"
            value={
              dashboardStore?.latestUpdate?.temperature.currentValue.value ?? 0
            }
            iconSrc={StaticThermometerSvg}
            animatedIconSrc={ThermometerSvg}
          />
          <CurrentWeatherCard
            label="Humidity"
            unit="%"
            value={
              dashboardStore?.latestUpdate?.humidity.currentValue.value ?? 0
            }
            iconSrc={StaticHumiditySvg}
            animatedIconSrc={HumiditySvg}
          />
          <CurrentWeatherCard
            label="Dew Point"
            unit="°F"
            value={
              dashboardStore?.latestUpdate?.dewPoint.currentValue.value ?? 0
            }
            iconSrc={StaticMistSvg}
            animatedIconSrc={MistSvg}
          />
        </FlexColumn>

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
