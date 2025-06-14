import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";
import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Card, Typography } from "@mui/material";

export const Dashboard = () => {
  const dashboardStore = useDashboardStore();
  const connectionStore = useConnectionStore((state) => state.status);

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
                {dashboardStore?.latestUpdate?.temperature.currentValue.value}°C
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
      </FlexColumn>
    </BaseLayout>
  );
};
