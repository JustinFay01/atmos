import type { DashboardUpdate } from "@/types";
import { ReadingTable } from "./reading-table";
import { useMemo } from "react";

type FiveMinuteAverageTableProps = {
  readings: DashboardUpdate[];
};

export const FiveMinuteAverageTable = ({
  readings,
}: FiveMinuteAverageTableProps) => {
  const filteredReadings = useMemo(() => {
    const seenTemps = new Set<number>();
    const seenHumidity = new Set<number>();
    const seenDewPoints = new Set<number>();

    const filtered = [...readings]
      .map((update) => ({
        temperature: update.temperature.fiveMinuteRollingAverage,
        humidity: update.humidity.fiveMinuteRollingAverage,
        dewPoint: update.dewPoint.fiveMinuteRollingAverage,
        timestamp: update.humidity.currentValue.timestamp,
      }))
      .filter(({ temperature, humidity, dewPoint }) => {
        const isUnique =
          !seenTemps.has(temperature) ||
          !seenHumidity.has(humidity) ||
          !seenDewPoints.has(dewPoint);

        seenTemps.add(temperature);
        seenHumidity.add(humidity);
        seenDewPoints.add(dewPoint);

        return isUnique;
      });

    return filtered.reverse();
  }, [readings]);

  return (
    <ReadingTable title="Five Minute Averages" readings={filteredReadings} />
  );
};
