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
const api: AxiosInstance = axios.create({
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

// Utility function for handling errors
const handleError = (error: AxiosError | Error): never => {
  console.error("API Error:", error);
  if (axios.isAxiosError(error) && error.response) {
    // Server responded with an error status
    console.error("Response data:", error.response.data);
    console.error("Response status:", error.response.status);
    console.error("Response headers:", error.response.headers);
    throw error.response.data;
  } else if (axios.isAxiosError(error) && error.request) {
    // No response received
    console.error("Request details:", error.request);
    throw { message: "No response from server. Please check your connection." };
  } else {
    // Something else went wrong
    console.error("Error message:", error.message);
    throw error;
  }
};

export const register = async (
  userData: RegisterData
): Promise<ApiResponse> => {
  try {
    console.log("Sending registration request with data:", userData);
    const response: AxiosResponse<ApiResponse> = await api.post(
      "/auth/register",
      {
        FirstName: userData.firstName,
        LastName: userData.lastName,
        Email: userData.email,
        Password: userData.password,
      }
    );
    console.log("Registration response:", response.data);
    return response.data;
  } catch (error) {
    console.error("Registration error:", error);
    return handleError(error as AxiosError | Error);
  }
};

export const login = async (credentials: LoginData): Promise<ApiResponse> => {
  try {
    console.log("Sending login request with data:", credentials);
    const response: AxiosResponse<ApiResponse> = await api.post("/auth/login", {
      Email: credentials.email,
      Password: credentials.password,
    });
    console.log("Login response:", response.data);
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

export const getUserProfile = async (): Promise<UserProfile> => {
  try {
    const response: AxiosResponse<ApiResponse<UserProfile>> = await api.get(
      "/user/profile"
    );
    return response.data.data as UserProfile;
  } catch (error) {
    return handleError(error as AxiosError | Error);
  }
};

export const updateUserProfile = async (
  userData: Partial<UserProfile>
): Promise<ApiResponse> => {
  try {
    const response: AxiosResponse<ApiResponse> = await api.put(
      "/user",
      userData
    );
    return response.data;
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
    console.error("Logout error:", error);
    return false;
  }
};
