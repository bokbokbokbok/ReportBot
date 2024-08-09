import { Box, Button, Typography } from "@mui/material";
import Menu from "../../components/Menu/Menu";
import styles from "./ProjectsPage.module.css";

const ProjectsPage = () => {
  return (
    <Box className={styles.homePage}>

      <Menu activeView="projects" />
      <Box className={styles.content}>
        <Box sx={{
          display: 'flex',
          flexDirection: 'column',
          height: '600px',
          width: '90%',
          backgroundColor: '#D9D9D9',
          borderRadius: '15px',
          padding: '10px 20px',
          gap: '10px',
        }}>
          <Typography variant="h6" sx={{ fontWeight: 'bold', textAlign: 'start' }}>All Projects</Typography>
          <Box sx={{
            display: 'flex',
            flexDirection: 'column',
            height: '100%',
            width: '100%',
            borderRadius: '15px',
            overflowY: 'auto',
            gap: '10px',
            '&::-webkit-scrollbar': {
              display: 'none',
            },
            '-ms-overflow-style': 'none',
            scrollbarWidth: 'none',
          }}>
            <div style={{ height: '150px' }}>
              <Box sx={{
                display: 'flex',
                flexDirection: 'row',
                justifyContent: 'space-between',
                alignItems: 'center',
                height: '150px',
                width: '100%',
                backgroundColor: '#f5f5f5',
                borderRadius: '15px',
                padding: '0 15px',
                marginBottom: '10px',
              }}>
                <Box sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  justifyContent: 'center',
                  alignItems: 'center',
                  height: '80%',
                  width: '15%',
                  borderRadius: '15px',
                  backgroundColor: '#E1F3E0',
                  padding: '10px',
                }}>
                  <Typography variant="h6"
                    sx={{
                      width: '100%',
                      fontWeight: 'bold',
                      textAlign: 'center',
                      whiteSpace: 'nowrap',
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                    }}>MCG_DM</Typography>
                </Box>
                <Box sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center',
                  height: '80%',
                  width: '40%',
                  borderRadius: '15px',
                  backgroundColor: '#E1F3E0',
                  padding: '10px',
                  gap: '10px',
                  overflowY: 'auto',
                  '&::-webkit-scrollbar': {
                    display: 'none',
                  },
                  '-ms-overflow-style': 'none',
                  scrollbarWidth: 'none',
                }}>
                  <Box sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    justifyContent: 'center',
                    alignItems: 'center',
                    height: '30%',
                    width: '100%',
                    backgroundColor: '#d9d9d9',
                    borderRadius: '5px',
                  }}>
                    <Typography variant="h6"
                      sx={{
                        width: '100%',
                        fontWeight: 'bold',
                        fontSize: '14px',
                        textAlign: 'start',
                        marginLeft: '10px',
                        whiteSpace: 'nowrap',
                        overflow: 'hidden',
                        textOverflow: 'ellipsis',
                      }}>Data Migration</Typography>
                  </Box>
                </Box>
                <Button
                  variant="contained"
                  sx={{
                    width: "150px",
                    height: "40px",
                    borderRadius: "10px",
                    color: "white",
                    fontSize: "16px",
                    textTransform: "none",
                    marginTop: "10px",
                  }}
                >
                  View
                </Button>
              </Box>
            </div>
          </Box>
        </Box>
      </Box>
    </Box>
  );
};

export default ProjectsPage;
