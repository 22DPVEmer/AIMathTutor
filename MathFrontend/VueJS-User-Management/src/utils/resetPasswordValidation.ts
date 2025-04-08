import { required, minLength, maxLength, validate } from './validators';

export function validateResetPasswordForm({ reset_code, password }) {
    return {
        code: validate(reset_code, [required, value => minLength(value, 16), value => maxLength(value, 16)]),
        password: validate(password, [required, value => minLength(value, 8), value => maxLength(value, 128)]),
    };
}