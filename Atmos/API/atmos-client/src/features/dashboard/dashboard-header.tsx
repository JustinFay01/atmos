import type { ConnectionStatus } from "@/stores/connection-store";
import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Box, Grid, Typography, useMediaQuery, useTheme } from "@mui/material";
import { LiveClock } from "./components/live-clock";

type DashboardHeaderProps = {
  status: ConnectionStatus;
};

export const DashboardHeader = ({ status }: DashboardHeaderProps) => {
  const theme = useTheme();

  const getLabel = (status: ConnectionStatus) => {
    switch (status) {
      case "connected":
        return "Live";
      case "disconnected":
        return "Disconnected";
      case "connecting":
        return "Connecting";
      case "reconnecting":
        return "Reconnecting";
      default:
        return "Unknown";
    }
  };

  const getColor = (status: ConnectionStatus) => {
    switch (status) {
      case "connected":
        return "success.main";
      case "disconnected":
        return "error.main";
      case "connecting":
      case "reconnecting":
        return "warning.main";
      default:
        return "primary.main";
    }
  };

  return (
    <Grid
      container
      sx={{ width: "100%", paddingLeft: 2, paddingRight: 2, paddingTop: 2 }}
      spacing={2}
      minWidth={300}
    >
      {useMediaQuery(theme.breakpoints.up("sm")) && (
        <Grid size={{ xs: 12, md: 6 }}>
          <Typography variant="h3" component="h1">
            Dashboard
          </Typography>
        </Grid>
      )}
      <Grid
        size={{ xs: 12, md: 6 }}
        width={"100%"}
        sx={{ display: "flex", justifyContent: "flex-end" }}
      >
        <FlexColumn>
          <FlexRow spacing={1}>
            {useMediaQuery(theme.breakpoints.up("md")) && (
              <Typography variant="h3" component="span">
                {getLabel(status)}
              </Typography>
            )}
            <LiveClock variant="h3" />
          </FlexRow>

          <Box
            sx={{
              height: 4,
              backgroundColor: getColor(status),
              borderRadius: 2,
            }}
          />
        </FlexColumn>
      </Grid>
    </Grid>
  );
};
