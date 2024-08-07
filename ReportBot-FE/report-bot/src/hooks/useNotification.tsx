import React, { useState } from 'react';
import Snackbar from '@mui/material/Snackbar';
import Alert, { AlertColor } from '@mui/material/Alert';
import { useSnackbar } from 'notistack';

interface NotificationProps {
    open: boolean;
    message: string;
    severity: AlertColor;
    onClose: () => void;
}

const Notification: React.FC<NotificationProps> = ({ open, message, severity, onClose }) => (
    <Snackbar open={open} autoHideDuration={6000} onClose={onClose}>
        <Alert onClose={onClose} severity={severity} sx={{ width: '100%' }}>
            {message}
        </Alert>
    </Snackbar>
);

interface UseNotification {
    notifySuccess: (message: string) => void;
    notifyError: (message: string) => void;
    notifyInfo: (message: string) => void;
    notifyWarning: (message: string) => void;
    Notification: React.FC;
}

const useNotification = (): UseNotification => {
    const { enqueueSnackbar } = useSnackbar();
    const [open, setOpen] = useState(false);
    const [message, setMessage] = useState('');
    const [severity, setSeverity] = useState<AlertColor>('success');

    const handleClose = () => {
        setOpen(false);
    };

    const notifySuccess = (message: string) => {
        enqueueSnackbar(message, { variant: 'success' });
    };

    const notifyError = (message: string) => {
        enqueueSnackbar(message, { variant: 'error' });
    };

    const notifyInfo = (message: string) => {
        enqueueSnackbar(message, { variant: 'info' });
    };

    const notifyWarning = (message: string) => {
        enqueueSnackbar(message, { variant: 'warning' });
    };

    return {
        notifyWarning,
        notifyInfo,
        notifySuccess,
        notifyError,
        Notification: () => <Notification open={open} message={message} severity={severity} onClose={handleClose} />
    };
};

export default useNotification;