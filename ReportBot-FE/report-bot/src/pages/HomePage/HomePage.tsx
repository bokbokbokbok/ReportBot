import { Box } from "@mui/material";
import styles from "./HomePage.module.css";
import Menu from "../../components/Menu/Menu";
import StatisticsContainer from "../../components/StatisticsContainer/StatisticsContainer";

const HomePage = () => {
    return (
        <Box className={styles.homePage}>

            <Menu activeView="home" />
            <Box className={styles.content}>
                <StatisticsContainer />
            </Box>
        </Box>
    );
};

export default HomePage;