import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";

import type { ReadingAggregate } from "@/types";
import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow, FlexSpacer } from "@/ui/layout/flexbox";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import RefreshIcon from "@mui/icons-material/Refresh";
import {
  Button,
  Dialog,
  DialogActions,
  Fab,
  FormControl,
  FormControlLabel,
  FormGroup,
  Grid,
  IconButton,
  InputLabel,
  LinearProgress,
  MenuItem,
  Select,
  Switch,
  Tab,
  Tabs,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { useQuery } from "@tanstack/react-query";
import { useDialogs } from "@toolpad/core/useDialogs";
import xlsx, { type IJsonSheet } from "json-as-xlsx";
import { useState } from "react";
import { CurrentWeatherContent } from "./components/current-weather/current-weather-content";
import { DashboardHeader } from "./dashboard-header";

export const Dashboard = () => {
  const dashboardStore = useDashboardStore();
  const connectionStore = useConnectionStore((state) => state.status);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [exporting, setExporting] = useState(false);

  const excelSettings = {
    extraLength: 8,
    writeMode: "writeFile",
  };

  const dialogs = useDialogs();

  const readings = useQuery({
    queryKey: readingKeys.all,
    queryFn: () => getReadingAggregates(),
    enabled: false,
  });

  const downloadExcel = async (fileName: string, data: ReadingAggregate[]) => {
    const formattedData: IJsonSheet[] = [
      {
        columns: [
          { label: "Timestamp", value: "timestamp" },
          { label: "Temperature (°F)", value: "temperature" },
          { label: "Humidity (%)", value: "humidity" },
          { label: "Dew Point (°F)", value: "dewPoint" },
          { label: "Temperature Min Time", value: "temperatureMinTime" },
          { label: "Temperature Min (°F)", value: "temperatureMin" },
          {
            label: "Temperature One Minute Average (°F)",
            value: "temperatureOneMinuteAverage",
          },
          {
            label: "Temperature Five Minute Average (°F)",
            value: "temperatureFiveMinuteAverage",
          },
          { label: "Temperature Max Time", value: "temperatureMaxTime" },
          { label: "Temperature Max (°F)", value: "temperatureMax" },
          {
            label: "Humidity One Minute Average (%)",
            value: "humidityOneMinuteAverage",
          },
          {
            label: "Humidity Five Minute Average (%)",
            value: "humidityFiveMinuteAverage",
          },
          { label: "Humidity Min Time", value: "humidityMinTime" },
          { label: "Humidity Min (%)", value: "humidityMin" },
          { label: "Humidity Max Time", value: "humidityMaxTime" },
          { label: "Humidity Max (%)", value: "humidityMax" },
          {
            label: "Dew Point One Minute Average (°F)",
            value: "dewPointOneMinuteAverage",
          },
          {
            label: "Dew Point Five Minute Average (°F)",
            value: "dewPointFiveMinuteAverage",
          },
          { label: "Dew Point Min Time", value: "dewPointMinTime" },
          { label: "Dew Point Min (°F)", value: "dewPointMin" },
          { label: "Dew Point Max Time", value: "dewPointMaxTime" },
          { label: "Dew Point Max (°F)", value: "dewPointMax" },
        ],
        content: data,
      },
    ];

    xlsx(formattedData, {
      ...excelSettings,
      fileName: fileName,
    });
  };

  const downloadInFormat = async (
    fileName: string,
    type: FileType,
    data: ReadingAggregate[]
  ) => {
    switch (type) {
      case "xlsx":
        await downloadExcel(fileName, data);
        break;
    }
  };

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
              <DataGrid
                sx={{
                  background: `linear-gradient(140deg, #292953, #292970)`,
                  minHeight: 375,
                  border: "none",
                }}
                rows={readings.data || []}
                columns={[
                  { field: "timestamp", headerName: "Timestamp", width: 180 },
                  {
                    field: "temperature",
                    headerName: "Temperature (°F)",
                    width: 150,
                  },
                  { field: "humidity", headerName: "Humidity (%)", width: 150 },
                  {
                    field: "dewPoint",
                    headerName: "Dew Point (°F)",
                    width: 150,
                  },
                  {
                    field: "temperatureOneMinuteAverage",
                    headerName: "Temperature 1 Min Avg (°F)",
                    width: 200,
                  },
                  {
                    field: "temperatureFiveMinuteAverage",
                    headerName: "Temperature 5 Min Avg (°F)",
                    width: 200,
                  },
                  {
                    field: "humidityOneMinuteAverage",
                    headerName: "Humidity 1 Min Avg (%)",
                    width: 200,
                  },
                  {
                    field: "humidityFiveMinuteAverage",
                    headerName: "Humidity 5 Min Avg (%)",
                    width: 200,
                  },
                  {
                    field: "dewPointOneMinuteAverage",
                    headerName: "Dew Point 1 Min Avg (°F)",
                    width: 200,
                  },
                  {
                    field: "dewPointFiveMinuteAverage",
                    headerName: "Dew Point 5 Min Avg (°F)",
                    width: 200,
                  },
                  {
                    field: "temperatureMinTime",
                    headerName: "Temperature Min Time",
                    width: 180,
                  },
                  {
                    field: "temperatureMin",
                    headerName: "Temperature Min (°F)",
                    width: 150,
                  },
                  {
                    field: "temperatureMaxTime",
                    headerName: "Temperature Max Time",
                    width: 180,
                  },
                  {
                    field: "temperatureMax",
                    headerName: "Temperature Max (°F)",
                    width: 150,
                  },
                  {
                    field: "humidityMinTime",
                    headerName: "Humidity Min Time",
                    width: 180,
                  },
                  {
                    field: "humidityMin",
                    headerName: "Humidity Min (%)",
                    width: 150,
                  },
                  {
                    field: "humidityMaxTime",
                    headerName: "Humidity Max Time",
                    width: 180,
                  },
                  {
                    field: "humidityMax",
                    headerName: "Humidity Max (%)",
                    width: 150,
                  },
                  {
                    field: "dewPointMinTime",
                    headerName: "Dew Point Min Time",
                    width: 180,
                  },
                  {
                    field: "dewPointMin",
                    headerName: "Dew Point Min (°F)",
                    width: 150,
                  },
                  {
                    field: "dewPointMaxTime",
                    headerName: "Dew Point Max Time",
                    width: 180,
                  },
                  {
                    field: "dewPointMax",
                    headerName: "Dew Point Max (°F)",
                    width: 150,
                  },
                ]}
                loading={readings.isLoading}
                pageSizeOptions={[10, 25, 50, 100]}
              />
            </FlexColumn>
          </Grid>
        )}
      </Grid>
    </BaseLayout>
  );
};

