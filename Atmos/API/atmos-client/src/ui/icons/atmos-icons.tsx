import { Box, type BoxProps } from "@mui/material";
import React from "react";

type AtmosIconProps = Omit<BoxProps, "src"> & {
  src: string;
  animatedSrc?: string;
  hovered?: boolean;
  onMouseEnter?: () => void;
  onMouseLeave?: () => void;
};

export const AtmosIcon = ({
  src,
  hovered,
  animatedSrc,
  ...props
}: AtmosIconProps) => {
  const [internalHovered, setHovered] = React.useState(false);
  const handleMouseEnter = () => {
    setHovered(true);
    props.onMouseEnter?.();
  };
  const handleMouseLeave = () => {
    setHovered(false);
    props.onMouseLeave?.();
  };

  return (
    <Box
      component="img"
      src={(hovered || internalHovered) && animatedSrc ? animatedSrc : src}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      {...props}
    />
  );
};
