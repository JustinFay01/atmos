import { Card, CardContent, Typography } from "@mui/material";
import { Gauge, type GaugeProps } from "@mui/x-charts";

type DialProps = GaugeProps & {
  unit: string;
  label: string;
};

export const Dial = ({ unit, label, ...gaugeProps }: DialProps) => {
  return (
    <Card sx={{ minWidth: 200, margin: 1, padding: 2 }}>
      <CardContent sx={{ textAlign: "center" }}>
        <Typography variant="body1" color="text.secondary">
          {label} {unit}
        </Typography>
        <Gauge sx={{ fontSize: "1.5rem" }} {...gaugeProps} />
      </CardContent>
    </Card>
  );
};
