import { Box, Button, TextField, Typography } from "@mui/material";
import styles from "./SignInPage.module.css";
import DescriptionIcon from '@mui/icons-material/Description';
import { useForm } from "react-hook-form"
import { yupResolver } from '@hookform/resolvers/yup';
import signInFormValidation from "../../validation/SignInFormValidation";
import Auth from "../../api/Auth";
import AuthRequest from "../../api/models/request/AuthRequest";
import { useNavigate } from "react-router-dom";
import useNotification from "../../hooks/useNotification";
import { useState } from "react";
import LoadingButton from '@mui/lab/LoadingButton';

export interface SignIn {
  email: string;
}

const SignInPage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const { notifyError, Notification } = useNotification();
  const {
    register,
    handleSubmit,
    formState: { errors }
  } = useForm<SignIn>({
    resolver: yupResolver(signInFormValidation),
    reValidateMode: 'onChange',
    mode: 'onTouched'
  });

  const handleSignIn = async (data: SignIn) => {
    setLoading(true);
    const response = await Auth.signIn(data as AuthRequest);

    if (response.success) {
      navigate(`/home`);
    }
    else {
      notifyError("Sign in failed");
      setLoading(false);
    }
  };


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
              id="email"
              label="Email"
              {...register('email')}
              error={!!errors.email}
              helperText={errors.email?.message || ' '}
              sx={{
                width: '90%',
                '& .MuiOutlinedInput-root': {
                  backgroundColor: '#f0f0f0',
                },
              }} size="small" variant="outlined" />
            <LoadingButton
              variant="contained"
              loading={loading}
              onClick={handleSubmit(handleSignIn)}
              sx={{ width: '90%', borderRadius: '10px', color: '#ffffff', textTransform: 'none' }}>Sign In</LoadingButton>
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