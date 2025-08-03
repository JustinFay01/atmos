import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import {
  Divider,
  Tooltip,
  Typography,
  useMediaQuery,
  type Theme,
} from "@mui/material";
import { DashboardCard } from "../base/dashboard-card";

type MinmaxCardProps = {
  max: number | null;
  min: number | null;
  maxTimestamp: string | null;
  minTimestamp: string | null;
  unit: string;
  label: string;
  loading?: boolean;
};

export const MinMaxCard = ({
  max,
  min,
  maxTimestamp,
  minTimestamp,
  unit = "°F",
  label = "Value",
  loading,
}: MinmaxCardProps) => {
  const isMobile = useMediaQuery((theme: Theme) =>
    theme.breakpoints.down("sm")
  );

  const formatTimestamp = (timestamp: string | null): string => {
    if (!timestamp) return "N/A";
    const date = new Date(timestamp);
    return date.toLocaleString("en-US", {
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const renderValue = (
    value: number | null,
    type: "Max" | "Min",
    timestamp: string | null
  ) => {
    const content = (
      <FlexColumn>
        <Typography variant="h3" component="h3" sx={{ fontWeight: 625 }}>
          {`${value ?? 0} ${unit}`}
        </Typography>
        <Typography variant="h6" component="h3">
          {`${type} ${label}`}
        </Typography>
        {isMobile && (
          <Typography variant="caption" color="text.secondary">
            {formatTimestamp(timestamp)}
          </Typography>
        )}
      </FlexColumn>
    );

    if (isMobile) {
      return content;
    }

    return (
      <Tooltip title={`${formatTimestamp(timestamp) ?? "N/A"} `}>
        {content}
      </Tooltip>
    );
  };

  return (
    <DashboardCard loading={loading}>
      <FlexRow
        sx={{
          justifyContent: "space-evenly",
          alignItems: { xs: "flex-start", sm: "center" },
          flexDirection: { xs: "column", sm: "row" },
          gap: { xs: 2, sm: 0 },
        }}
      >
        {renderValue(max, "Max", maxTimestamp)}
        <Divider
          orientation={isMobile ? "horizontal" : "vertical"}
          flexItem
          sx={isMobile ? { width: "100%" } : {}}
        />
        {renderValue(min, "Min", minTimestamp)}
      </FlexRow>
    </DashboardCard>
  );
};
