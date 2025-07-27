import { BasicLayout } from "@/ui/layout/basic-layout";
import { FlexColumn } from "@/ui/layout/flexbox";
import { Box, Button, Typography } from "@mui/material";

interface ErrorLayoutProps {
  code: string;
  message: string;
  actionLabel?: string;
  actionHref?: string;
}

export const ErrorLayout = ({
  code,
  message,
  actionLabel = "Back to Home",
  actionHref = "/",
}: ErrorLayoutProps) => {
  return (
    <BasicLayout>
      <FlexColumn
        sx={{
          alignItems: "center",
          justifyContent: "center",
          height: "100vh",
          textAlign: "center",
          px: 2,
        }}
      >
        <Box
          sx={{
            borderRadius: 4,
            boxShadow: 6,
            bgcolor: "background.paper",
            p: 6,
            maxWidth: 400,
            width: "100%",
          }}
        >
          <Typography variant="h2" color="primary.main" fontWeight="bold">
            {code}
          </Typography>
          <Typography variant="h6" color="text.secondary" sx={{ mt: 1, mb: 3 }}>
            {message}
          </Typography>
          <Button variant="contained" href={actionHref}>
            {actionLabel}
          </Button>
        </Box>
      </FlexColumn>
    </BasicLayout>
  );
};
