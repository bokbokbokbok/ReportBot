import { Box, Button, Typography } from "@mui/material";
import styles from "./StatisticsContainer.module.css";

const StatisticsContainer = () => {
    const today = new Date();

    return (
        <Box className={styles.statisticsContainer}>
            <Box className={styles.topStatistics}>
                <Box className={styles.reportStatistics}>
                    <Typography sx={{ fontSize: '16px', fontWeight: 'bold', margin: '10px 20px 0' }}>Today's Reports {today.toDateString()}</Typography>
                    <Box className={styles.reportStatisticsBox}>
                        <Box className={styles.totalSessionBox}>
                            <Typography sx={{ fontSize: '18px', fontWeight: 'bold' }}>Total Sessions</Typography>
                            <Typography sx={{ fontSize: '12px' }}>This section provides the total number of sessions.</Typography>
                            <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>8</Typography>
                        </Box>
                        <Box className={styles.secondStatistics}>
                            <Box className={styles.secondStatisticsBox}>
                                <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Closed Sessions</Typography>
                                <Typography sx={{ fontSize: '10px' }}>This section contains details about the sessions that have been closed.</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>8</Typography>
                            </Box>
                            <Box className={styles.secondStatisticsBox}>
                                <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Opened Sessions</Typography>
                                <Typography sx={{ fontSize: '10px' }}>This section contains details about the sessions that are currently open.</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>8</Typography>
                            </Box>
                        </Box>
                        <Box className={styles.bottomReportStatistics}>
                            <Box className={styles.bottomReportStatisticsBox}>
                                <Typography sx={{ fontSize: '12px', fontWeight: 'bold' }}>Employees Worked</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>8</Typography>
                                <Typography sx={{ fontSize: '10px', textAlign: 'center' }}>employees who worked today</Typography>
                            </Box>
                            <Box className={styles.bottomReportStatisticsBox}>
                                <Typography sx={{ fontSize: '12px', fontWeight: 'bold' }}>Projects Worked On</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>8</Typography>
                                <Typography sx={{ fontSize: '10px', textAlign: 'center' }}>projects that were worked on today</Typography>
                            </Box>
                            <Box className={styles.bottomReportStatisticsBox}>
                                <Typography sx={{ fontSize: '12px', fontWeight: 'bold' }}>Employees Worked</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>8</Typography>
                                <Typography sx={{ fontSize: '10px', textAlign: 'center' }}>employees who worked today</Typography>
                            </Box>
                        </Box>
                    </Box>
                </Box>
                <Box className={styles.projectsContainer}>
                    <Typography sx={{ fontSize: '16px', fontWeight: 'bold', margin: '10px 20px 0' }}>List of projects</Typography>
                    <Box className={styles.projectsBox}>
                        <Box className={styles.projectsList}>
                            <Box className={styles.projectBox}>
                                <Box className={styles.projectTitle}>
                                    <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>MCG_DM</Typography>
                                    <Typography sx={{ fontSize: '12px' }}>
                                        1000 minutes per week +{' '}
                                        <Typography component="span" sx={{ fontSize: '12px' }} color="primary">
                                            400 per day
                                        </Typography>
                                    </Typography>
                                </Box>
                                <Button variant="contained" sx={{
                                    width: '100px',
                                    height: '35px',
                                    borderRadius: '10px',
                                    color: 'white',
                                    fontSize: '12px',
                                    textTransform: 'none',
                                }}>View</Button>

                            </Box>
                        </Box>
                        <Button variant="contained" sx={{
                            margin: '20px',
                            width: '100px',
                            height: '35px',
                            borderRadius: '10px',
                            color: 'white',
                            fontSize: '12px',
                            textTransform: 'none',
                        }}>View</Button>
                    </Box>
                </Box>
            </Box>

            <Box className={styles.bottomStatistics}>
                <Typography sx={{ fontSize: '16px', fontWeight: 'bold', margin: '10px 20px 0' }}>Report statistics</Typography>
                <Box className={styles.bottomStatisticsContainer}>
                    <Box className={styles.bottomStatisticsBox}>
                        <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Daily reports</Typography>
                        <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>1000</Typography>
                        <Typography sx={{ fontSize: '16px', textAlign: 'center' }}>minutes in 3 projects</Typography>
                    </Box>
                    <Box className={styles.bottomStatisticsBox}>
                        <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Weekly reports</Typography>
                        <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>1000</Typography>
                        <Typography sx={{ fontSize: '16px', textAlign: 'center' }}>minutes in 3 projects</Typography>
                    </Box>
                    <Box className={styles.bottomStatisticsBox}>
                        <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Monthly reports</Typography>
                        <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>5500</Typography>
                        <Typography sx={{ fontSize: '16px', textAlign: 'center' }}>minutes in 5 projects</Typography>
                    </Box>

                </Box>
            </Box>

        </Box>
    );
};

export default StatisticsContainer;