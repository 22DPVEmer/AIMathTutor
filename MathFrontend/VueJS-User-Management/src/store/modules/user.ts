import * as userApi from "@/api/user";
import { Module } from "vuex";
import { RootState } from "..";
import axios from "axios";

// Define the UserState interface
export interface UserState {
  userData: {
    email: string;
    firstName?: string;
    lastName?: string;
    id?: string;
    token?: string;
    [key: string]: any;
  };
  loading: boolean;
  error: string | null;
}

// Define the types for the API responses
interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  data?: any;
  errors?: string[];
}

// Create the Vuex module with proper typing
const userModule: Module<UserState, RootState> = {
  namespaced: true,

  state: (): UserState => ({
    userData: {
      email: "",
    },
    loading: false,
    error: null,
  }),

  mutations: {
    SET_USER_DATA(
      state: UserState,
      userData: Partial<UserState["userData"]>
    ): void {
      state.userData = { ...state.userData, ...userData };

      // Save token to localStorage if present
      if (userData.token) {
        localStorage.setItem("token", userData.token);
      }
      
      // Cache user data if we have meaningful user information
      if (userData.firstName || userData.lastName || userData.id) {
        // Store a copy of user data without the token
        const { token, ...userDataWithoutToken } = state.userData;
        localStorage.setItem("userData", JSON.stringify(userDataWithoutToken));
      }
    },

    SET_LOADING(state: UserState, loading: boolean): void {
      state.loading = loading;
    },

    SET_ERROR(state: UserState, error: string | null): void {
      state.error = error;
    },

    CLEAR_USER_DATA(state: UserState): void {
      state.userData = { email: "" };
      localStorage.removeItem("token");
      localStorage.removeItem("userData");
    },
  },

  actions: {
    async register(
      { commit },
      userData: {
        firstName: string;
        lastName: string;
        email: string;
        password: string;
      }
    ): Promise<AuthResponse> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const response = await userApi.register(userData);

        if (response.success) {
          commit("SET_USER_DATA", {
            email: userData.email,
            firstName: userData.firstName,
            lastName: userData.lastName,
          });
        } else {
          commit("SET_ERROR", response.message || "Registration failed");
        }

        commit("SET_LOADING", false);
        return response;
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error ? error.message : "Registration failed";
        commit("SET_ERROR", errorMessage);
        throw error;
      }
    },

    async login(
      { commit },
      credentials: {
        email: string;
        password: string;
        firstName?: string;
        lastName?: string;
      }
    ): Promise<AuthResponse> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const response = await userApi.login(credentials);

        if (response.success) {
          commit("SET_USER_DATA", {
            ...response.user,
            token: response.token,
          });
        } else {
          commit("SET_ERROR", response.message || "Login failed");
        }

        commit("SET_LOADING", false);
        return response;
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error ? error.message : "Login failed";
        commit("SET_ERROR", errorMessage);
        throw error;
      }
    },

    async logout({ commit }): Promise<void> {
      try {
        await userApi.logout();
      } catch (error) {
        console.error("Logout error:", error);
      } finally {
        commit("CLEAR_USER_DATA");
      }
    },

    async getUserProfile({ commit, state }): Promise<any> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const profileData = await userApi.getUserProfile();
        // Preserve the existing token when updating user data
        const existingToken = state.userData.token;
        commit("SET_USER_DATA", {
          ...profileData,
          token: existingToken,
        });
        commit("SET_LOADING", false);
        return profileData;
      } catch (error) {
        commit("SET_LOADING", false);
        
        // Check if this is an authentication error
        if (axios.isAxiosError(error) && error.response?.status === 401) {
          // If unauthorized, clear user data and redirect to login
          commit("CLEAR_USER_DATA");
          // Router redirect will be handled by the axios interceptor
        } else {
          const errorMessage =
            error instanceof Error ? error.message : "Failed to get user profile";
          commit("SET_ERROR", errorMessage);
        }
        
        throw error;
      }
    },

    async updateUserProfile(
      { commit },
      userData: Partial<UserState["userData"]>
    ): Promise<AuthResponse> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const response = await userApi.updateUserProfile(userData);
        
        if (response.success && response.data) {

          commit("SET_USER_DATA", response.data);
          console.log("Profile updated successfully", response.data);
        } else {
          commit("SET_ERROR", response.message || "Profile update failed");
          console.log("Profile update failed", response.message);
        }

        commit("SET_LOADING", false);
        return response;
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error ? error.message : "Profile update failed";
        commit("SET_ERROR", errorMessage);
        throw error;
      }
    },

    async changePassword(
      { commit },
      passwordData: {
        userId: string;
        currentPassword: string;
        newPassword: string;
      }
    ): Promise<AuthResponse> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const response = await userApi.changePassword(passwordData);

        if (!response.success) {
          commit("SET_ERROR", response.message || "Password change failed");
        }

        commit("SET_LOADING", false);
        return response;
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error ? error.message : "Password change failed";
        commit("SET_ERROR", errorMessage);
        throw error;
      }
    },

    async deleteAccount({ commit }, userId: string): Promise<AuthResponse> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const response = await userApi.deleteAccount(userId);

        if (response.success) {
          // On success, clear user data as they've deleted their account
          commit("CLEAR_USER_DATA");
        } else {
          commit("SET_ERROR", response.message || "Account deletion failed");
        }

        commit("SET_LOADING", false);
        return response;
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error ? error.message : "Account deletion failed";
        commit("SET_ERROR", errorMessage);
        throw error;
      }
    },

    checkAuth({ commit, dispatch }): void {
      const token = localStorage.getItem("token");
      if (token) {
        // Set the token in state
        commit("SET_USER_DATA", { token });
        
        // Try to recover any cached user data
        const cachedUserData = localStorage.getItem("userData");
        if (cachedUserData) {
          try {
            const userData = JSON.parse(cachedUserData);
            // Restore the cached user data with the current token
            commit("SET_USER_DATA", { ...userData, token });
          } catch (error) {
            console.error("Error parsing cached user data:", error);
            // If there's an error, we'll just rely on getUserProfile
          }
        }
      }
    },
  },

  getters: {
    isAuthenticated(state: UserState): boolean {
      return !!state.userData.token;
    },

    userData(state: UserState): UserState["userData"] {
      return state.userData;
    },

    loading(state: UserState): boolean {
      return state.loading;
    },

    error(state: UserState): string | null {
      return state.error;
    },
  },
};

export default userModule;
