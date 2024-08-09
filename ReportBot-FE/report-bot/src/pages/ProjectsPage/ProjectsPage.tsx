import { Box, Button, Typography } from "@mui/material";
import Menu from "../../components/Menu/Menu";
import styles from "./ProjectsPage.module.css";
import { useEffect, useState } from "react";
import ProjectResponse from "../../api/models/response/ProjectResponse";
import Project from "../../api/Project";
import { useNavigate } from "react-router-dom";

const ProjectsPage = () => {
  const [projects, setProjects] = useState<ProjectResponse[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    const getProjects = async () => {
      const response = await Project.getAll();
      if (response.success) {
        setProjects(response.data ?? []);
      }
    };
    getProjects();
  }, []);

  return (
    <Box className={styles.homePage}>
      <Menu activeView="projects" />
      <Box className={styles.content}>
        <Box className={styles.contentBox}>
          <Typography variant="h6" sx={{ fontWeight: 'bold', textAlign: 'start' }}>All Projects</Typography>
          <Box className={styles.projectList}>
            {projects.map((project) => (
              <div style={{ height: '150px' }}>
                <Box className={styles.projectCard}>
                  <Box className={styles.projectName}>
                    <Typography variant="h6"
                      sx={{
                        width: '100%',
                        fontWeight: 'bold',
                        textAlign: 'center',
                        whiteSpace: 'nowrap',
                        overflow: 'hidden',
                        textOverflow: 'ellipsis',
                      }}>{project.name}</Typography>
                  </Box>
                  <Box className={styles.userBox}>
                    {project.users.map((user) => (
                      <Box className={styles.userCard}>
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
                          }}>{user.firstName} {user.lastName}</Typography>
                      </Box>
                    ))}
                  </Box>
                  <Button
                  onClick={() => {navigate(`/project/${project.id}`)}}
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
            ))}
          </Box>
        </Box>
      </Box>
    </Box>
  );
};

export default ProjectsPage;
