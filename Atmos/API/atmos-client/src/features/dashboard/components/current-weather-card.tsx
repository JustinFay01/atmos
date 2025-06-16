import { FlexColumn } from "@/ui/layout/flexbox";
import { Card, Typography, type CardProps } from "@mui/material";

type CurrentWeatherCardProps = CardProps & {
  value?: number;
  unit: string;
  label: string;
};

export const CurrentWeatherCard = ({
  unit,
  value,
  label,
  ...cardProps
}: CurrentWeatherCardProps) => {
  return (
    <Card sx={{ width: "25%", borderRadius: 5 }} {...cardProps}>
      <FlexColumn>
        <Typography variant="h3" component="h3">
          {`${value ?? 0} ${unit}`}
        </Typography>
        <Typography variant="h6" component="h3">
          {label}
        </Typography>
      </FlexColumn>
    </Card>
  );
};
