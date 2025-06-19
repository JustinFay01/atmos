import type { ReadingAggregate, FileType } from "@/types";
import xlsx, { type IJsonSheet } from "json-as-xlsx";

const excelSettings = {
  extraLength: 8,
  writeMode: "writeFile",
};

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

const downloadJson = async (fileName: string, data: ReadingAggregate[]) => {
  const jsonContent = JSON.stringify(data, null, 2);
  const blob = new Blob([jsonContent], { type: "application/json" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

export const downloadInFormat = async (
  fileName: string,
  type: FileType,
  data: ReadingAggregate[]
) => {
  switch (type) {
    case "xlsx":
      await downloadExcel(fileName, data);
      break;
    case "json":
      await downloadJson(fileName, data);
      break;
  }
};
