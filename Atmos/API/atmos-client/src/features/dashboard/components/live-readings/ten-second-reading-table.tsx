import type { DashboardUpdate } from "@/types";
import { ReadingTable } from "./reading-table";
import { useMemo } from "react";

type TenSecondReadingTableProps = {
  readings: DashboardUpdate[];
};

export const TenSecondReadingTable = ({
  readings,
}: TenSecondReadingTableProps) => {
  const filteredReadings = useMemo(() => {
    return [...readings].reverse().map((update) => ({
      temperature: update.temperature.currentValue.value,
      humidity: update.humidity.currentValue.value,
      dewPoint: update.dewPoint.currentValue.value,
      timestamp: update.humidity.currentValue.timestamp,
    }));
  }, [readings]);

  return (
    <ReadingTable title="Ten Second Readings" readings={filteredReadings} />
  );
};
