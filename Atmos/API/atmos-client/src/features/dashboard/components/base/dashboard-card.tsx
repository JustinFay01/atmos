import { FlexColumn } from "@/ui/layout/flexbox";
import { Card, Skeleton, type CardProps } from "@mui/material";

export type DashboardCardProps = CardProps & {
  loading?: boolean;
};

export const DashboardCard = ({
  children,
  loading,
  ...rest
}: DashboardCardProps) => {
  if (loading) {
    return <DashboardCardSkeleton {...rest} />;
  }

  return <Card {...rest}>{children}</Card>;
};

export const DashboardCardSkeleton = (props: CardProps) => {
  return (
    <Card {...props}>
      <FlexColumn>
        <Skeleton variant="text" width="60%" height={40} />
        <Skeleton variant="text" width="40%" height={30} />
      </FlexColumn>
    </Card>
  );
};
