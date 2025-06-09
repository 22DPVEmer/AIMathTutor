<template>
  <main class="min-h-screen bg-gray-50">
    <!-- Mobile: Full screen form optimized for better spacing and positioning -->
    <div class="md:hidden min-h-screen flex items-start justify-center px-4 pt-16 pb-8">
      <!-- Container: Wider and better positioned for improved visual balance -->
      <div class="w-full max-w-sm bg-white rounded-xl shadow-xl px-8 py-10">
        <!-- Header Section: Optimized spacing -->
        <div class="text-center">
          <h1 class="text-3xl font-bold text-gray-900 mb-4">Welcome Back</h1>
          <p class="text-base text-gray-600 leading-relaxed px-2">
            Sign in to your account or
            <router-link class="text-primary-600 hover:text-primary-700 font-medium" to="/register"
              >create a new one</router-link
            >
          </p>
        </div>

        <!-- Mobile Login Form: ~300px height (45% of screen) with optimized spacing -->
        <form @submit.prevent="login" autocomplete="off" class="space-y-5">
          <div class="space-y-5">
            <!-- Email Field with 20-25px spacing -->
            <div>
              <InputField
                v-model="email"
                id="email"
                type="email"
                label="Email"
                placeholder="name@example.com"
                :disabled="isSubmitting"
              />
              <div v-if="validationErrors.email" class="alert-message mt-1">
                <span class="text-red-600 text-sm">{{
                  validationErrors.email
                }}</span>
              </div>
            </div>

            <!-- Password Field with consistent spacing -->
            <div>
              <PasswordField
                v-model="password"
                id="password"
                label="Password"
                :disabled="isSubmitting"
              />
              <div
                v-if="validationErrors.password"
                class="alert-message mt-1"
              >
                <span class="text-red-600 text-sm">{{
                  validationErrors.password
                }}</span>
              </div>
            </div>

            <!-- Forgot Password Link with minimal spacing -->
            <div class="text-center">
              <router-link
                to="/forgot-password"
                class="text-sm text-primary-600 hover:text-primary-700 font-medium"
                >Forgot your password?</router-link
              >
            </div>

            <!-- Error Message with proper mobile spacing -->
            <div
              v-if="error"
              class="p-4 rounded-lg bg-red-50 border border-red-200 shadow-sm"
            >
              <div class="flex items-center">
                <svg
                  class="h-5 w-5 text-red-500 mr-3 flex-shrink-0"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
                  />
                </svg>
                <div>
                  <p class="text-sm font-medium text-red-800">
                    {{ error }}
                  </p>
                  <router-link
                    v-if="emailExists"
                    to="/verify-account"
                    class="text-sm font-medium text-primary-600 hover:text-primary-800 underline"
                  >
                    Verify Account
                  </router-link>
                </div>
              </div>
            </div>

            <!-- Login Button: 48-56px height, matches input height for consistency -->
            <button
              class="w-full h-12 px-6 bg-primary-600 hover:bg-primary-700 text-white font-semibold rounded-lg transition-colors text-base focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 shadow-lg mt-1"
              type="submit"
              :disabled="isSubmitting"
            >
              <span v-if="isSubmitting" class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Logging in...
              </span>
              <span v-else>Login</span>
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Desktop: Two-column layout -->
    <div class="hidden md:flex min-h-screen items-center justify-center px-6 lg:px-8">
      <div class="w-full max-w-6xl">
        <div class="bg-white rounded-lg shadow-lg overflow-hidden">
          <div class="flex">
            <!-- Left Side: Image and Introduction -->
            <SideBanner />

            <!-- Right Side: Login Form -->
            <div class="w-1/2 flex items-center justify-center">
              <div class="w-full max-w-md p-8 lg:p-10">
                <div class="mb-8">
                  <h1 class="text-3xl font-bold text-gray-900">Login</h1>
                  <p class="text-base mt-2 text-gray-600">
                    Enter your account or
                    <router-link class="text-primary-600 hover:text-primary-700 font-medium" to="/register"
                      >create</router-link
                    >
                    a new account.
                  </p>
                </div>

                <!-- Desktop Login Form -->
                <form @submit.prevent="login" autocomplete="off" class="space-y-6">
                  <div>
                    <InputField
                      v-model="email"
                      id="email-desktop"
                      type="email"
                      label="Email"
                      placeholder="name@example.com"
                      :disabled="isSubmitting"
                    />
                    <div v-if="validationErrors.email" class="alert-message">
                      <span class="text-danger">{{
                        validationErrors.email
                      }}</span>
                    </div>
                  </div>
                  <div>
                    <PasswordField
                      v-model="password"
                      id="password-desktop"
                      label="Password"
                      :disabled="isSubmitting"
                    />
                    <div
                      v-if="validationErrors.password"
                      class="alert-message"
                    >
                      <span class="text-danger">{{
                        validationErrors.password
                      }}</span>
                    </div>
                  </div>
                  <div class="text-right">
                    <router-link
                      to="/forgot-password"
                      class="text-sm text-primary-600 hover:text-primary-700 font-medium"
                      >Forgot password?</router-link
                    >
                  </div>
                  <div
                    v-if="error"
                    class="p-3 rounded-lg bg-red-50 border border-red-200 shadow-sm"
                  >
                    <div class="flex items-center">
                      <svg
                        class="h-5 w-5 text-red-500 mr-2 flex-shrink-0"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                      >
                        <path
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          stroke-width="2"
                          d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
                        />
                      </svg>
                      <div>
                        <p class="text-sm font-medium text-red-800">
                          {{ error }}
                        </p>
                        <router-link
                          v-if="emailExists"
                          to="/verify-account"
                          class="text-sm font-medium text-primary-600 hover:text-primary-800 underline"
                        >
                          Verify Account
                        </router-link>
                      </div>
                    </div>
                  </div>
                  <div>
                    <button
                      class="w-full py-3 px-4 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-md transition-colors text-base focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                      type="submit"
                      :disabled="isSubmitting"
                    >
                      <span v-if="isSubmitting" class="flex items-center justify-center">
                        <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                        </svg>
                        Logging in...
                      </span>
                      <span v-else>Login</span>
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</template>

<script>
import SideBanner from "@/components/Common/SideBanner.vue";
import InputField from "@/components/Common/InputField.vue";
import PasswordField from "@/components/Common/PasswordField.vue";
import { validateLoginForm } from "@/utils/loginValidation";

export default {
  components: {
    SideBanner,
    InputField,
    PasswordField,
  },
  data() {
    return {
      email: "",
      password: "",
      error: null,
      validationErrors: {},
      isSubmitting: false,
      emailExists: false,
    };
  },
  methods: {
    async login() {
      // Set loading state
      this.isSubmitting = true;

      // reset error variables
      this.error = null;
      this.validationErrors = {};

      // remove user store data
      this.$store.dispatch("user/logout");
      this.emailExists = false;

      // Validate form data
      const validationErrors = validateLoginForm({
        email: this.email,
        password: this.password,
      });

      // Check for validation errors
      if (Object.values(validationErrors).some((error) => error !== null)) {
        this.validationErrors = validationErrors;
        this.isSubmitting = false;
        return;
      }

      try {
        await this.$store.dispatch("user/login", {
          email: this.email,
          password: this.password,
        });

        // push to profile route
        this.$router.push({ name: "Profile" });
      } catch (error) {
        this.error = error.message;

        if (this.$store.getters["user/userData"].email) {
          this.emailExists = true;
        }
      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>
