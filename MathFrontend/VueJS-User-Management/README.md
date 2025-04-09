# User Management System

This project is a User Management System built using Vue.js, Vuex, and Vue-Router. It supports features such as user registration, account verification, login, password management (forgot/reset), and profile management. The project follows best practices for Vue.js applications with a modular structure, including centralized state management and dynamic form validation.

## Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Running the Project](#running-the-project)
- [Routing and Navigation](#routing-and-navigation)
- [License](#license)
- [Contact](#contact)

## Features

- **User Registration:** New users can register with their first name, last name, email, and password.
- **Account Verification:** Users receive a confirmation code via email to verify their account after registration.
- **Login/Logout:** Users can log in and out using their email and password.
- **Password Management:**
    - Forgot password: Users can request a password reset via email.
    - Reset password: Users can reset their password using the reset link.
- **User Profile:** Users can view their profile information.
- **Authentication Handling:** JWT token-based authentication with protected routes.
- **Form Validation:** All forms (register, login, password reset) are dynamically validated with custom validation functions.
- **Error Handling:** Detailed error handling for API responses (e.g., email already registered, server down, etc.).

## Technologies Used

- **Vue.js 3**: Frontend framework for building the user interface.
- **Vuex**: State management for handling global state (e.g., user data).
- **Vue-Router**: Client-side routing for navigating between views (login, register, profile, etc.).
- **Axios**: HTTP client for communicating with the backend API.
- **Bootstrap 5**: For responsive design and UI components.
- **JavaScript (ES6+)**: Core programming language used in the project.

## Project Structure

The project follows a modular structure to keep the code clean and maintainable.
  ````
    ├── public/
    ├── src/ 
    │ ├── api/ # API requests (e.g., register, login)
    │ ├── assets/ # Static assets (images, css, etc.) 
    │ ├── components/ # Reusable components (form inputs, side banners)
    │ ├── router/ # Vue Router configuration
    │ ├── store/ # Vuex store (modules, actions, mutations)
    │ ├── utils/ # Utility functions (validation, helpers)
    │ ├── views/ # Vue components (pages like Register, Login, Profile)
    │ ├── App.vue # Main App component
    │ └── main.js # Entry point for the Vue app
    ├── .env # Environment variables
    ├── package.json # Project dependencies and scripts
    └── README.md # Documentation
  ````

## Installation
1. Clone the repository:

   ```bash
   git clone https://github.com/Mahrnoud/VueJS-User-Management.git
   cd VueJS-User-Management
   
2. Install dependencies:
   ```bash
   npm install

## Running the Project
Start the development server:
   ```bash
   npm run dev
   ```

## Routing and Navigation
The project uses `Vue-Router` for navigation between views (e.g., login, register, profile). Some routes are protected, meaning users must be authenticated to access them. This is handled with route guards.

### Protected Routes
Routes like `/profile` require authentication. If a user is not logged in, they will be redirected to the `/login` page.

Example of protected route in router/index.js:
   ```bash
     {
        path: '/profile',
        name: 'Profile',
        component: Profile,
        meta: { requiresAuth: true },
     }
   ```

### Route Guard Example
A global route guard checks if the user is authenticated before allowing access to protected routes:
   ```bash
     router.beforeEach((to, from, next) => {
        if (to.matched.some(record => record.meta.requiresAuth)) {
          if (!store.getters['user/isAuthenticated']) {
            next({ name: 'Login' });
          } else {
            next();
          }
        } else {
          next();
        }
     });
   ```

---

## License

- This project is licensed under the MIT License.

---

## Contact

If you have any questions, issues, or suggestions regarding this project, feel free to reach out:

- **Email**: [me@mahmoud-mohsen.com](mailto:me@mahmoud-mohsen.com)

I am happy to assist and answer any queries related to this service.
