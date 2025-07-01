import { FlexRow } from "@/ui/layout/flexbox";
import { Box, Divider, Tooltip, Typography } from "@mui/material";
import { AnimatePresence, motion } from "framer-motion";

type TableReading = {
  temperature: number;
  humidity: number;
  dewPoint: number;
  timestamp: string;
};

type ReadingTableProps = {
  title: string;
  readings: TableReading[];
};

export const ReadingTable = ({ title, readings }: ReadingTableProps) => {
  function round(num: number, fractionDigits: number): number | null {
    if (isNaN(num)) {
      return null;
    }

    return Number(num.toFixed(fractionDigits));
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
      <Box sx={{ overflowY: "auto", height: "70vh" }} width={"100%"}>
        <Box sx={{ width: "100%" }} paddingRight={2}>
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
                  <Tooltip
                    title={
                      <Box>
                        <Typography variant="body2">
                          {new Date(update.timestamp).toLocaleString()}
                        </Typography>
                      </Box>
                    }
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
                        {round(update.temperature, 2)} °F
                      </Typography>
                      <Typography variant="body1" sx={{ marginLeft: 2 }}>
                        {round(update.humidity, 2)} %
                      </Typography>
                      <Typography variant="body1" sx={{ marginLeft: 2 }}>
                        {round(update.dewPoint, 2)} °F
                      </Typography>
                    </FlexRow>
                  </Tooltip>
                </motion.div>
              ))}
          </AnimatePresence>
        </Box>
      </Box>
    </Box>
  );
};
