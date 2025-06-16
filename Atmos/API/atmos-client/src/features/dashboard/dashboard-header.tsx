import { FlexColumn, FlexRow, FlexSpacer } from "@/ui/layout/flexbox";
import { Box, Typography } from "@mui/material";
import { LiveClock } from "./components/live-clock";
import type { ConnectionStatus } from "@/stores/connection-store";

type DashboardHeaderProps = {
  status: ConnectionStatus;
};

export const DashboardHeader = ({ status }: DashboardHeaderProps) => {
  const getLabel = (status: ConnectionStatus) => {
    switch (status) {
      case "connected":
        return "Live";
      case "disconnected":
        return "Disconnected";
      case "connecting":
        return "Connecting";
      case "reconnecting":
        return "Reconnecting...";
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
    <FlexColumn sx={{ padding: 2 }} width={"100%"}>
      <FlexRow spacing={2} alignItems="center">
        <Typography variant="h3" component="h1">
          Dashboard
        </Typography>
        <FlexSpacer />
        <FlexColumn>
          <FlexRow spacing={1} alignItems="center">
            <Typography variant="h3" component="span">
              {getLabel(status)}
            </Typography>
            <FlexSpacer />
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
      </FlexRow>
    </FlexColumn>
  );
};
