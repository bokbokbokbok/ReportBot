import { Box } from "@mui/material";
import Menu from "../../components/Menu/Menu";
import styles from "./ProjectsPage.module.css";

const ProjectsPage = () => {
    return (
        <Box className={styles.homePage}>

        <Menu activeView="projects" />
        <Box className={styles.content}>
            <Box>
                project
            </Box>
        </Box>
    </Box>
    );
};

export default ProjectsPage;