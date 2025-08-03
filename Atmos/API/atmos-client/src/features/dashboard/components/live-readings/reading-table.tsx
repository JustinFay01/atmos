import { FlexRow } from "@/ui/layout/flexbox";
import {
  Box,
  Card,
  CardContent,
  Divider,
  Tooltip,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { AnimatePresence, motion } from "framer-motion";
import { Thermostat, Opacity, Grain, ShowChart } from "@mui/icons-material";

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

const getTemperatureColor = (temp: number | null) => {
  if (temp === null) return "inherit";
  if (temp > 85) return "error.main";
  if (temp < 32) return "info.main";
  return "text.primary";
};

export const ReadingTable = ({ title, readings }: ReadingTableProps) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));

  function round(num: number | null, fractionDigits: number): number | null {
    if (num === null || isNaN(num)) {
      return null;
    }
    return Number(num.toFixed(fractionDigits));
  }

  function formatReading(value: number | null, unit: string): string {
    if (value === null || isNaN(value)) {
      return "N/A";
    }
    return `${round(value, 1)} ${unit}`;
  }

  // A self-contained component for rendering a single row/card.
  const ReadingRow = ({ reading }: { reading: TableReading }) => (
    <Tooltip
      title={`Last updated: ${new Date(reading.timestamp).toLocaleTimeString(
        [],
        {
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
        }
      )}`}
      placement="top"
    >
      <motion.div
        layout
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, scale: 0.8 }}
        transition={{ duration: 0.4, ease: "easeInOut" }}
      >
        <Card variant="outlined" sx={{ mb: 1.5 }}>
          <CardContent sx={{ p: 1.5, "&:last-child": { pb: 1.5 } }}>
            {isMobile ? (
              // Mobile View: Stacked layout in a card
              <Box>
                <FlexRow
                  justifyContent="space-between"
                  alignItems="center"
                  mb={1.5}
                >
                  <FlexRow alignItems="center" gap={1}>
                    <ShowChart fontSize="small" color="action" />
                    <Typography variant="subtitle2" color="text.secondary">
                      {new Date(reading.timestamp).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </Typography>
                  </FlexRow>
                </FlexRow>
                <FlexRow justifyContent="space-around" alignItems="center">
                  <FlexRow alignItems="center" gap={0.5}>
                    <Thermostat />
                    <Typography
                      variant="body1"
                      sx={{ fontSize: "1.1rem", fontWeight: 500 }}
                      color={getTemperatureColor(reading.temperature)}
                    >
                      {formatReading(reading.temperature, "°F")}
                    </Typography>
                  </FlexRow>
                  <FlexRow alignItems="center" gap={0.5}>
                    <Opacity />
                    <Typography
                      variant="body1"
                      sx={{ fontSize: "1.1rem", fontWeight: 500 }}
                    >
                      {formatReading(reading.humidity, "%")}
                    </Typography>
                  </FlexRow>
                  <FlexRow alignItems="center" gap={0.5}>
                    <Grain />
                    <Typography
                      variant="body1"
                      sx={{ fontSize: "1.1rem", fontWeight: 500 }}
                    >
                      {formatReading(reading.dewPoint, "°F")}
                    </Typography>
                  </FlexRow>
                </FlexRow>
              </Box>
            ) : (
              // Desktop View: Table-like row
              <FlexRow justifyContent="space-between" alignItems="center">
                <Typography
                  variant="body1"
                  sx={{ flex: 1, fontSize: "1.1rem", fontWeight: 500 }}
                >
                  <FlexRow alignItems="center" gap={1}>
                    <Thermostat color="action" />
                    <Box
                      component="span"
                      color={getTemperatureColor(reading.temperature)}
                    >
                      {formatReading(reading.temperature, "°F")}
                    </Box>
                  </FlexRow>
                </Typography>
                <Typography
                  variant="body1"
                  sx={{
                    flex: 1,
                    textAlign: "center",
                    fontSize: "1.1rem",
                    fontWeight: 500,
                  }}
                >
                  <FlexRow alignItems="center" gap={1} justifyContent="center">
                    <Opacity color="action" />
                    {formatReading(reading.humidity, "%")}
                  </FlexRow>
                </Typography>
                <Typography
                  variant="body1"
                  sx={{
                    flex: 1,
                    textAlign: "right",
                    fontSize: "1.1rem",
                    fontWeight: 500,
                  }}
                >
                  <FlexRow
                    alignItems="center"
                    gap={1}
                    justifyContent="flex-end"
                  >
                    <Grain color="action" />
                    {formatReading(reading.dewPoint, "°F")}
                  </FlexRow>
                </Typography>
              </FlexRow>
            )}
          </CardContent>
        </Card>
      </motion.div>
    </Tooltip>
  );

  return (
    <Box sx={{ maxWidth: "800px", margin: "0 auto", p: { xs: 1, sm: 2 } }}>
      <FlexRow alignItems="center" gap={1.5} sx={{ mb: 1 }}>
        <ShowChart color="primary" sx={{ fontSize: "2.5rem" }} />
        <Typography variant="h4" component="h1" sx={{ fontWeight: 600 }}>
          {title}
        </Typography>
      </FlexRow>
      <Divider sx={{ mb: 2 }} />

      {!isMobile && (
        <FlexRow justifyContent="space-between" sx={{ mb: 1, px: 2 }}>
          <Typography variant="subtitle1" sx={{ flex: 1, fontWeight: "bold" }}>
            Temperature
          </Typography>
          <Typography
            variant="subtitle1"
            sx={{ flex: 1, fontWeight: "bold", textAlign: "center" }}
          >
            Humidity
          </Typography>
          <Typography
            variant="subtitle1"
            sx={{ flex: 1, fontWeight: "bold", textAlign: "right" }}
          >
            Dew Point
          </Typography>
        </FlexRow>
      )}

      <Box
        sx={{
          width: "100%",
          overflowY: "auto",
          height: "70vh",
          pr: 0.5,
        }}
      >
        {readings.length === 0 ? (
          <Typography
            variant="body1"
            sx={{ p: 2, textAlign: "center", color: "text.secondary" }}
          >
            Awaiting first reading...
          </Typography>
        ) : (
          <AnimatePresence initial={false}>
            {readings.map((reading) => (
              <ReadingRow key={reading.timestamp} reading={reading} />
            ))}
          </AnimatePresence>
        )}
      </Box>
    </Box>
  );
};