type FileType = "xlsx" | "json" | "txt";

type ExportDialogResult = {
  confirmed: boolean;
  fileName: string | null;
  fileType: FileType;
  startDate: Date | null;
  endDate: Date | null;
};

type ExportDialogProps = {
  open: boolean;
  onClose: (result: ExportDialogResult) => Promise<void>;
};

function ExportDialog({ open, onClose }: ExportDialogProps) {
  const [useTimestamp, setUseTimestamp] = useState(true);
  const [fileName, setFileName] = useState<string | null>(null);
  const [fileType, setFileType] = useState<FileType>("xlsx");
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);

  return (
    <Dialog
      open={open}
      onClose={() => {
        onClose({
          confirmed: false,
          fileName: null,
          fileType: fileType,
          startDate: startDate,
          endDate: endDate,
        });
      }}
      fullWidth
      maxWidth="md"
    >
      <FlexColumn spacing={2}>
        <Typography variant="body2" color="textSecondary">
          Choose your export options here.
        </Typography>
        <FormGroup>
          <FormControl fullWidth>
            <InputLabel id="export-format-label">Export Format</InputLabel>
            <Select
              labelId="export-format-label"
              label="Export Format"
              defaultValue="xlsx"
              onChange={(e) => setFileType(e.target.value as FileType)}
              value={fileType}
            >
              <MenuItem value="xlsx">XLSX</MenuItem>
              <MenuItem value="json">JSON</MenuItem>
              <MenuItem value="txt">TXT</MenuItem>
            </Select>
          </FormControl>
          <FormControlLabel
            control={
              <Switch
                checked={!!useTimestamp}
                onChange={(e) => setUseTimestamp(e.target.checked)}
              />
            }
            label={"Use Timestamp for Date Selection"}
          />
        </FormGroup>
        <LocalizationProvider dateAdapter={AdapterDayjs}>
          {!useTimestamp ? (
            <>
              <DatePicker
                label="Start Date"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setStartDate(date);
                }}
              />
              <DatePicker
                label="End Date"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setEndDate(date);
                }}
              />
            </>
          ) : (
            <>
              <DateTimePicker
                label="Start Date and Time"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setStartDate(date);
                }}
              />
              <DateTimePicker
                label="End Date and Time"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setEndDate(date);
                }}
              />
            </>
          )}
        </LocalizationProvider>
        <TextField
          id="file-name"
          label="File Name"
          variant="outlined"
          value={fileName}
          onChange={(e) => setFileName(e.target.value)}
        />
        <DialogActions>
          <Button
            variant="outlined"
            onClick={() =>
              onClose({
                confirmed: false,
                fileName: null,
                fileType: fileType,
                startDate: startDate,
                endDate: endDate,
              })
            }
          >
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={() => {
              onClose({
                confirmed: true,
                fileName: fileName,
                fileType: fileType,
                startDate: startDate,
                endDate: endDate,
              });
            }}
          >
            Export
          </Button>
        </DialogActions>
      </FlexColumn>
    </Dialog>
  );
}
