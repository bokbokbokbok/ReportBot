import { Box, Button, TextField, Typography } from "@mui/material";
import styles from "./SignInPage.module.css";
import DescriptionIcon from '@mui/icons-material/Description';

const SignInPage = () => {
  return (
    <Box className={styles.page}>
      <Box className={styles.formContainer}>
        <Box className={styles.form}>
          <Box className={styles.formHeader}>
            <Typography variant="h4">Welcome!</Typography>
            <Typography sx={{ fontSize: '15px' }}>You have successfully logged into the admin panel of the reporting bot.<br />
              Please enter your Worksnaps email to sign in.</Typography>
          </Box>
          <Box className={styles.fieldsBox}>
              <TextField 
             sx={{
              width: '90%',
              '& .MuiOutlinedInput-root': {
                backgroundColor: '#f0f0f0',
              },
            }} size="small" id="outlined-basic" label="Email" variant="outlined" />
              <Button variant="contained" sx={{ width: '90%', borderRadius: '10px', color: '#ffffff', textTransform: 'none' }}>Sign In</Button>
          </Box>
            <Box className={styles.formFooter}>
            <Typography sx={{ fontSize: '10px' }}>Your security is our priority. We employ advanced security measures to protect your account information and ensure your data remains confidential.</Typography>
            </Box>
        </Box>
      </Box>
      <Box className={styles.logoContainer} sx={{ background: 'linear-gradient(to bottom,#0AC602, #A7FEA3)' }}>
        <DescriptionIcon sx={{ fontSize: '60px' }} />
        <Typography variant="h4">Admin panel of the reporting bot</Typography>
      </Box>
    </Box>
  )
};

export default SignInPage;