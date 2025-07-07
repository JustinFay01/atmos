import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";

import type { FileType } from "@/types";
import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow, FlexSpacer } from "@/ui/layout/flexbox";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import RefreshIcon from "@mui/icons-material/Refresh";
import {
  Card,
  Fab,
  Grid,
  IconButton,
  LinearProgress,
  Tab,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tabs,
  Tooltip,
  Typography,
} from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { useDialogs } from "@toolpad/core/useDialogs";
import { useMemo, useState } from "react";
import { CurrentWeatherContent } from "./components/current-weather/current-weather-content";
import { MinMaxBarGraph } from "./components/current-weather/min-max-bar-graph";
import { MinMaxCard } from "./components/current-weather/min-max-card";
import { ExportDialog } from "./components/history/export-dialog";
import { HistoryTable } from "./components/history/history-table";
import { downloadInFormat } from "./components/history/util/export-handler";
import { FiveMinuteAverageTable } from "./components/live-readings/five-minute-average-table";
import { OneMinuteAverageTable } from "./components/live-readings/one-minute-average-table";
import { TenSecondReadingTable } from "./components/live-readings/ten-second-reading-table";
import { DashboardHeader } from "./dashboard-header";

export const Dashboard = () => {
  const CURRENT_READINGS_INDEX = 0;
  const HISTORICAL_DATA_INDEX = 1;
  const LIVE_READINGS_INDEX = 2;

  const dashboardStore = useDashboardStore();
  const connectionStore = useConnectionStore((state) => state.status);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [exporting, setExporting] = useState(false);

  const dialogs = useDialogs();

  const readings = useQuery({
    queryKey: readingKeys.all,
    queryFn: () => getReadingAggregates(),
    enabled: false,
  });

  const onSubmit = async (
    fileName: string | null,
    fileType: FileType,
    startDate: Date | null,
    endDate: Date | null
  ) => {
    setExporting(true);
    try {
      const data = await getReadingAggregates({
        from: startDate,
        to: endDate,
      });

      let finalFileName = fileName
        ? fileName.trim()
        : `atmos_readings${startDate ? `_${startDate.toDateString()}` : ""}${endDate ? `_${endDate.toDateString()}` : ""}`;
      finalFileName = finalFileName.replace(/\s+/g, "_");
      await downloadInFormat(finalFileName, fileType, data);
    } catch (error) {
      console.error("Error exporting data:", error);
    }
  };

  const onExport = async () => {
    const result = await dialogs.open(ExportDialog);
    if (!result.confirmed) {
      return;
    }
    setExporting(true);
    await onSubmit(
      result.fileName,
      result.fileType,
      result.startDate,
      result.endDate
    );
    setExporting(false);
  };

  const orderTwelveHourUpdates = useMemo(() => {
    // Get current hour (0-23)
    const currentHour = new Date().getHours();
    // Build an array of the last 12 hours, starting from currentHour
    const last12Hours = Array.from(
      { length: 12 },
      (_, i) => (currentHour - i + 24) % 24
    );
    // Reorder twelveHourUpdates to match last12Hours
    return last12Hours.map((hour) =>
      dashboardStore.hourUpdates.find((r) => r?.hour === hour)
    );
  }, [dashboardStore.hourUpdates]);

  return (
    <BaseLayout>
      <DashboardHeader status={connectionStore} />
      <Tabs
        value={selectedIndex}
        onChange={(_, newValue) => setSelectedIndex(newValue)}
      >
        <Tab label="Current Weather" />
        <Tab label="Historical Data" />
        <Tab label="Live Readings" />
      </Tabs>
      <Grid container>
        {selectedIndex === CURRENT_READINGS_INDEX && (
          <>
            <Grid
              size={{ sm: 12, md: 3 }}
              padding={2}
              sx={{ width: "100%", height: "100%" }}
              visibility={
                selectedIndex === CURRENT_READINGS_INDEX ? "visible" : "hidden"
              }
            >
              <CurrentWeatherContent
                temperature={
                  dashboardStore.latestUpdate?.temperature.currentValue.value
                }
                humidity={
                  dashboardStore.latestUpdate?.humidity.currentValue.value
                }
                dewPoint={
                  dashboardStore.latestUpdate?.dewPoint.currentValue.value
                }
              />
            </Grid>
            <Grid
              size={{ sm: 12, md: 5.5 }}
              padding={2}
              sx={{ overflowY: "auto" }}
            >
              <TableContainer
                component={Card}
                sx={{ height: "50vh", overflowY: "auto" }}
              >
                <Typography
                  variant="h6"
                  component="h2"
                  sx={{ textAlign: "start" }}
                >
                  Last 12 Hour Updates
                </Typography>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Time</TableCell>
                      <TableCell>Temperature (°F)</TableCell>
                      <TableCell>Humidity (%)</TableCell>
                      <TableCell>Dew Point (°F)</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody sx={{ overflowY: "auto" }}>
                    {orderTwelveHourUpdates.map(
                      (reading) =>
                        reading && (
                          <TableRow key={reading.hour}>
                            <TableCell>
                              {new Date(
                                new Date().setHours(reading.hour, 0, 0, 0)
                              ).toLocaleTimeString([], {
                                hour: "2-digit",
                                minute: "2-digit",
                              })}
                            </TableCell>
                            <TableCell>
                              {reading.temperature.toFixed(1)}
                            </TableCell>
                            <TableCell>{reading.humidity.toFixed(1)}</TableCell>
                            <TableCell>{reading.dewPoint.toFixed(1)}</TableCell>
                          </TableRow>
                        )
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </Grid>
            <Grid size={{ sm: 12, md: 3.5 }} padding={2}>
              <FlexColumn gap={2}>
                <MinMaxCard
                  loading={!dashboardStore.latestUpdate}
                  label="Temperature"
                  unit="°F"
                  max={
                    dashboardStore.latestUpdate?.temperature.maxValue.value ||
                    null
                  }
                  min={
                    dashboardStore.latestUpdate?.temperature.minValue.value ||
                    null
                  }
                  maxTimestamp={
                    dashboardStore.latestUpdate?.temperature.maxValue
                      .timestamp || null
                  }
                  minTimestamp={
                    dashboardStore.latestUpdate?.temperature.minValue
                      .timestamp || null
                  }
                />
                <MinMaxCard
                  loading={!dashboardStore.latestUpdate}
                  label="Humidity"
                  unit="%"
                  max={
                    dashboardStore.latestUpdate?.humidity.maxValue.value || null
                  }
                  min={
                    dashboardStore.latestUpdate?.humidity.minValue.value || null
                  }
                  maxTimestamp={
                    dashboardStore.latestUpdate?.humidity.maxValue.timestamp ||
                    null
                  }
                  minTimestamp={
                    dashboardStore.latestUpdate?.humidity.minValue.timestamp ||
                    null
                  }
                />
                <MinMaxCard
                  loading={!dashboardStore.latestUpdate}
                  label="Dew Point"
                  unit="°F"
                  max={
                    dashboardStore.latestUpdate?.dewPoint.maxValue.value || null
                  }
                  min={
                    dashboardStore.latestUpdate?.dewPoint.minValue.value || null
                  }
                  maxTimestamp={
                    dashboardStore.latestUpdate?.dewPoint.maxValue.timestamp ||
                    null
                  }
                  minTimestamp={
                    dashboardStore.latestUpdate?.dewPoint.minValue.timestamp ||
                    null
                  }
                />
              </FlexColumn>
            </Grid>
            <Grid size={{ sm: 12, md: 8 }}>
              <MinMaxBarGraph
                latestReading={dashboardStore.latestUpdate || null}
              />
            </Grid>
            <Grid size={{ sm: 12, md: 4 }}>
              {/* Sensor connection status */}
            </Grid>
          </>
        )}
        {selectedIndex === HISTORICAL_DATA_INDEX && (
          <Grid size={12} sx={{ height: "80vh" }} padding={2}>
            <FlexColumn gap={2} sx={{ height: "100%" }}>
              <FlexRow alignItems="end" spacing={3}>
                <FlexSpacer />
                <Tooltip title="Refresh Data">
                  <IconButton
                    color="primary"
                    onClick={() => {
                      readings.refetch();
                    }}
                    size="large"
                  >
                    <RefreshIcon />
                  </IconButton>
                </Tooltip>
                <Tooltip title="Export Data">
                  <Fab
                    aria-label="export"
                    variant="extended"
                    color="primary"
                    onClick={async () => await onExport()}
                    size="large"
                  >
                    <FileDownloadIcon />
                    Export
                  </Fab>
                </Tooltip>
              </FlexRow>
              <LinearProgress
                sx={{
                  visibility: exporting ? "visible" : "hidden",
                  padding: 0,
                  margin: 0,
                }}
              />
              <HistoryTable
                readings={readings.data || []}
                isLoading={readings.isLoading}
              />
            </FlexColumn>
          </Grid>
        )}
        {selectedIndex === LIVE_READINGS_INDEX && (
          <>
            <Grid size={{ sm: 12, md: 4 }} padding={2}>
              <TenSecondReadingTable readings={dashboardStore.recentUpdates} />
            </Grid>
            <Grid size={{ sm: 12, md: 4 }} padding={2}>
              <OneMinuteAverageTable readings={dashboardStore.recentUpdates} />
            </Grid>
            <Grid size={{ sm: 12, md: 4 }} padding={2}>
              <FiveMinuteAverageTable readings={dashboardStore.recentUpdates} />
            </Grid>
          </>
        )}
      </Grid>
    </BaseLayout>
  );
};
