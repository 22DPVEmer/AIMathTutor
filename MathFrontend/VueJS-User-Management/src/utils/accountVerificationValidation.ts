import { required, minLength, maxLength, validEmail, validate } from './validators';

export function validateAccountVerificationForm({ email, code }) {
    return {
        email: validate(email, [required, validEmail, value => maxLength(value, 128)]),
        code: validate(code, [required, value => minLength(value, 6), value => maxLength(value, 10)]),
    };
}