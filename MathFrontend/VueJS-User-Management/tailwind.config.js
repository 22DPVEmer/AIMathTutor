/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#f0f7ff',
          100: '#e0efff',
          200: '#b8dfff',
          300: '#78c6ff',
          400: '#37abff',
          500: '#217bff',
          600: '#1a62cc',
          700: '#154fa3',
          800: '#113d7a',
          900: '#0e2f5c',
        },
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
      },
      boxShadow: {
        'input': '0 1px 2px rgba(0, 0, 0, 0.06), 0 0 0 1px rgba(104, 113, 130, 0.16)',
        'input-focus': '0 0 0 2px rgba(62, 207, 142, 0.25), 0 1px 2px rgba(0, 0, 0, 0.06), 0 0 0 1px rgba(104, 113, 130, 0.16)',
      },
    },
  },
  plugins: [
    require('@tailwindcss/forms'),
  ],
} 