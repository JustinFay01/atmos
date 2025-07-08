import type { HourReading } from "@/types";
import {
  TableContainer,
  Card,
  Typography,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  Skeleton,
} from "@mui/material";

type TwelveHourTableProps = {
  loading: boolean;
  readings: (HourReading | null)[];
};

export const TwelveHourTable = ({
  loading,
  readings,
}: TwelveHourTableProps) => {
  if (loading) {
    return <TwelveHourTableSkeleton />;
  }

  return (
    <TableContainer component={Card} sx={{ height: "50vh", overflowY: "auto" }}>
      <Typography variant="h6" component="h2" sx={{ textAlign: "start" }}>
        Last 12 Hour Updates
      </Typography>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Time</TableCell>
            <TableCell align="right">Temperature (°F)</TableCell>
            <TableCell align="right">Humidity (%)</TableCell>
            <TableCell align="right">Dew Point (°F)</TableCell>
          </TableRow>
        </TableHead>
        <TableBody sx={{ overflowY: "auto" }}>
          {readings.map(
            (reading) =>
              reading && (
                <TableRow key={reading.hour}>
                  <TableCell>
                    {new Date(
                      new Date().setHours(reading.hour, 0, 0, 0)
                    ).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </TableCell>
                  <TableCell align="right">
                    {reading.temperature.toFixed(1)}
                  </TableCell>
                  <TableCell align="right">
                    {reading.humidity.toFixed(1)}
                  </TableCell>
                  <TableCell align="right">
                    {reading.dewPoint.toFixed(1)}
                  </TableCell>
                </TableRow>
              )
          )}
        </TableBody>
      </Table>
    </TableContainer>
  );
};

const TwelveHourTableSkeleton = () => {
  return (
    <TableContainer component={Card} sx={{ height: "50vh", overflowY: "auto" }}>
      <Typography variant="h6" component="h2" sx={{ textAlign: "start" }}>
        Last 12 Hour Updates
      </Typography>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Time</TableCell>
            <TableCell align="right">Temperature (°F)</TableCell>
            <TableCell align="right">Humidity (%)</TableCell>
            <TableCell align="right">Dew Point (°F)</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {Array.from({ length: 12 }).map((_, index) => (
            <TableRow key={index}>
              <TableCell>
                <Skeleton variant="text" width="100px" />
              </TableCell>
              <TableCell align="right">
                <Skeleton variant="text" width="50px" />
              </TableCell>
              <TableCell align="right">
                <Skeleton variant="text" width="50px" />
              </TableCell>
              <TableCell align="right">
                <Skeleton variant="text" width="50px" />
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
};
