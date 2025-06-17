import { getReadingAggregates } from "@/api/readings/get-readings";
import { readingKeys } from "@/api/readings/reading-keys";
import { useConnectionStore } from "@/stores/connection-store";
import { useDashboardStore } from "@/stores/dashboard-store";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";

import { BaseLayout } from "@/ui/layout/blocks";
import {
  Button,
  FormControlLabel,
  FormGroup,
  Grid,
  IconButton,
  Switch,
  Tab,
  Tabs,
  Tooltip,
} from "@mui/material";
import { DataGrid } from "@mui/x-data-grid";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { CurrentWeatherContent } from "./components/current-weather/current-weather-content";
import { DashboardHeader } from "./dashboard-header";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { FlexColumn, FlexRow, FlexSpacer } from "@/ui/layout/flexbox";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import RefreshIcon from "@mui/icons-material/Refresh";

export const Dashboard = () => {
  const dashboardStore = useDashboardStore();
  const connectionStore = useConnectionStore((state) => state.status);
  const [selectedIndex, setSelectedIndex] = useState(0);

  const [useTimestamp, setUseTimestamp] = useState(true);

  const readings = useQuery({
    queryKey: readingKeys.all,
    queryFn: () => getReadingAggregates(),
    enabled: false,
  });

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
      <Grid container spacing={2} sx={{ margin: 2 }}>
        {selectedIndex === 0 && (
          <Grid size={4}>
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
          <Grid size={12} sx={{ height: "80vh" }}>
            <FlexColumn gap={2} sx={{ height: "100%" }}>
              <FlexRow alignItems="center" spacing={3}>
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
                <FlexSpacer />
                <FlexColumn alignItems={"flex-end"} spacing={1}>
                  <FormGroup>
                    <FormControlLabel
                      control={
                        <Switch
                          defaultChecked={useTimestamp}
                          onChange={() => setUseTimestamp(!useTimestamp)}
                        />
                      }
                      label="By Time Stamp"
                    />
                  </FormGroup>
                  <LocalizationProvider dateAdapter={AdapterDayjs}>
                    <FlexRow spacing={2}>
                      {!useTimestamp ? (
                        <>
                          <DatePicker label="Start Date" onChange={() => {}} />
                          <DatePicker label="End Date" onChange={() => {}} />
                        </>
                      ) : (
                        <>
                          <DateTimePicker label="Basic date time picker" />
                          <DateTimePicker label="Basic date time picker" />{" "}
                        </>
                      )}
                    </FlexRow>
                  </LocalizationProvider>
                </FlexColumn>
              </FlexRow>
              <DataGrid
                sx={{
                  background: `linear-gradient(140deg, #292953, #292970)`,
                  minHeight: 375,
                  borderRadius: 5,
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
