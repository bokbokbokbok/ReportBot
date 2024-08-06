import { useNavigate } from "react-router-dom";
import styles from './Menu.module.css';
import DashboardIcon from '@mui/icons-material/Dashboard';
import ChatBubbleOutlineOutlinedIcon from '@mui/icons-material/ChatBubbleOutlineOutlined';
import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import FolderIcon from '@mui/icons-material/Folder';
import ExitToAppIcon from '@mui/icons-material/ExitToApp';
import { Box, Typography } from "@mui/material";

const Menu = (props: { activeView: string }) => {
    const navigate = useNavigate();

    const handleSignOut = () => {
        localStorage.clear();
        navigate('/');
    };
    return (
        <Box className={styles.menuBox}>
            <Box className={styles.buttonBox} >
            <Typography sx={{ fontSize: '18px', fontWeight: 'bold', margin: '20px 0 10px 10px'}}>Manager Dashboard</Typography>
                <Box
                    className={styles.button}
                    sx={{ backgroundColor: props.activeView === 'home' ? '#E1F3E0' : 'transparent' }}
                    onClick={() => navigate('/home')}>
                    <DashboardIcon sx={{ width: '25px', height: '25px' }} />
                    <Typography sx={{ fontSize: '18px' }}>Home</Typography>
                </Box>
                <Box
                    className={styles.button}
                    sx={{ backgroundColor: props.activeView === 'users' ? '#E1F3E0' : 'transparent' }}
                    onClick={() => navigate('/users')}
                >
                    <PeopleAltIcon sx={{ width: '25px', height: '25px' }} />
                    <Typography sx={{ fontSize: '18px' }}>Users</Typography>
                </Box>
                <Box
                    className={styles.button}
                    sx={{ backgroundColor: props.activeView === 'reports' ? '#E1F3E0' : 'transparent' }}
                    onClick={() => navigate('/reports')}
                >
                    <ChatBubbleOutlineOutlinedIcon sx={{ width: '25px', height: '25px' }} />
                    <Typography sx={{ fontSize: '18px' }}>Reports</Typography>
                </Box>

                <Box
                    className={styles.button}
                    sx={{ backgroundColor: props.activeView === 'projects' ? '#E1F3E0' : 'transparent' }}
                    onClick={() => navigate('/projects')}
                >
                    <FolderIcon sx={{ width: '25px', height: '25px' }} />
                    <Typography sx={{ fontSize: '18px' }}>Projects</Typography>
                </Box>
            </Box>
            <Box className={styles.settingsBox}>
                <Box className={styles.line} />
                <Box
                    className={styles.button}
                    onClick={() => handleSignOut()}
                >
                    <ExitToAppIcon sx={{ width: '25px', height: '25px' }} />
                    <Typography sx={{ fontSize: '18px' }}>Sign out</Typography>
                </Box>
            </Box>
        </Box>
    );
};

export default Menu;