import { Box, Button, CircularProgress, Skeleton, Typography } from "@mui/material";
import styles from "./StatisticsContainer.module.css";
import { useEffect, useState } from "react";
import ProjectStatisticsResponse from "../../api/models/response/ProjectStatisticsResponse";
import Statistics from "../../api/Statistics";
import { useNavigate } from "react-router-dom";
import ReportStatisticsResponse from "../../api/models/response/ReportStatisticsResponse";
import SessionStatisticsResponse from "../../api/models/response/SessionStatisticsResponse";

const StatisticsContainer = () => {
    const [projectStatistics, setProjectStatistics] = useState<ProjectStatisticsResponse[] | undefined>([]);
    const [sessionStatistics, setSessionStatistics] = useState<SessionStatisticsResponse | undefined>();
    const [reportStatistics, setReportStatistics] = useState<Record<string, ReportStatisticsResponse> | undefined>();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const today = new Date();

    
    useEffect(() => {
        const getProjectStatistics = async () => {
            const response = await Statistics.projectStatistics();
            if (response.success) {
                setProjectStatistics(response.data);
            }
            setLoading(false);
        };
    
        const getSessionStatistics = async () => {
            const response = await Statistics.sessionStatistics();
            if (response.success) {
                setSessionStatistics(response.data);
            }
        };
    
        const getReportStatistics = async () => {
            const response = await Statistics.reportStatistics();
            if (response.success) {
                setReportStatistics(response.data);
            }
        };
        
        getProjectStatistics();
        getSessionStatistics();
        getReportStatistics();
    }, []);

    return (
        <>
        {loading ? (
            <Box className={styles.statisticsContainer}>
            <CircularProgress />
            </Box>
        )
        : (
        <Box className={styles.statisticsContainer}>
            <Box className={styles.topStatistics}>
                <Box className={styles.reportStatistics}>
                    <Typography sx={{ fontSize: '16px', fontWeight: 'bold', margin: '10px 20px 0' }}>Today's Reports {today.toDateString()}</Typography>
                    <Box className={styles.reportStatisticsBox}>
                        <Box className={styles.totalSessionBox}>
                            <Typography sx={{ fontSize: '18px', fontWeight: 'bold' }}>Total Sessions</Typography>
                            <Typography sx={{ fontSize: '12px' }}>This section provides the total number of sessions.</Typography>
                            <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>{sessionStatistics?.totalSessions ?? 0}</Typography>
                        </Box>
                        <Box className={styles.secondStatistics}>
                            <Box className={styles.secondStatisticsBox}>
                                <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Closed Sessions</Typography>
                                <Typography sx={{ fontSize: '10px' }}>This section contains details about the sessions that have been closed.</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>{sessionStatistics?.closedSessions ?? 0}</Typography>
                            </Box>
                            <Box className={styles.secondStatisticsBox}>
                                <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Opened Sessions</Typography>
                                <Typography sx={{ fontSize: '10px' }}>This section contains details about the sessions that are currently open.</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>{sessionStatistics?.openedSessions ?? 0}</Typography>
                            </Box>
                        </Box>
                        <Box className={styles.bottomReportStatistics}>
                            <Box className={styles.bottomReportStatisticsBox}>
                                <Typography sx={{ fontSize: '12px', fontWeight: 'bold' }}>Employees Worked</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>{sessionStatistics?.employeesCount ?? 0}</Typography>
                                <Typography sx={{ fontSize: '10px', textAlign: 'center' }}>employees who worked today</Typography>
                            </Box>
                            <Box className={styles.bottomReportStatisticsBox}>
                                <Typography sx={{ fontSize: '12px', fontWeight: 'bold' }}>Projects Worked On</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>{sessionStatistics?.projectsCount ?? 0}</Typography>
                                <Typography sx={{ fontSize: '10px', textAlign: 'center' }}>projects that were worked on today</Typography>
                            </Box>
                            <Box className={styles.bottomReportStatisticsBox}>
                                <Typography sx={{ fontSize: '12px', fontWeight: 'bold' }}>Reports Received</Typography>
                                <Typography sx={{ fontSize: '20px', textAlign: 'center', fontWeight: 'bold' }}>{sessionStatistics?.reportsCount ?? 0}</Typography>
                                <Typography sx={{ fontSize: '10px', textAlign: 'center' }}>reports received</Typography>
                            </Box>
                        </Box>
                    </Box>
                </Box>
                <Box className={styles.projectsContainer}>
                    <Typography sx={{ fontSize: '16px', fontWeight: 'bold', margin: '10px 20px 0' }}>List of projects</Typography>
                    <Box className={styles.projectsBox}>
                        <Box className={styles.projectsList}>
                            {loading ? (
                                Array.from(new Array(4)).map((_, index) => (
                                    <Box key={index} className={styles.projectBox}>
                                        <Skeleton variant="text" width={'100%'} height={30} />
                                    </Box>
                                ))
                            ) : (
                                projectStatistics?.map((project, index) => (
                                    <Box key={index} className={styles.projectBox}>
                                        <Box className={styles.projectTitle}>
                                            <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>
                                                {project.project.name}
                                            </Typography>
                                            <Typography sx={{ fontSize: '12px' }}>
                                                {project.totalMinutesPerWeek ?? 0} minutes per week +{' '}
                                                <Typography component="span" sx={{ fontSize: '12px' }} color="primary">
                                                    {project.totalMinutesPerDay ?? 0} per day
                                                </Typography>
                                            </Typography>
                                        </Box>
                                    </Box>
                                ))
                            )}
                        </Box>
                        {!loading && (
                            <Button
                                onClick={() => { navigate('/projects') }}
                                variant="contained"
                                sx={{
                                    margin: '20px',
                                    width: '100px',
                                    height: '35px',
                                    borderRadius: '10px',
                                    color: 'white',
                                    fontSize: '12px',
                                    textTransform: 'none',
                                }}
                            >
                                View
                            </Button>
                        )}
                    </Box>
                </Box>
            </Box>
            <Box className={styles.bottomStatistics}>
                <Typography sx={{ fontSize: '16px', fontWeight: 'bold', margin: '10px 20px 0' }}>Report statistics</Typography>
                <Box className={styles.bottomStatisticsContainer}>
                    <Box className={styles.bottomStatisticsBox}>
                        <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Daily reports</Typography>
                        <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>{reportStatistics?.daily.totalReportsMinutes ?? 0}</Typography>
                        <Typography sx={{ fontSize: '16px', textAlign: 'center' }}>minutes in {reportStatistics?.daily.totalProjects ?? 0} projects</Typography>
                    </Box>
                    <Box className={styles.bottomStatisticsBox}>
                        <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Weekly reports</Typography>
                        <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>{reportStatistics?.weekly.totalReportsMinutes ?? 0}</Typography>
                        <Typography sx={{ fontSize: '16px', textAlign: 'center' }}>minutes in {reportStatistics?.weekly.totalProjects ?? 0} projects</Typography>
                    </Box>
                    <Box className={styles.bottomStatisticsBox}>
                        <Typography sx={{ fontSize: '16px', fontWeight: 'bold' }}>Monthly reports</Typography>
                        <Typography sx={{ fontSize: '20px', fontWeight: 'bold', textAlign: 'center' }}>{reportStatistics?.monthly.totalReportsMinutes ?? 0}</Typography>
                        <Typography sx={{ fontSize: '16px', textAlign: 'center' }}>minutes in {reportStatistics?.monthly.totalProjects} projects</Typography>
                    </Box>
                </Box>
            </Box>
        </Box>
                    )}
                    </>
    );
};

export default StatisticsContainer;