import { Box, Button, Typography } from "@mui/material";
import Menu from "../../components/Menu/Menu";
import styles from "./ReportsPage.module.css";
import FilterPanel from "../../components/FilterPanel/FilterPanel";
import Field from "../../components/Field/Field";
import { useState } from "react";
import FilterRequest from "../../api/models/request/FilterRequest";
import Reports from "../../api/Reports";
import ReportResponse from "../../api/models/response/ReportResponse";

function splitDateTime(dateTime: Date) {
  const dateString = new Date(dateTime).toLocaleDateString("en", {
    year: "numeric",
    day: "2-digit",
    month: "short",
  });

  return dateString;
}

const ReportsPage = () => {
  const [reports, setReports] = useState<ReportResponse[]>([]);

  const [expandedStates, setExpandedStates] = useState<boolean[]>(
    new Array(reports.length).fill(false)
  );

  const getReports = async (request: FilterRequest) => {
    const response = await Reports.getAll(request);
    setReports(response.data ?? []);
    setExpandedStates(new Array(response.data?.length ?? 0).fill(false));
  };

  const handleToggle = (index: number) => {
    setExpandedStates((prevStates) =>
      prevStates.map((state, i) => (i === index ? !state : state))
    );
  };

  return (
    <Box className={styles.reportsPage}>
      <Menu activeView="reports" />
      <Box className={styles.content}>
        <FilterPanel getFilterReports={getReports} projectName={null} />
        <Box className={styles.reportsContainer}>
          <Box className={styles.reportsBox}>
            {reports.length === 0 ?
              (
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
              )
              : (reports.map((report, index) => (
                <Box
                  key={index}
                  className={styles.reportCart}
                  sx={{
                    height: expandedStates[index] ? "300px" : "250px",
                    maxHeight: expandedStates[index] ? "300px" : "250px",
                    transition: "max-height 0.5s ease-in-out",
                    overflow: "hidden",
                  }}
                >
                  <Box className={styles.fieldBox}>
                    <Field label="Project" content={report.project.name} />
                    <Field label="User" content={report.userName} />
                    <Field label="Date" content={splitDateTime(report.dateOfShift)} />
                    <Field label="Time" content={report.timeOfShift.toString()} />
                  </Box>
                  <Box
                    sx={{
                      display: "flex",
                      flexDirection: "column",
                      justifyContent: "space-between",
                      alignItems: "center",
                      width: "60%",
                      backgroundColor: "#F5F5F5",
                      maxHeight: expandedStates[index] ? "280px" : "90%",
                      height: expandedStates[index] ? "280px" : "90%",
                      borderRadius: "10px",
                      padding: "10px",
                      transition: "max-height 0.5s ease-in-out",
                      overflow: "hidden",
                    }}
                  >
                    <Typography
                      sx={{
                        width: "100%",
                        whiteSpace: "normal",
                        overflow: "hidden",
                        textOverflow: "ellipsis",
                        display: expandedStates[index] ? "block" : "-webkit-box",
                        WebkitLineClamp: expandedStates[index] ? "none" : 7,
                        WebkitBoxOrient: "vertical",
                        overflowY: expandedStates[index] ? "auto" : "hidden",
                        '&::-webkit-scrollbar': expandedStates[index]
                          ? { display: "none" }
                          : {},
                        "-ms-overflow-style": expandedStates[index] ? "none" : "auto",
                        scrollbarWidth: expandedStates[index] ? "none" : "auto",
                      }}
                    >
                      {report.message} sdfdfds dsff dsfsf sdfsdfd dsfsdfd sdfdfsd sfddf sdfsdf
                    </Typography>

                    <Button
                      variant="contained"
                      sx={{
                        width: "100px",
                        height: "30px",
                        borderRadius: "10px",
                        color: "white",
                        fontSize: "12px",
                        textTransform: "none",
                        marginTop: "10px",
                      }}
                      onClick={() => handleToggle(index)}
                    >
                      {expandedStates[index] ? "Collapse" : "View"}
                    </Button>
                  </Box>
                </Box>
              )))}
          </Box>
        </Box>
      </Box>
    </Box>
  );
};

export default ReportsPage;
