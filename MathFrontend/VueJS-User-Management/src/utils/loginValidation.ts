import { required, minLength, maxLength, validEmail, validate } from './validators';

export function validateLoginForm({ email, password }) {
    return {
        email: validate(email, [required, validEmail, value => maxLength(value, 128)]),
        password: validate(password, [required, value => minLength(value, 6), value => maxLength(value, 128)]),
    };
}