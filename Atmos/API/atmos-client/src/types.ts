export type Reading = {
  temperature: number;
  humidity: number;
  dewPoint: number;
};

export type Metric = {
  date: string;
  value: number;
};

export type MetricAggregate = {
  currentValue: Metric;
  minValue: Metric;
  maxValue: Metric;
  oneMinuteAverage: number;
  tenMinuteAverage: number;
  fiveMinuteAverage: number;
  recentReadings: Metric[];
  oneMinuteAverages: Metric[];
};

export type DashboardUpdate = {
  latestReading: Reading;
  temperature: MetricAggregate;
  humidity: MetricAggregate;
  dewPoint: MetricAggregate;
};
