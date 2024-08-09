import './App.css';
import { createTheme, CssBaseline, ThemeProvider } from '@mui/material';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { SnackbarProvider } from 'notistack';
import SignInPage from './pages/SignInPage/SignInPage';
import HomePage from './pages/HomePage/HomePage';
import ReportsPage from './pages/ReportsPage/ReportsPage';
import UserPage from './pages/UserPage/UserPage';
import ProjectsPage from './pages/ProjectsPage/ProjectsPage';
import ProjectPage from './pages/ProjectPage/ProjectPage';

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
              <Route path="/home" element={<HomePage/>} />
              <Route path="/reports" element={<ReportsPage/>} />
              <Route path="/users" element={<UserPage />} />
              <Route path="/projects" element={<ProjectsPage />} />
              <Route path="/project/:id" element={<ProjectPage />} />
              <Route path="/" element={<SignInPage />} />
              <Route path="*" element={<div>Not Found Page</div>} />
            </Routes>
          </BrowserRouter>
          </ThemeProvider>
    </SnackbarProvider>
  );
}

export default App;