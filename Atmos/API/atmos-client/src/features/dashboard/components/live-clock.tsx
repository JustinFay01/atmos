import { Typography, type TypographyProps } from "@mui/material";
import React from "react";
import { useEffect } from "react";

type LiveClockProps = TypographyProps;

export const LiveClock = (props: LiveClockProps) => {
  const [time, setTime] = React.useState(new Date());
  const [timerId, setTimerId] = React.useState<number | null>(null);

  useEffect(() => {
    const timerId = setInterval(() => {
      setTime(new Date());
    }, 1000);
    setTimerId(timerId);

    return () => {
      if (timerId) {
        clearInterval(timerId);
      }
    };
  }, []);

  return (
    <Typography key={timerId} variant="h6" {...props}>
      {time.toLocaleTimeString()}
    </Typography>
  );
};
