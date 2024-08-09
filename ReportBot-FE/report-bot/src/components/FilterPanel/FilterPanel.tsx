import { Box, MenuItem, TextField } from "@mui/material";
import styles from "./FilterPanel.module.css";
import { DateField } from '@mui/x-date-pickers/DateField';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs, { Dayjs } from 'dayjs';
import { useEffect, useState } from "react";
import InputAdornment from '@mui/material/InputAdornment';
import ProjectResponse from "../../api/models/response/ProjectResponse";
import Project from "../../api/Project";
import User from "../../api/User";
import UserResponse from "../../api/models/response/UserResponse";
import FilterRequest from "../../api/models/request/FilterRequest";
import Reports from "../../api/Reports";

interface FilterProps {
    getFilterReports: (request: FilterRequest) => Promise<void>;
    projectName: string | null;
}

const FilterPanel: React.FC<FilterProps> = ({ getFilterReports, projectName }) => {
    const [fromDate, setFromDate] = useState<Dayjs | null>(null);
    const [toDate, setToDate] = useState<Dayjs | null>(null);
    const [selectedProject, setSelectedProject] = useState<string>('1');
    const [selectedUser, setSelectedUser] = useState<string>('1');
    const [projects, setProjects] = useState<ProjectResponse[]>([]);
    const [users, setUsers] = useState<UserResponse[]>([]);

    useEffect(() => {
        const getReports = async () => {
            const request: FilterRequest = {
                projectName: selectedProject === '1' ? '' : selectedProject,
                fromDate: fromDate?.format('YYYY-MM-DD') ?? '',
                toDate: toDate?.format('YYYY-MM-DD') ?? '',
                userName: selectedUser === '1' ? '' : selectedUser,
            };
            getFilterReports(request);
        }
        getReports();
    }, [fromDate, toDate, selectedProject, selectedUser]);

    useEffect(() => {
        const getProjects = async () => {
            const response = await Project.getAll();
            if (response.success) {
                setProjects(response.data ?? []);
            }
        }
        const getUsers = async () => {
            const response = await User.getAll(0);
            if (response.success) {
                setUsers(response.data ?? []);
            }
        }

        getUsers();
        getProjects();
    }, []);

    return (
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <Box className={styles.filterBox}>
                <DateField
                    value={fromDate}
                    onChange={(newValue) => setFromDate(newValue)}
                    InputProps={{
                        startAdornment: (
                            <InputAdornment position="start">
                                From:
                            </InputAdornment>
                        ),
                    }}
                    sx={{
                        '& .MuiInputBase-root': {
                            backgroundColor: '#E1F3E0',
                            borderRadius: '15px',
                        },
                    }}
                />
                <DateField
                    value={toDate}
                    onChange={(newValue) => setToDate(newValue)}
                    InputProps={{
                        startAdornment: (
                            <InputAdornment position="start">
                                To:
                            </InputAdornment>
                        ),
                    }}
                    sx={{
                        '& .MuiInputBase-root': {
                            backgroundColor: '#E1F3E0',
                            borderRadius: '15px',
                        },
                    }}
                />
                {projectName != null ? null : (
                <TextField
                    id="outlined-select-currency"
                    select
                    defaultValue="1"
                    onChange={(newValue) => setSelectedProject(newValue.target.value)}
                    sx={{
                        width: '200px',
                        '& .MuiInputBase-root': {
                            backgroundColor: '#E1F3E0',
                            borderRadius: '15px',
                        },
                    }}
                >
                    <MenuItem value="1">
                        Projects
                    </MenuItem>
                    {projects.map((project) => (
                        <MenuItem key={project.id} value={project.name}>
                            {project.name}
                        </MenuItem>
                    ))}
                </TextField>
                )}
                <TextField
                    id="outlined-select-currency"
                    select
                    defaultValue="1"
                    onChange={(newValue) => setSelectedUser(newValue.target.value)}
                    sx={{
                        width: '200px',
                        '& .MuiInputBase-root': {
                            backgroundColor: '#E1F3E0',
                            borderRadius: '15px',
                        },
                    }}
                >
                    <MenuItem value="1">
                        Users
                    </MenuItem>
                    {users.map((user) => (
                        <MenuItem key={user.id} value={user.login}>
                            {user.firstName} {user.lastName}
                        </MenuItem>
                    ))}
                </TextField>
            </Box>
        </LocalizationProvider>
    );
};

export default FilterPanel;
