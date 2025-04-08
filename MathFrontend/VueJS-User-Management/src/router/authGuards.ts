import store from "../store";
import { NavigationGuardNext, RouteLocationNormalized } from "vue-router";

// Interface for user data from store
interface UserData {
  email: string;
  firstName?: string;
  lastName?: string;
  id?: string;
  token?: string;
  [key: string]: any;
}

export const requireAuth = (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
): void => {
  if (!store.getters["user/isAuthenticated"]) {
    next({ name: "Login" });
  } else {
    next();
  }
};

export const guestOnly = (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
): void => {
  if (store.getters["user/isAuthenticated"]) {
    next({ name: "Profile" });
  } else {
    next();
  }
};

export const verifyAccountAccess = (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
): void => {
  if (store.getters["user/isAuthenticated"]) {
    next({ name: "Profile" });
  } else {
    const userData = store.getters["user/userData"] as UserData;
    if (!userData.email) {
      next({ name: "Login" });
    } else {
      next();
    }
  }
};

export const handleLogout = async (
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
): Promise<void> => {
  await store.dispatch("user/logout");
  next({ name: "Login" });
};
