import { createApp } from "vue";
import App from "@/App.vue";
import router from "./router";
import store, { key } from "./store";
import './assets/main.css'

// Create the Vue application
const app = createApp(App);

// Initialize authentication state
const initializeAuth = async () => {
  try {
    // Check if we have a token in localStorage
    const token = localStorage.getItem("token");
    if (token) {
      // Initialize auth state with token
      await store.dispatch("user/checkAuth");
      
      // Fetch user profile if we have a valid token
      await store.dispatch("user/getUserProfile");
    }
  } catch (error) {
    console.error("Error initializing auth:", error);
    // Clear invalid token
    localStorage.removeItem("token");
  }
};

// Initialize auth before mounting the app
initializeAuth().then(() => {
  // Use router and store
  app.use(router);
  app.use(store, key);
  
  // Mount the app
  app.mount("#app");
});
