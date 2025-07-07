import { FlexColumn, FlexRow } from "@/ui/layout/flexbox";
import { Box, Card, Typography, type CardProps } from "@mui/material";
import { useState } from "react";
import { AtmosIcon } from "@/ui/icons/atmos-icons";

type CurrentWeatherCardProps = CardProps & {
  value?: number;
  unit: string;
  label: string;
  iconSrc: string;
  animatedIconSrc: string;
};

export const CurrentWeatherCard = ({
  unit,
  value,
  label,
  iconSrc,
  animatedIconSrc,
  ...cardProps
}: CurrentWeatherCardProps) => {
  const [hovered, setHovered] = useState(false);

  return (
    <Card
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
      sx={{
        position: "relative",
        width: "100%",
        "&:hover": {
          transform: "scale(1.02)",
          transition: "transform 0.3s ease-in-out",
          boxShadow: 3,
        },
        ...cardProps.sx,
      }}
      {...cardProps}
    >
      <FlexRow>
        <FlexColumn>
          <Typography variant="h3" component="h3" sx={{ fontWeight: 625 }}>
            {`${value ?? 0} ${unit}`}
          </Typography>
          <Typography variant="h6" component="h3">
            {label}
          </Typography>
        </FlexColumn>
        <Box
          sx={{
            position: "absolute",
            borderRadius: 5,
            width: 75,
            height: 75,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            marginLeft: "auto",
            marginRight: 2,
            top: 9,
            right: -3,
            zIndex: 1,
          }}
        >
          {
            <AtmosIcon
              hovered={hovered}
              src={iconSrc}
              animatedSrc={animatedIconSrc}
              sx={{
                height: 100,
                width: 100,
              }}
            />
          }
        </Box>
      </FlexRow>
    </Card>
  );
};
