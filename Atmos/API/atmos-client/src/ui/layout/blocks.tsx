/* eslint-disable @typescript-eslint/no-empty-object-type */
import { Box, Stack } from "@mui/material";

import type { SxProps, Theme } from "@mui/material/styles";

export type WithChildren<T = {}> = React.PropsWithChildren<T>;
// export type ElementalChild = React.ReactElement
// export type WithElementalChild<T = {}> = T & { children?: ElementalChild }
export type SupportsItemSx<P extends string = "itemSx"> = {
  [prop in P]?: SxProps<Theme>;
};
export type SupportsSx = { sx?: SxProps<Theme>; className?: string };

export type WithSx<T = {}> = T & SupportsSx;
export type WithChildrenAndSx<T = {}> = WithChildren<WithSx<T>>;

export const LayoutContainer = (props: WithChildren) => {
  const { children } = props;
  return (
    <Stack direction="column" alignItems="stretch" sx={{ minHeight: "100vh" }}>
      {children}
    </Stack>
  );
};

export const Main = ({ children, sx }: WithChildrenAndSx) => (
  <Box
    component="main"
    flex="1 1 auto"
    sx={{
      backgroundColor: "#E2E2E2",
      ...sx,
    }}
  >
    {children}
  </Box>
);

export const BaseLayout = ({ children }: WithChildren) => {
  return (
    <LayoutContainer>
      <Main>{children}</Main>
    </LayoutContainer>
  );
};
