import {
  useLatestDashboardReading,
  useSubscribeToDashboardUpdates,
} from "@/api/get-dashboard-update";
import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Card, Typography } from "@mui/material";
import { useEffect, useState } from "react";

export const Dashboard = () => {
  useSubscribeToDashboardUpdates();
  const data = useLatestDashboardReading();
  const [temperature, setTemperature] = useState(
    data?.latestReading.temperature || 0
  );
  const [humidity, setHumidity] = useState(data?.latestReading.humidity || 0);
  const [dewPoint, setDewPoint] = useState(data?.latestReading.dewPoint || 0);

  useEffect(() => {
    if (data) {
      setTemperature(data.latestReading.temperature);
      setHumidity(data.latestReading.humidity);
      setDewPoint(data.latestReading.dewPoint);
    }
  }, [data, data?.latestReading]);

  return (
    <BaseLayout>
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
                {temperature}°C
              </Typography>
            </FlexColumn>
          </Card>
          <Card sx={{ padding: 2 }}>
            <FlexColumn alignItems="center">
              <Typography variant="h6" component="h2">
                Humidity
              </Typography>
              <Typography variant="body1" component="p">
                {humidity}%
              </Typography>
            </FlexColumn>
          </Card>
          <Card sx={{ padding: 2 }}>
            <FlexColumn alignItems="center">
              <Typography variant="h6" component="h2">
                Dew Point
              </Typography>
              <Typography variant="body1" component="p">
                {dewPoint}°C
              </Typography>
            </FlexColumn>
          </Card>
        </FlexRow>
      </FlexColumn>
    </BaseLayout>
  );
};
