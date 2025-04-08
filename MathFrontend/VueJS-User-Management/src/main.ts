import { createApp } from "vue";
import App from "@/App.vue";
import router from "./router";
import store, { key } from "./store";

// Create the Vue application
const app = createApp(App);

// Use router and store
app.use(router);
app.use(store, key); // Pass the store injection key

// Mount the app
app.mount("#app");
