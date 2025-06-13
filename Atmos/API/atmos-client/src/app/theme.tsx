import { createTheme } from "@mui/material/styles";

// Override MUI Palette and PaletteOptions interfaces with our custom color keys
// Key types are defined as the same type as whatever Palette['primary'] or PaletteOptions['primary'] is
declare module "@mui/material/styles" {
  interface Palette {
    moonstone: Palette["primary"];
    cerulean: Palette["primary"];
    cadetGray: Palette["primary"];
    spaceCadet: Palette["primary"];
    cardBg: Palette["primary"];
    lightGrey: Palette["primary"];
    bg: Palette["primary"];
  }

  interface TypeText {
    // Extends TypeText with this new property
    // Bc TypeText will be defined twice, and TS will merge the defs
    secondaryLetterSpacing?: string;
  }

  interface PaletteOptions {
    moonstone: PaletteOptions["primary"];
    cerulean: PaletteOptions["primary"];
    cadetGray: PaletteOptions["primary"];
    spaceCadet: PaletteOptions["primary"];
    cardBg: PaletteOptions["primary"];
    lightGrey: PaletteOptions["primary"];
    bg: PaletteOptions["primary"];
  }
}

const theme = createTheme({
  palette: {
    moonstone: {
      main: "#01A7C2",
    },
    cerulean: {
      main: "#007090",
      light: "#0095B6",
    },
    cadetGray: {
      main: "#A3BAC3",
    },
    spaceCadet: {
      main: "#2E294E",
    },
    cardBg: {
      main: "##EAEBED",
      dark: "#9CA5B6",
    },
    lightGrey: {
      main: "#6c6f7e",
    },
    bg: {
      main: "#1A1C24",
    },
    text: {
      primary: "#FFFFFF",
      secondary: "#A3BAC3",
      secondaryLetterSpacing: ".75px",
    },
  },
});

export default theme;
