import { FlexRow } from "@/ui/layout/flexbox";
import { Box, Divider, Typography } from "@mui/material";
import { AnimatePresence, motion } from "framer-motion";

type TableReading = {
  temperature: number | null;
  humidity: number | null;
  dewPoint: number | null;
  timestamp: string;
};

type ReadingTableProps = {
  title: string;
  readings: TableReading[];
};

export const ReadingTable = ({ title, readings }: ReadingTableProps) => {
  function round(num: number | null, fractionDigits: number): number | null {
    if (num === null || isNaN(num)) {
      return null;
    }

    return Number(num.toFixed(fractionDigits));
  }

  function formatReading(num: number | null, unit: string): string {
    if (num === null || isNaN(num)) {
      return "N/A";
    }
    return `${round(num, 2)} ${unit}`;
  }

  return (
    <Box sx={{ minWidth: "300px" }}>
      <Typography variant="h5">{title}</Typography>
      <Divider sx={{ marginBottom: 2 }} />
      <FlexRow alignItems="start" justifyContent="space-between">
        <Typography variant="h6">Temperature</Typography>
        <Typography variant="h6">Humidity</Typography>
        <Typography variant="h6">Dew Point</Typography>
      </FlexRow>
      <Box sx={{ width: "100%" }}>
        <Box
          sx={{
            width: "100%",
            overflowY: "auto",
            height: "70vh",
            paddingRight: 2,
          }}
        >
          {readings.length === 0 && (
            <Typography variant="body1" sx={{ padding: 2 }}>
              No readings available.
            </Typography>
          )}
          <AnimatePresence>
            {readings.length > 0 &&
              readings.map((update) => (
                <motion.div
                  key={`${update.temperature}-${update.humidity}-${update.dewPoint}`}
                  layout
                  initial={{ opacity: 0, y: -20 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, y: -20 }}
                >
                  <FlexRow
                    alignItems="start"
                    sx={{
                      borderBottom: "1px solid #ccc",
                      width: "100%",
                      padding: 1,
                    }}
                    gap={2}
                    padding={1}
                    justifyContent={"space-between"}
                  >
                    <Typography variant="body1">
                      {formatReading(update.temperature, "°F")}
                    </Typography>
                    <Typography variant="body1" sx={{ marginLeft: 2 }}>
                      {formatReading(update.humidity, "%")}
                    </Typography>
                    <Typography variant="body1" sx={{ marginLeft: 2 }}>
                      {formatReading(update.dewPoint, "°F")}
                    </Typography>
                  </FlexRow>
                </motion.div>
              ))}
          </AnimatePresence>
        </Box>
      </Box>
    </Box>
  );
};
