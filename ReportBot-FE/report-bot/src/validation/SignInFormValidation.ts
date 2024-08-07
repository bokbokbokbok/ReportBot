import * as yup from 'yup';
import { SignIn } from '../pages/SignInPage/SignInPage';

const signInFormValidation = yup.object<SignIn>().shape({
    email: yup.string()
        .required('Email is required')
        .email('Email is invalid'),
});

export default signInFormValidation;