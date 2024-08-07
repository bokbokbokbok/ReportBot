import styled from "@emotion/styled";
import {
    TableCell,
    tableCellClasses,
    TableRow,
    TableContainer,
    Paper,
    Table,
    TableHead,
    TableBody,
    TablePagination,
    Skeleton,
    Box,
    TextField,
    MenuItem,
} from "@mui/material";
import { useEffect, useState } from "react";
import UserResponse from "../../api/models/response/UserResponse";
import User from "../../api/User";

const StyledTableCell = styled(TableCell)(({ theme }) => ({
    [`&.${tableCellClasses.head}`]: {
        backgroundColor: '#D9D9D9',
        color: '#000000',
    },
    [`&.${tableCellClasses.body}`]: {
        fontSize: 14,
    },
}));

const StyledTableRow = styled(TableRow)(({ theme }) => ({
    '&:nth-of-type(odd)': {
        backgroundColor: '#F3F3F3',
    },
    '&:last-child td, &:last-child th': {
        border: 0,
    },
}));

const UserTable = () => {
    const [users, setUsers] = useState<UserResponse[]>([]);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(7);
    const [loading, setLoading] = useState(true);
    const [sorting, setSorting] = useState(0);
    useEffect(() => {
        const getUsers = async () => {
            setLoading(true);
            const response = await User.getAll(sorting);
            if (response.success) {
                setUsers(response.data ?? []);
            }
            setLoading(false);
        };
        getUsers();
    }, [sorting]);


    const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const paginatedUsers = users.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

    return (
        <Box sx={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            width: '90%',
            gap: '20px',
        }}>
            <TextField
                id="outlined-select-currency"
                select
                defaultValue="0"
                onChange={(e) => {setSorting(parseInt(e.target.value))}}
                sx={{
                    display: 'flex',
                    alignSelf: 'flex-start',
                    width: '200px',
                    height: '50px',
                    '& .MuiInputBase-root': {
                        backgroundColor: '#E1F3E0',
                        borderRadius: '15px',
                        height: '50px',
                    },
                }}>
                <MenuItem value="0">
                    None
                </MenuItem>
                <MenuItem value="1">
                    FirstName
                </MenuItem>
                <MenuItem value="2">
                    LastName
                </MenuItem>
                <MenuItem value="3">
                    Username
                </MenuItem>
                <MenuItem value="4">
                    Email
                </MenuItem>
            </TextField>
            <TableContainer component={Paper} sx={{ width: '100%', borderRadius: '10px 10px 0 0', }}>
                <Table sx={{ width: '100%' }} aria-label="customized table">
                    <TableHead>
                        <TableRow>
                            <StyledTableCell>Username</StyledTableCell>
                            <StyledTableCell align="right">FirstName</StyledTableCell>
                            <StyledTableCell align="right">LastName</StyledTableCell>
                            <StyledTableCell align="right">Email</StyledTableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {loading
                            ? [0, 1, 2, 3, 4, 5, 6].map((index) => (
                                <StyledTableRow key={index}>
                                    <StyledTableCell>
                                        <Skeleton variant="text" />
                                    </StyledTableCell>
                                    <StyledTableCell align="right">
                                        <Skeleton variant="text" />
                                    </StyledTableCell>
                                    <StyledTableCell align="right">
                                        <Skeleton variant="text" />
                                    </StyledTableCell>
                                    <StyledTableCell align="right">
                                        <Skeleton variant="text" />
                                    </StyledTableCell>
                                </StyledTableRow>
                            ))
                            : paginatedUsers.map((user) => (
                                <StyledTableRow key={user.id}>
                                    <StyledTableCell component="th" scope="row">
                                        {user.login}
                                    </StyledTableCell>
                                    <StyledTableCell align="right">{user.firstName}</StyledTableCell>
                                    <StyledTableCell align="right">{user.lastName}</StyledTableCell>
                                    <StyledTableCell align="right">{user.email}</StyledTableCell>
                                </StyledTableRow>
                            ))}
                    </TableBody>
                </Table>
                <TablePagination
                    rowsPerPageOptions={[7]}
                    component="div"
                    count={users.length}
                    rowsPerPage={rowsPerPage}
                    page={page}
                    onPageChange={handleChangePage}
                    onRowsPerPageChange={handleChangeRowsPerPage}
                />
            </TableContainer>
        </Box>
    );
};

export default UserTable;
