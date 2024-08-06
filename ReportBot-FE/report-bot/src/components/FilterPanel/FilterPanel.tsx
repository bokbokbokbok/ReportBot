import { Box, MenuItem, TextField } from "@mui/material";
import styles from "./FilterPanel.module.css";
import { DateField } from '@mui/x-date-pickers/DateField';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import dayjs, { Dayjs } from 'dayjs';
import { useEffect, useState } from "react";
import InputAdornment from '@mui/material/InputAdornment';

const FilterPanel = () => {
    const [fromDate, setFromDate] = useState<Dayjs | null>(dayjs(Date.now()));
    const [toDate, setToDate] = useState<Dayjs | null>(dayjs(Date.now()));
    const [selectedProject, setSelectedProject] = useState<string>('1');
    const [selectedUser, setSelectedUser] = useState<string>('1');

    useEffect(() => {
        console.log(fromDate?.toDate());
        console.log(toDate?.toDate());
        console.log(selectedProject);
        console.log(selectedUser);
    }, [fromDate, toDate, selectedProject, selectedUser]);

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
                    <MenuItem value="2">
                        Pfesasadad
                    </MenuItem>
                </TextField>
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
                    <MenuItem value="2">
                        Pfesasadad
                    </MenuItem>
                </TextField>
            </Box>
        </LocalizationProvider>
    );
};

export default FilterPanel;
