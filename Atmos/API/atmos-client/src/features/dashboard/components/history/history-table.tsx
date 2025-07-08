import type { ReadingAggregate } from "@/types";
import { DataGrid } from "@mui/x-data-grid";

type HistoryTableProps = {
  readings: ReadingAggregate[];
  isLoading: boolean;
};

export const HistoryTable = ({ readings, isLoading }: HistoryTableProps) => {
  return (
    <DataGrid
      sx={{
        background: `linear-gradient(140deg, #292953, #292970)`,
        minHeight: 375,
        border: "none",
      }}
      rows={readings || []}
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
      loading={isLoading}
      pageSizeOptions={[10, 25, 50, 100]}
    />
  );
};
