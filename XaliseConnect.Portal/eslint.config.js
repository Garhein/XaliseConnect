import globals from 'globals';
import eslint from '@eslint/js';

export default [
    {
        files: ['wwwroot/js/**/*.js'],
        languageOptions: {
            ecmaVersion: 'latest',
            sourceType: 'module',
            globals: {
                ...globals.browser,
                bootstrap: 'readonly',
            },
        },
        rules: {
            ...eslint.configs.recommended.rules,
        },
    },
];
