import type { IContent } from "json-as-xlsx";

export type FileType = "xlsx" | "json" | "txt";

export type Reading = {
  temperature: number;
  humidity: number;
  dewPoint: number;
};

export type Metric = {
  timestamp: string;
  value: number;
};

export type MetricAggregate = {
  currentValue: Metric;
  minValue: Metric;
  maxValue: Metric;
  oneMinuteAverage: number;
  tenMinuteAverage: number;
  fiveMinuteRollingAverage: number;
  recentReadings: Metric[];
  oneMinuteAverages: Metric[];
};

export type DashboardUpdate = {
  latestReading: Reading;
  temperature: MetricAggregate;
  humidity: MetricAggregate;
  dewPoint: MetricAggregate;
};

export type HourReading = {
  hour: number;
  timestamp: string;
  temperature: number;
  humidity: number;
  dewPoint: number;
};

// Flattened ReadingAggregate for historical data to download or to display in tables
export interface ReadingAggregate extends IContent {
  timestamp: string;
  temperature: number;
  temperatureMinTime: string;
  temperatureMin: number;
  temperatureMaxTime: string;
  temperatureMax: number;
  temperatureOneMinuteAverage: number | null;
  temperatureFiveMinuteRollingAverage: number | null;
  humidity: number;
  humidityMinTime: string;
  humidityMin: number;
  humidityMaxTime: string;
  humidityMax: number;
  humidityOneMinuteAverage: number | null;
  humidityFiveMinuteRollingAverage: number | null;
  dewPoint: number;
  dewPointMinTime: string;
  dewPointMin: number;
  dewPointMaxTime: string;
  dewPointMax: number;
  dewPointOneMinuteAverage: number | null;
  dewPointFiveMinuteRollingAverage: number | null;
  id: string;
}
