import axios, {
  AxiosInstance,
  AxiosError,
  AxiosRequestConfig,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios";

// Define interfaces for our data models
interface RegisterData {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

interface LoginData {
  email: string;
  password: string;
}

interface VerifyAccountData {
  code: string;
}

interface ResetPasswordData {
  code: string;
  password: string;
}

interface ChangePasswordData {
  userId: string;
  currentPassword: string;
  newPassword: string;
}

interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
  createdAt: string;
}

interface ApiResponse<T = any> {
  success: boolean;
  message: string;
  data?: T;
  token?: string;
  errors?: string[];
  user?: any;
}

// Base API URL for our C# backend
const API_URL = "http://localhost:5000/api";

// Create axios instance with default config
export const api: AxiosInstance = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config: InternalAxiosRequestConfig): InternalAxiosRequestConfig => {
    const token = localStorage.getItem("token");
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  }
);

// Add response interceptor to handle token errors
api.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    // If the error is 401 Unauthorized and not a login request
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url?.includes("/auth/login")
    ) {
      originalRequest._retry = true;

      // Clear invalid token
      localStorage.removeItem("token");

      // Redirect to login page
      if (window.location.pathname !== "/login") {
        window.location.href = "/login";
      }
    }

    return Promise.reject(error);
  }
);

// Utility function for handling errors
const handleError = (error: AxiosError | Error): never => {
  if (axios.isAxiosError(error) && error.response) {
    // Server responded with an error status
    throw error.response.data;
  } else if (axios.isAxiosError(error) && error.request) {
    // No response received
    throw { message: "No response from server. Please check your connection." };
  } else {
    // Something else went wrong
    throw error;
  }
};

export const register = async (
  userData: RegisterData
): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.post(
      "/auth/register",
      {
        FirstName: userData.firstName,
        LastName: userData.lastName,
        Email: userData.email,
        Password: userData.password,
      }
    );
    return response.data;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const login = async (credentials: LoginData): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.post("/auth/login", {
      Email: credentials.email,
      Password: credentials.password,
    });
    return response.data;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const verifyAccount = async (
  data: VerifyAccountData
): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.post(
      "/auth/verify-account",
      data
    );
    return response.data;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const forgotPassword = async (email: string): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.post(
      "/auth/forgot-password",
      { email }
    );
    return response.data;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const resetPassword = async (
  data: ResetPasswordData
): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.post(
      `/auth/reset-password/${data.code}`,
      {
        newPassword: data.password,
      }
    );
    return response.data;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const getUserProfile = async (): Promise<any> => {
  try {
    const response = await api.get("/user/profile");
    return response.data;
  } catch (error) {
    console.error("Error getting user profile:", error);
    throw error;
  }
};

export const updateUserProfile = async (
  userData: Partial<UserProfile>
): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse = await api.put("/user/profile", userData);

    // Transform the direct response into ApiResponse format
    return {
      success: true,
      data: response.data,
      message: "Profile updated successfully",
    };
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const changePassword = async (
  data: ChangePasswordData
): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.post(
      "/user/change-password",
      {
        UserId: data.userId,
        CurrentPassword: data.currentPassword,
        NewPassword: data.newPassword,
      }
    );
    return response.data;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const deleteAccount = async (userId: string): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.delete(
      `/user/${userId}`
    );
    return response.data;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const logout = async (): Promise<boolean> => {
  try {
    // Clear the token from axios headers
    delete api.defaults.headers.common["Authorization"];
    // Clear token from localStorage
    localStorage.removeItem("token");
    return true;
  } catch (error) {
    return false;
  }
};
