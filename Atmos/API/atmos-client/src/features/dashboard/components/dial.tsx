import { Card, CardContent, Typography } from "@mui/material";

type DialProps = {
  value: number;
  unit: string;
  label: string;
};

export const Dial = ({ value, unit, label }: DialProps) => {
  return (
    <Card sx={{ minWidth: 200, margin: 1, padding: 2 }}>
      <CardContent sx={{ textAlign: "center" }}>
        <Typography variant="h4">{label}</Typography>
        <Typography variant="h5" component="div" sx={{ marginTop: 1 }}>
          {value} {unit}
        </Typography>
      </CardContent>
    </Card>
  );
};
