import { required, minLength, maxLength, validEmail, validate } from './validators';

export function validateRegisterForm({ firstname, lastname, email, password }) {
    return {
        firstname: validate(firstname, [required, value => minLength(value, 3), value => maxLength(value, 64)]),
        lastname: validate(lastname, [required, value => minLength(value, 3), value => maxLength(value, 64)]),
        email: validate(email, [required, validEmail, value => maxLength(value, 128)]),
        password: validate(password, [required, value => minLength(value, 8), value => maxLength(value, 128)]),
    };
}