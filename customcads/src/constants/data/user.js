export default {
    username: {
        isRequired: true,
        maxLength: 62,
        minLength: 3,
    },
    email: {
        isRequired: true,
        regex: /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/,
        maxLength: 320,
        minLength: 3,
    },
    password: {
        isRequired: true,
        maxLength: 100,
        minLength: 6,
    },
    confirmPassword: {
        isRequired: true,
    }
};