import { required, maxLength, validEmail, validate } from './validators';

export function validateForgotPasswordForm({ email }) {
    return {
        email: validate(email, [required, validEmail, value => maxLength(value, 128)]),
    };
}