import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";

import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow, FlexSpacer } from "@/ui/layout/flexbox";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import RefreshIcon from "@mui/icons-material/Refresh";
import {
  Fab,
  Grid,
  IconButton,
  LinearProgress,
  Tab,
  Tabs,
  Tooltip,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useQuery } from "@tanstack/react-query";
import { useDialogs } from "@toolpad/core/useDialogs";
import { useState } from "react";
import { CurrentWeatherContent } from "./components/current-weather/current-weather-content";
import { DashboardHeader } from "./dashboard-header";
import type { FileType } from "@/types";
import { downloadInFormat } from "./components/history/util/export-handler";
import { ExportDialog } from "./components/history/export-dialog";
import { HistoryTable } from "./components/history/history-table";

export const Dashboard = () => {
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

      let finalFileName =
        fileName ??
        `atmos_readings${startDate ? `_${startDate.toDateString()}` : ""}${endDate ? `_${endDate.toDateString()}` : ""}`;
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

  return (
    <BaseLayout>
      <DashboardHeader status={connectionStore} />
      <Tabs
        value={selectedIndex}
        onChange={(_, newValue) => setSelectedIndex(newValue)}
      >
        <Tab label="Current Weather" />
        <Tab label="Historical Data" />
      </Tabs>
      <Grid container spacing={2}>
        {selectedIndex === 0 && (
          <Grid size={4} padding={2}>
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
        )}
        {selectedIndex === 1 && (
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
      </Grid>
    </BaseLayout>
  );
};
