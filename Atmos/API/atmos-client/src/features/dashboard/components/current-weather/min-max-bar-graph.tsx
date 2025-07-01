import type { DashboardUpdate } from "@/types";
import { BarChart } from "@mui/x-charts";

export const MinMaxBarGraph = ({
  latestReading,
}: {
  latestReading: DashboardUpdate | null;
}) => {
  const toDataSet = (latestReading: DashboardUpdate | null) => {
    if (!latestReading) {
      return [];
    }

    return [
      {
        readingType: "Temperature",
        minValue: latestReading.temperature.minValue.value,
        maxValue: latestReading.temperature.maxValue.value,
        currentValue: latestReading.temperature.currentValue.value,
      },
      {
        readingType: "Humidity",
        minValue: latestReading.humidity.minValue.value,
        maxValue: latestReading.humidity.maxValue.value,
        currentValue: latestReading.humidity.currentValue.value,
      },
      {
        readingType: "Dew Point",
        minValue: latestReading.dewPoint.minValue.value,
        maxValue: latestReading.dewPoint.maxValue.value,
        currentValue: latestReading.dewPoint.currentValue.value,
      },
    ];
  };
  return (
    <BarChart
      dataset={toDataSet(latestReading)}
      xAxis={[{ dataKey: "readingType" }]}
      barLabel={"value"}
      series={[
        {
          dataKey: "minValue",
          label: "Min Value",
        },
        {
          dataKey: "maxValue",
          label: "Max Value",
        },
        {
          dataKey: "currentValue",
          label: "Current Value",
        },
      ]}
    />
  );
};
