import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";

import { BaseLayout } from "@/ui/layout/blocks";
import { FlexColumn, FlexRow, FlexSpacer } from "@/ui/layout/flexbox";
import FileDownloadIcon from "@mui/icons-material/FileDownload";
import RefreshIcon from "@mui/icons-material/Refresh";
import {
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
  Tooltip,
  Typography,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { useQuery } from "@tanstack/react-query";
import { useDialogs } from "@toolpad/core/useDialogs";
import { useState } from "react";
import { CurrentWeatherContent } from "./components/current-weather/current-weather-content";
import { DashboardHeader } from "./dashboard-header";
import xlsx, { type IJsonSheet } from "json-as-xlsx";
import type { ReadingAggregate } from "@/types";

export const Dashboard = () => {
  const dashboardStore = useDashboardStore();
  const connectionStore = useConnectionStore((state) => state.status);
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);
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

  const downloadExcel = async (data: ReadingAggregate[]) => {
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
        content: data.map((reading) => ({
          timestamp: reading.timestamp,
          temperature: reading.temperature,
          humidity: reading.humidity,
          dewPoint: reading.dewPoint,
          temperatureMinTime: reading.temperatureMinTime,
          temperatureMin: reading.temperatureMin,
          temperatureMaxTime: reading.temperatureMaxTime,
          temperatureMax: reading.temperatureMax,
          humidityMinTime: reading.humidityMinTime,
          humidityMin: reading.humidityMin,
          humidityMaxTime: reading.humidityMaxTime,
          humidityMax: reading.humidityMax,
          dewPointMinTime: reading.dewPointMinTime,
          dewPointMin: reading.dewPointMin,
          dewPointMaxTime: reading.dewPointMaxTime,
          dewPointMax: reading.dewPointMax,
        })),
      },
    ];

    xlsx(formattedData, {
      ...excelSettings,
      fileName: `atmos_readings_${startDate?.toISOString()}_${endDate?.toISOString()}.xlsx`,
    });
  };

  const downloadInFormat = async (type: "xlsx", data: ReadingAggregate[]) => {
    if (type === "xlsx") {
      await downloadExcel(data);
    }
  };

  const onExport = async () => {
    const confirmed = await dialogs.confirm(
      <ExportDialog
        onStartDateChange={setStartDate}
        onEndDateChange={setEndDate}
      />,
      { title: "Export Data" }
    );
    if (!confirmed) return;
    setExporting(true);
    try {
      const data = await getReadingAggregates({
        from: startDate,
        to: endDate,
      });

      await downloadInFormat("xlsx", data);
    } catch (error) {
      console.error("Error exporting data:", error);
    } finally {
      setExporting(false);
    }
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
                onRowClick={(params) => {
                  //dashboardStore.setSelectedReading(params.row);
                  console.log("Row clicked:", params.row);
                }}
              />
            </FlexColumn>
          </Grid>
        )}
      </Grid>
    </BaseLayout>
  );
};

type ExportDialogProps = {
  onStartDateChange?: (date: Date | null) => void;
  onEndDateChange?: (date: Date | null) => void;
};

const ExportDialog = ({
  onStartDateChange,
  onEndDateChange,
}: ExportDialogProps) => {
  const [useTimestamp, setUseTimestamp] = useState(true);

  return (
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
            defaultValue="csv"
          >
            <MenuItem value="csv">CSV</MenuItem>
            <MenuItem value="json">JSON</MenuItem>
            <MenuItem value="txt">TXT</MenuItem>
          </Select>
        </FormControl>
        <FormControlLabel
          control={
            <Switch
              checked={!!useTimestamp}
              onChange={() => setUseTimestamp(!useTimestamp)}
            />
          }
          label="With Timestamp"
        />
      </FormGroup>
      <LocalizationProvider dateAdapter={AdapterDayjs}>
        {!useTimestamp ? (
          <>
            <DatePicker
              label="Start Date"
              onChange={(pickerValue) => {
                const date = pickerValue ? pickerValue.toDate() : null;
                onStartDateChange?.(date);
              }}
            />
            <DatePicker
              label="End Date"
              onChange={(pickerValue) => {
                const date = pickerValue ? pickerValue.toDate() : null;
                onEndDateChange?.(date);
              }}
            />
          </>
        ) : (
          <>
            <DateTimePicker
              label="Start Date and Time"
              onChange={(pickerValue) => {
                const date = pickerValue ? pickerValue.toDate() : null;
                onStartDateChange?.(date);
              }}
            />
            <DateTimePicker
              label="End Date and Time"
              onChange={(pickerValue) => {
                const date = pickerValue ? pickerValue.toDate() : null;
                onEndDateChange?.(date);
              }}
            />
          </>
        )}
      </LocalizationProvider>
    </FlexColumn>
  );
};
