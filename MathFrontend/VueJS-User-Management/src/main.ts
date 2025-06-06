import { createApp } from "vue";
import App from "@/App.vue";
import router from "./router";
import store, { key } from "./store";
import './assets/main.css'
import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";

// Toast options
const toastOptions = {
  position: "top-right",
  timeout: 3000,
  closeOnClick: true,
  pauseOnFocusLoss: true,
  pauseOnHover: true,
  draggable: true,
  draggablePercent: 0.6,
  showCloseButtonOnHover: false,
  hideProgressBar: false,
  closeButton: "button",
  icon: true,
  rtl: false
};

// Create the Vue application
const app = createApp(App);

// Use plugins
app.use(router);
app.use(store, key);
app.use(Toast, toastOptions);

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
    // Clear invalid token
    localStorage.removeItem("token");
  }
};

// Initialize auth and mount app
initializeAuth().then(() => {
  app.mount("#app");
});
