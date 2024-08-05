import './App.css';
import { createTheme, CssBaseline, ThemeProvider } from '@mui/material';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { SnackbarProvider } from 'notistack';
import SignInPage from './pages/SignInPage/SignInPage';
import HomePage from './pages/HomePage/HomePage';

const theme = createTheme({
  palette: {
    primary: {
      main: '#0AC602',
    },
  },
  typography: { 'fontFamily': '"Lexend_Deca"' },
});

function App() {
  return (
    <SnackbarProvider maxSnack={3}>
      <ThemeProvider theme={theme}>
          <CssBaseline />
          <BrowserRouter>
            <Routes>
              <Route path="/" element={<HomePage/>} />
              <Route path="/sign-in" element={<SignInPage />} />
              <Route path="*" element={<div>Not Found Page</div>} />
            </Routes>
          </BrowserRouter>
          </ThemeProvider>
    </SnackbarProvider>
  );
}

export default App;