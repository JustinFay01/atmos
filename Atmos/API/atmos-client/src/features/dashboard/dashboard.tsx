import { useSubscribeToDashboardUpdates } from "@/api/get-dashboard-update";
import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Card, Typography } from "@mui/material";

export const Dashboard = () => {
  const liveUpdate = useSubscribeToDashboardUpdates();

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
                22°C
              </Typography>
            </FlexColumn>
          </Card>
          <Card sx={{ padding: 2 }}>
            <FlexColumn alignItems="center">
              <Typography variant="h6" component="h2">
                Humidity
              </Typography>
              <Typography variant="body1" component="p">
                60%
              </Typography>
            </FlexColumn>
          </Card>
          <Card sx={{ padding: 2 }}>
            <FlexColumn alignItems="center">
              <Typography variant="h6" component="h2">
                Dew Point
              </Typography>
              <Typography variant="body1" component="p">
                15°C
              </Typography>
            </FlexColumn>
          </Card>
        </FlexRow>
      </FlexColumn>
    </BaseLayout>
  );
};
