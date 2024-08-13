import { Box, Typography, Button, IconButton } from "@mui/material";
import styles from "./ProjectPage.module.css";
import Menu from "../../components/Menu/Menu";
import FilterPanel from "../../components/FilterPanel/FilterPanel";
import { useState, useRef, useEffect } from "react";
import FilterRequest from "../../api/models/request/FilterRequest";
import ReportResponse from "../../api/models/response/ReportResponse";
import Reports from "../../api/Reports";
import KeyboardArrowLeftIcon from '@mui/icons-material/KeyboardArrowLeft';
import KeyboardArrowRightIcon from '@mui/icons-material/KeyboardArrowRight';
import Field from "../../components/Field/Field";
import { useParams } from "react-router-dom";
import ProjectResponse from "../../api/models/response/ProjectResponse";
import Project from "../../api/Project";

function splitDateTime(dateTime: Date) {
    const dateString = new Date(dateTime).toLocaleDateString("en", {
        year: "numeric",
        day: "2-digit",
        month: "short",
    });

    return dateString;
}

const ProjectPage = () => {
    const id = useParams().id;
    const [project, setProject] = useState<ProjectResponse | undefined>();
    const [reports, setReports] = useState<ReportResponse[]>([]);
    const scrollContainerRef = useRef<HTMLDivElement>(null);

    const getReports = async (request: FilterRequest) => {
        const response = await Reports.getForProject(id ?? '', request);
        setReports(response.data ?? []);
    };

    const scrollLeft = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({ left: -920, behavior: "smooth" });
        }
    };

    const scrollRight = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({ left: 920, behavior: "smooth" });
        }
    };

    useEffect(() => {
        const getProjectById = async () => {
            const response = await Project.getById(id ?? "");
            setProject(response.data ?? undefined);
        };
        getProjectById();
    }, []);

    return (
        <Box className={styles.homePage}>
            <Menu activeView="projects" />
            <Box className={styles.content}>
                <Box className={styles.contentBox}>
                    <Box className={styles.titleBox}>
                        <Box className={styles.headerBox}>
                            <Box className={styles.projectStatus}
                                sx={{
                                    backgroundColor: project?.status === 'active' ? '#A7FEA3' : '#FF5C5C',
                                }}>
                                {project?.status}
                            </Box>
                            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>{project?.name}</Typography>
                        </Box>
                        <Box className={styles.userBox}>
                            <Box className={styles.userContainer}>
                                {project?.users.map((item, index) => (
                                    <Box className={styles.userTitle}>
                                        <Typography variant="h6" sx={{ fontWeight: 'bold' }}>{item.firstName} {item.lastName}</Typography>
                                    </Box>
                                ))}
                            </Box>
                        </Box>
                    </Box>
                    <Box className={styles.reportBox}>
                        <FilterPanel getFilterReports={getReports} projectName='Report Bot' />
                        <Box className={styles.reportContainer}>
                            <IconButton aria-label="delete" onClick={scrollLeft}>
                                <KeyboardArrowLeftIcon />
                            </IconButton>
                            <Box className={styles.reportList} ref={scrollContainerRef}>
                                {reports.length === 0 ? (
                                    <Box className={styles.notFoundBox}>
                                        <Typography sx={{
                                            fontWeight: 'bold',
                                            fontSize: '28px',
                                        }}>No reports found</Typography>
                                        <Typography sx={{
                                            textAlign: 'center',
                                            width: '270px',
                                            fontWeight: 'bold',
                                            fontSize: '13px',
                                        }}>Sorry, but no report was found for these filters.</Typography>
                                    </Box>
                                ) :
                                    reports.map((report, index) => (
                                        <Box
                                            key={index}
                                            ref={scrollContainerRef}
                                            className={styles.reportCard}>
                                            <Box className={styles.fieldBox}>
                                                <Field label="Project" content={report.project.name} />
                                                <Field label="User" content={report.userName} />
                                                <Field label="Date" content={splitDateTime(report.dateOfShift)} />
                                                <Field label="Time" content={report.timeOfShift.toString()} />
                                            </Box>
                                            <Box className={styles.textBox}
                                                sx={{ transition: "max-height 0.5s ease-in-out" }}>
                                                <Typography
                                                    sx={{
                                                        width: "100%",
                                                        whiteSpace: "normal",
                                                        overflow: "hidden",
                                                        textOverflow: "ellipsis",
                                                        display: "block",
                                                        WebkitLineClamp: "none",
                                                        WebkitBoxOrient: "vertical",
                                                        overflowY: "auto",
                                                        '&::-webkit-scrollbar': { display: "none" },
                                                        "-ms-overflow-style": "none",
                                                        scrollbarWidth: "none"
                                                    }}>
                                                    {report.message}
                                                </Typography>
                                            </Box>
                                        </Box>
                                    ))}
                            </Box>
                            <IconButton aria-label="delete" onClick={scrollRight}>
                                <KeyboardArrowRightIcon />
                            </IconButton>
                        </Box>
                    </Box>
                </Box>
            </Box>
        </Box>
    );
};

export default ProjectPage;