import {
  createRouter,
  createWebHistory,
  RouteRecordRaw,
  RouteLocationNormalized,
  NavigationGuardNext,
} from "vue-router";
import { guestOnly, verifyAccountAccess, handleLogout } from "./authGuards";
import Register from "../views/Register.vue";
import VerifyAccount from "../views/VerifyAccount.vue";
import Login from "../views/Login.vue";
import Profile from "../views/Profile.vue";
import ForgotPassword from "../views/ForgotPassword.vue";
import ResetPassword from "../views/ResetPassword.vue";
import store from "@/store";

// Define the route meta types
interface RouteMeta {
  requiresAuth?: boolean;
  title?: string;
}

// Extend the RouteRecordRaw to include our custom meta
interface AppRouteRecordRaw extends Omit<RouteRecordRaw, "meta"> {
  meta?: RouteMeta;
}

const routes: AppRouteRecordRaw[] = [
  {
    path: "/",
    redirect: "/login",
  },
  {
    path: "/register",
    name: "Register",
    component: Register,
    beforeEnter: guestOnly,
    meta: { title: "Register" },
  },
  {
    path: "/verify-account",
    name: "VerifyAccount",
    component: VerifyAccount,
    beforeEnter: verifyAccountAccess,
    meta: { title: "Verify Account" },
  },
  {
    path: "/login",
    name: "Login",
    component: Login,
    beforeEnter: guestOnly,
    meta: { title: "Login" },
  },
  {
    path: "/profile",
    name: "Profile",
    component: Profile,
    meta: { requiresAuth: true, title: "Profile" },
  },
  {
    path: "/forgot-password",
    name: "ForgotPassword",
    component: ForgotPassword,
    beforeEnter: guestOnly,
    meta: { title: "Forgot Password" },
  },
  {
    path: "/reset-password/:code",
    name: "ResetPassword",
    component: ResetPassword,
    beforeEnter: guestOnly,
    meta: { title: "Reset Password" },
  },
  {
    path: "/logout",
    name: "Logout",
    beforeEnter: handleLogout,
    meta: { title: "Logout" },
  },
  {
    path: "/:pathMatch(.*)*",
    name: "NotFound",
    component: () => import("../views/NotFound.vue"),
    meta: { title: "Page Not Found" },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes: routes as RouteRecordRaw[],
});

router.beforeEach(
  (
    to: RouteLocationNormalized,
    from: RouteLocationNormalized,
    next: NavigationGuardNext
  ) => {
    // Update document title
    const defaultTitle = "Math Tutor";
    document.title = to.meta.title
      ? `${to.meta.title} | ${defaultTitle}`
      : defaultTitle;

    // Check auth requirements
    if (to.matched.some((record) => record.meta.requiresAuth)) {
      if (!store.getters["user/isAuthenticated"]) {
        next({ name: "Login" });
      } else {
        next();
      }
    } else {
      next();
    }
  }
);

export default router;
