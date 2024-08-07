import { Box } from '@mui/material';
import styles from './UserPage.module.css';
import Menu from '../../components/Menu/Menu';
import UserTable from '../../components/UserTable/UserTable';

const UserPage = () => {
    return (
        <Box className={styles.homePage}>
            <Menu activeView="users" />
            <Box className={styles.content}>
                <UserTable />
            </Box>
        </Box>
    );
}

export default UserPage;