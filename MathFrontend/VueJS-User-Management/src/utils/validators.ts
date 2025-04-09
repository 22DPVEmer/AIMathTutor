// Required Field Validator
export function required(value) {
    return value ? null : 'This field is required.';
}

// Minimum Length Validator
export function minLength(value, length) {
    return value.length >= length ? null : `Minimum length is ${length} characters.`;
}

// Maximum Length Validator
export function maxLength(value, length) {
    return value.length <= length ? null : `Maximum length is ${length} characters.`;
}

// Valid Email Validator
export function validEmail(value) {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(value) ? null : 'Invalid email format.';
}

// Validation Logic
export function validate(value, validators) {
    for (let validator of validators) {
        const error = validator(value);
        if (error) return error; // Return the first error found
    }
    return null; // No errors
}
