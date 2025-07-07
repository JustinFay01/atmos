import { createTheme } from "@mui/material/styles";

// Custom palette tokens
const colors = {
  backgroundDark: "#0B0D2A", // Full background
  surfaceDark: "#292953", // Cards and containers
  surfaceDarkGradient: "#292970", // Gradient for cards
  primaryBlue: "#00CFFF",
  secondaryYellow: "#FDCB6E",
  textPrimary: "#FFFFFF",
  textSecondary: "#A5B1C2",
  errorRed: "#FF5C5C",
  successGreen: "#6EFFA1",
};

const theme = createTheme({
  palette: {
    mode: "dark",
    background: {
      default: colors.backgroundDark,
      paper: colors.surfaceDark,
    },
    primary: {
      main: colors.primaryBlue,
    },
    secondary: {
      main: colors.secondaryYellow,
    },
    text: {
      primary: colors.textPrimary,
      secondary: colors.textSecondary,
    },
    error: {
      main: colors.errorRed,
    },
    success: {
      main: colors.successGreen,
    },
  },
  typography: {
    fontFamily: `'Inter', 'Roboto', 'Helvetica', 'Arial', sans-serif`,
    h1: {
      fontWeight: 700,
      fontSize: "2rem",
      color: colors.textPrimary,
    },
    h2: {
      fontWeight: 600,
      fontSize: "1.5rem",
      color: colors.textPrimary,
    },

    body1: {
      fontSize: "1rem",
      color: colors.textSecondary,
    },
    body2: {
      fontSize: "0.875rem",
      color: colors.textSecondary,
    },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: {
          // fallback for Firefox
          scrollbarColor: "#888 transparent",
          scrollbarWidth: "thin",
        },
        // Chrome, Edge, Safari
        "*::-webkit-scrollbar": {
          width: "8px",
        },
        "*::-webkit-scrollbar-thumb": {
          backgroundColor: "#888",
          borderRadius: "8px",
        },
        "*::-webkit-scrollbar-track": {
          background: "transparent",
        },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundColor: colors.surfaceDark,
          borderRadius: "12px",
          padding: "1rem",
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          background: `linear-gradient(140deg, ${colors.surfaceDark}, ${colors.surfaceDarkGradient})`,
          borderRadius: "12px",
          boxShadow: "0 2px 4px rgba(0, 0, 0, 0.1)",
        },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: colors.backgroundDark,
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: "8px",
          textTransform: "none",
        },
      },
    },
    MuiIconButton: {
      styleOverrides: {
        root: {
          backgroundColor: colors.surfaceDark,
        },
      },
    },
  },
});

export default theme;
