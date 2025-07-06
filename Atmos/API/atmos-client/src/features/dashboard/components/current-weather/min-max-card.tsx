import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Divider, Tooltip, Typography } from "@mui/material";
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
  const formatTimestamp = (timestamp: string | null): string => {
    if (!timestamp) return "N/A";
    const date = new Date(timestamp);
    return date.toLocaleString("en-US", {
      hour: "2-digit",
      minute: "2-digit",
    });
  };
  return (
    <DashboardCard loading={loading}>
      <FlexRow sx={{ justifyContent: "space-between" }}>
        <Tooltip title={`${formatTimestamp(maxTimestamp) ?? "N/A"} `}>
          <FlexColumn>
            <Typography variant="h3" component="h3" sx={{ fontWeight: 625 }}>
              {`${max ?? 0} ${unit}`}
            </Typography>
            <Typography variant="h6" component="h6" sx={{ fontWeight: 625 }}>
              {`Max ${label}`}
            </Typography>
          </FlexColumn>
        </Tooltip>
        <Divider orientation="vertical" flexItem />
        <Tooltip title={`${formatTimestamp(minTimestamp) ?? "N/A"} `}>
          <FlexColumn>
            <Typography variant="h3" component="h3" sx={{ fontWeight: 625 }}>
              {`${min ?? 0} ${unit}`}
            </Typography>
            <Typography variant="h6" component="h6" sx={{ fontWeight: 625 }}>
              {`Min ${label}`}
            </Typography>
          </FlexColumn>
        </Tooltip>
      </FlexRow>
    </DashboardCard>
  );
};
