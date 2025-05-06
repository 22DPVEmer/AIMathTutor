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
import MathProblemView from "../views/MathProblemView.vue";
import TopicsView from "../views/TopicsView.vue";
import MyProblemsView from "../views/MyProblemsView.vue";
import store from "@/store";

// Define the route meta types
interface RouteMeta {
  requiresAuth?: boolean;
  title?: string;
}

// Extend the RouteRecordRaw type to include our meta type
interface AppRouteRecordRaw extends RouteRecordRaw {
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
    path: "/math-problems",
    name: "MathProblems",
    component: MathProblemView,
    meta: { requiresAuth: true, title: "Math Problem Generator" },
  },
  {
    path: "/topics",
    component: () => import("../views/TopicsView.vue"),
    meta: { requiresAuth: true, title: "Math Topics" },
    children: [
      {
        path: "",
        name: "TopicsList",
        component: () => import("../components/MathProblems/TopicsList.vue"),
        meta: { title: "Math Topics" }
      },
      {
        path: "parent/:parentId",
        name: "SubtopicsList",
        component: () => import("../components/MathProblems/SubtopicsList.vue"),
        meta: { title: "Subtopics" },
        props: true
      },
      {
        path: ":topicId/problems",
        name: "TopicProblems",
        component: () => import("../components/MathProblems/ProblemsList.vue"),
        meta: { title: "Topic Problems" },
        props: true
      },
      {
        path: ":topicId/problem/:problemId",
        name: "ProblemView",
        component: () => import("../components/MathProblems/ProblemView.vue"),
        meta: { title: "Problem" },
        props: true
      }
    ]
  },
  {
    path: "/my-problems",
    name: "MyProblems",
    component: MyProblemsView,
    meta: { requiresAuth: true, title: "My Problems" },
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

// Global navigation guard
router.beforeEach(async (to: RouteLocationNormalized, from: RouteLocationNormalized, next: NavigationGuardNext) => {
  // Set page title
  document.title = to.meta?.title ? `${to.meta.title} - AI Math Tutor` : 'AI Math Tutor';

  // Check if route requires authentication
  if (to.meta?.requiresAuth) {
    const isAuthenticated = store.getters["user/isAuthenticated"];

    if (!isAuthenticated) {
      // Try to restore auth state
      const token = localStorage.getItem("token");
      if (token) {
        try {
          await store.dispatch("user/checkAuth");
          await store.dispatch("user/getUserProfile");

          // If we successfully restored auth, proceed
          if (store.getters["user/isAuthenticated"]) {
            return next();
          }
        } catch (error) {
          console.error("Error restoring auth state:", error);
          localStorage.removeItem("token");
        }
      }

      // If we couldn't restore auth, redirect to login
      return next({ name: "Login", query: { redirect: to.fullPath } });
    }
  }

  next();
});

export default router;
