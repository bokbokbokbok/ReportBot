import { Box, Button, Typography } from "@mui/material";
import Menu from "../../components/Menu/Menu";
import styles from "./ReportsPage.module.css";
import FilterPanel from "../../components/FilterPanel/FilterPanel";
import Field from "../../components/Field/Field";
import { useState } from "react";

const ReportsPage = () => {
  const statistics: string[] = [
    "Total Users",
    "Total Projects",
    "Total Tasks",
    "Total Time Spent",
    "Total Time Saved",
  ];

  // Initialize an array of booleans to track expanded state for each item
  const [expandedStates, setExpandedStates] = useState<boolean[]>(
    new Array(statistics.length).fill(false)
  );

  const handleToggle = (index: number) => {
    // Toggle the specific index's expanded state
    setExpandedStates((prevStates) =>
      prevStates.map((state, i) => (i === index ? !state : state))
    );
  };

  return (
    <Box className={styles.reportsPage}>
      <Menu activeView="reports" />
      <Box className={styles.content}>
        <FilterPanel />

        <Box className={styles.reportsContainer}>
          <Box className={styles.reportsBox}>
            {statistics.map((stat, index) => (
              <Box
                key={index}
                className={styles.reportCart}
                sx={{
                  maxHeight: expandedStates[index] ? "300px" : "250px", // Use maxHeight for smoother transition
                  transition: "max-height 0.5s ease-in-out", // Smooth transition for max-height
                  overflow: "hidden",
                }}
              >
                <Box
                  sx={{
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "center",
                    alignItems: "center",
                    width: "25%",
                    gap: "10px",
                  }}
                >
                  <Field label="Project" content="0" />
                  <Field label="User" content="0" />
                  <Field label="Date" content="0" />
                  <Field label="Time" content="0" />
                </Box>
                <Box
                  sx={{
                    display: "flex",
                    flexDirection: "column",
                    justifyContent: "space-between",
                    alignItems: "center",
                    width: "60%",
                    backgroundColor: "#F5F5F5",
                    height: expandedStates[index] ? "280px" : "90%", 
                    borderRadius: "10px",
                    padding: "10px",
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
                      transition: "all 0.5s ease-in-out", // Smooth transition
                      overflowY: expandedStates[index] ? "auto" : "hidden",
                    '&::-webkit-scrollbar': expandedStates[index]
                      ? { display: "none" } // Hides the scrollbar in WebKit browsers when expanded
                      : {},
                    "-ms-overflow-style": expandedStates[index] ? "none" : "auto", // Hides scrollbar in IE and Edge when expanded
                    scrollbarWidth: expandedStates[index] ? "none" : "auto", 
                    }}
                  >
                    Працював sdf dfssss dfsdfsd sdfffffffsdf sdfsdfsfsf
                    sdfsdfsdfsd sdfsdfsdf sdfsdfsdfнад проектами. Зробив щоб
                    для конкретного юзера проекти підтягувались з worksnaps.
                    Також зробив зв'язки між проектами та юзерами. dsfdf dfsdfds
                    dsfsdfd sdfsdfd sdfsdfds Працював sdf dfssss dfsdfsd
                    sdfffffffsdf sdfsdfsfsf sdfsdfsdfsd sdfsdfsdf sdfsdfsdfнад
                    проектами. Зробив щоб для конкретного юзера проекти
                    підтягувались з worksnaps. Також зробив зв'язки між
                    проектами та юзерами. dsfdf dfsdfds dsfsdfd sdfsdfd sdfsdfds
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
            ))}
          </Box>
        </Box>
      </Box>
    </Box>
  );
};

export default ReportsPage;
