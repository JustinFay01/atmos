import type { ReadingAggregate } from "@/types";
import { readingKeys } from "./reading-keys";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/axios";
import type { AxiosResponse } from "axios";

const READING_URL = "api/v1/reading-aggregate";

/**
 * Query parameters for aggregating readings within a specified time range.
 *
 * @property from - (Optional) The start date and time (inclusive) for the aggregation, in UTC.
 * @property to - (Optional) The end date and time (inclusive) for the aggregation, in UTC.
 */
type ReadingAggregateQueryParams = {
  from?: Date;
  to?: Date;
};

export const getReadingAggregates = async (
  params?: ReadingAggregateQueryParams
): Promise<ReadingAggregate[]> => {
  let url = `${import.meta.env.VITE_BASE_URL}/${READING_URL}`;

  if (params?.from || params?.to) {
    url += "?";
    if (params?.from) {
      url += `from=${params.from.toISOString()}&`;
    }
    if (params?.to) {
      url += `to=${params.to.toISOString()}`;
    }
  }

  const response = (await api.get(url)) as AxiosResponse<ReadingAggregate[]>;

  // Convert dates to local time
  response.data.forEach((reading: ReadingAggregate) => {
    reading.timestamp = new Date(reading.timestamp).toLocaleString();
    reading.temperatureMinTime = new Date(
      reading.temperatureMinTime
    ).toLocaleString();
    reading.temperatureMaxTime = new Date(
      reading.temperatureMaxTime
    ).toLocaleString();
    reading.humidityMinTime = new Date(
      reading.humidityMinTime
    ).toLocaleString();
    reading.humidityMaxTime = new Date(
      reading.humidityMaxTime
    ).toLocaleString();
    reading.dewPointMinTime = new Date(
      reading.dewPointMinTime
    ).toLocaleString();
    reading.dewPointMaxTime = new Date(
      reading.dewPointMaxTime
    ).toLocaleString();
  });
  response.data.sort((a, b) => {
    return (
      -1 * (new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime())
    );
  });

  return response.data as ReadingAggregate[];
};

export const useGetReadingAggregates = (
  params?: ReadingAggregateQueryParams
) => {
  return useQuery({
    queryKey: readingKeys.all,
    queryFn: () => getReadingAggregates(params),
  });
};
