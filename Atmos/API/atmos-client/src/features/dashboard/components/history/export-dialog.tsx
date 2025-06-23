import type { FileType } from "@/types";
import { FlexColumn } from "@/ui/layout/flexbox";
import {
  Dialog,
  Typography,
  FormGroup,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormControlLabel,
  Switch,
  TextField,
  DialogActions,
  Button,
} from "@mui/material";
import { useState } from "react";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DateTimePicker } from "@mui/x-date-pickers/DateTimePicker";

type ExportDialogResult = {
  confirmed: boolean;
  fileName: string;
  fileType: FileType;
  startDate: Date | null;
  endDate: Date | null;
};

type ExportDialogProps = {
  open: boolean;
  onClose: (result: ExportDialogResult) => Promise<void>;
};

export function ExportDialog({ open, onClose }: ExportDialogProps) {
  const [useTimestamp, setUseTimestamp] = useState(true);
  const [fileName, setFileName] = useState<string>("");
  const [fileType, setFileType] = useState<FileType>("xlsx");
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);

  return (
    <Dialog
      open={open}
      onClose={() => {
        onClose({
          confirmed: false,
          fileName: "",
          fileType: fileType,
          startDate: startDate,
          endDate: endDate,
        });
      }}
      fullWidth
      maxWidth="md"
    >
      <FlexColumn spacing={2}>
        <Typography variant="body2" color="textSecondary">
          Choose your export options here.
        </Typography>
        <FormGroup>
          <FormControl fullWidth>
            <InputLabel id="export-format-label">Export Format</InputLabel>
            <Select
              labelId="export-format-label"
              label="Export Format"
              defaultValue="xlsx"
              onChange={(e) => setFileType(e.target.value as FileType)}
              value={fileType}
            >
              <MenuItem value="xlsx">XLSX</MenuItem>
              <MenuItem value="json">JSON</MenuItem>
              <MenuItem value="txt">TXT</MenuItem>
            </Select>
          </FormControl>
          <FormControlLabel
            control={
              <Switch
                checked={!!useTimestamp}
                onChange={(e) => setUseTimestamp(e.target.checked)}
              />
            }
            label={"Use Timestamp for Date Selection"}
          />
        </FormGroup>
        <LocalizationProvider dateAdapter={AdapterDayjs}>
          {!useTimestamp ? (
            <>
              <DatePicker
                label="Start Date"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setStartDate(date);
                }}
              />
              <DatePicker
                label="End Date"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setEndDate(date);
                }}
              />
            </>
          ) : (
            <>
              <DateTimePicker
                label="Start Date and Time"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setStartDate(date);
                }}
              />
              <DateTimePicker
                label="End Date and Time"
                onChange={(pickerValue) => {
                  const date = pickerValue ? pickerValue.toDate() : null;
                  setEndDate(date);
                }}
              />
            </>
          )}
        </LocalizationProvider>
        <TextField
          id="file-name"
          label="File Name"
          variant="outlined"
          value={fileName}
          onChange={(e) => setFileName(e.target.value)}
        />
        <DialogActions>
          <Button
            variant="outlined"
            onClick={() =>
              onClose({
                confirmed: false,
                fileName: "",
                fileType: fileType,
                startDate: startDate,
                endDate: endDate,
              })
            }
          >
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={() => {
              onClose({
                confirmed: true,
                fileName: fileName,
                fileType: fileType,
                startDate: startDate,
                endDate: endDate,
              });
            }}
          >
            Export
          </Button>
        </DialogActions>
      </FlexColumn>
    </Dialog>
  );
}
