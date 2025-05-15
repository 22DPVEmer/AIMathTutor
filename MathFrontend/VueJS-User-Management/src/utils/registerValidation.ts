import { required, minLength, maxLength, validEmail, validate } from './validators';

interface RegisterFormData {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
}

export function validateRegisterForm({ firstName, lastName, email, password }: RegisterFormData) {
    return {
        firstName: validate(firstName, [(value: string) => required(value), (value: string) => minLength(value, 3), (value: string) => maxLength(value, 64)]),
        lastName: validate(lastName, [(value: string) => required(value), (value: string) => minLength(value, 3), (value: string) => maxLength(value, 64)]),
        email: validate(email, [(value: string) => required(value), (value: string) => validEmail(value), (value: string) => maxLength(value, 128)]),
        password: validate(password, [(value: string) => required(value), (value: string) => minLength(value, 8), (value: string) => maxLength(value, 128)]),
    };
}