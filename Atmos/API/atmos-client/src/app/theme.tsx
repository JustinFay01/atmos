import { createTheme } from "@mui/material/styles";

const theme = createTheme({
  palette: {
    primary: {
      main: "#01A7C2", // moonstone
      light: "#0095B6", // cerulean light
      dark: "#007090", // cerulean dark
      contrastText: "#FFFFFF",
    },
    secondary: {
      main: "#A3BAC3", // cadetGray
      light: "#EAEBED", // cardBg light
      dark: "#9CA5B6", // cardBg dark
    },
    text: {
      secondary: "#A3BAC3", // text secondary
    },
  },
});

export default theme;
