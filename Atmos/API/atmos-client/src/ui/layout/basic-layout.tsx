import { LayoutContainer, Main, type WithChildrenAndSx } from "./blocks";

export const BasicLayout = ({ children }: WithChildrenAndSx) => {
  return (
    <LayoutContainer>
      <Main>{children}</Main>
    </LayoutContainer>
  );
};
