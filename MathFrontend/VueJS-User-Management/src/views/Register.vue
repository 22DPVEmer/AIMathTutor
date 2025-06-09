<template>
  <main class="min-h-screen bg-gray-50">
    <!-- Mobile: Full screen form -->
    <div class="md:hidden min-h-screen flex items-center justify-center px-6 py-12">
      <div class="w-full max-w-md bg-white rounded-xl shadow-xl p-8">
        <div class="text-center mb-10">
          <h1 class="text-3xl font-bold text-gray-900">Join Us</h1>
          <p class="text-base mt-3 text-gray-600 leading-relaxed">
            Create your account or
            <router-link class="text-primary-600 hover:text-primary-700 font-medium" to="/login">sign in</router-link> if you already have one
          </p>
        </div>

        <!-- Mobile Register Form -->
        <form @submit.prevent="register" autoComplete="off" class="space-y-6">
          <!-- Mobile name fields - stacked -->
          <div class="space-y-5">
            <div>
              <InputField
                v-model="firstName"
                id="firstName"
                label="First Name"
                placeholder="John"
                :disabled="isSubmitting"
              />
              <div
                v-if="validationErrors.firstName"
                class="alert-message"
              >
                <span class="text-danger">{{
                  validationErrors.firstName
                }}</span>
              </div>
            </div>
            <div>
              <InputField
                v-model="lastName"
                id="lastName"
                label="Last Name"
                placeholder="Doe"
                :disabled="isSubmitting"
              />
              <div
                v-if="validationErrors.lastName"
                class="alert-message"
              >
                <span class="text-danger">{{
                  validationErrors.lastName
                }}</span>
              </div>
            </div>
            <div>
              <InputField
                v-model="email"
                id="email"
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
                id="password"
                label="Password"
                :disabled="isSubmitting"
              />
              <!-- Password strength hidden on mobile for cleaner form -->
              <div
                v-if="validationErrors.password"
                class="alert-message"
              >
                <span class="text-danger">{{
                  validationErrors.password
                }}</span>
              </div>
            </div>

            <div v-if="error" class="p-4 rounded-lg bg-red-50 border border-red-200 shadow-sm mb-4">
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
                <p class="text-sm font-medium text-red-800">
                  <span class="font-semibold">Error:</span> {{ error }}
                </p>
              </div>
            </div>

            <button
              class="w-full py-4 px-6 bg-primary-600 hover:bg-primary-700 text-white font-semibold rounded-lg transition-colors text-lg focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 shadow-lg"
              type="submit"
              :disabled="isSubmitting"
            >
              <span v-if="isSubmitting" class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Creating Account...
              </span>
              <span v-else>Create Account</span>
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

            <!-- Right Side: Register Form -->
            <div class="w-1/2 flex items-center justify-center">
              <div class="w-full max-w-md p-8 lg:p-10">
                <div class="mb-8">
                  <h1 class="text-3xl font-bold text-gray-900">Register</h1>
                  <p class="text-base mt-2 text-gray-600">
                    Create your new account or
                    <router-link class="text-primary-600 hover:text-primary-700 font-medium" to="/login">login</router-link> with existing
                    account.
                  </p>
                </div>

                <!-- Desktop Register Form -->
                <form @submit.prevent="register" autoComplete="off" class="space-y-6">
                  <!-- Name fields - side by side on desktop -->
                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <InputField
                        v-model="firstName"
                        id="firstName-desktop"
                        label="First Name"
                        placeholder="John"
                        :disabled="isSubmitting"
                      />
                      <div
                        v-if="validationErrors.firstName"
                        class="alert-message"
                      >
                        <span class="text-danger">{{
                          validationErrors.firstName
                        }}</span>
                      </div>
                    </div>
                    <div>
                      <InputField
                        v-model="lastName"
                        id="lastName-desktop"
                        label="Last Name"
                        placeholder="Doe"
                        :disabled="isSubmitting"
                      />
                      <div
                        v-if="validationErrors.lastName"
                        class="alert-message"
                      >
                        <span class="text-danger">{{
                          validationErrors.lastName
                        }}</span>
                      </div>
                    </div>
                  </div>

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
                    <PasswordStrength :password="password" class="mt-2" />
                    <div
                      v-if="validationErrors.password"
                      class="alert-message"
                    >
                      <span class="text-danger">{{
                        validationErrors.password
                      }}</span>
                    </div>
                  </div>

                  <div v-if="error" class="p-3 rounded-lg bg-red-50 border border-red-200 shadow-sm">
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
                      <p class="text-sm font-medium text-red-800">
                        <span class="font-semibold">Error:</span> {{ error }}
                      </p>
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
                        Creating Account...
                      </span>
                      <span v-else>Create Account</span>
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
import PasswordStrength from "@/components/Common/PasswordStrength.vue";
import { validateRegisterForm } from "@/utils/registerValidation";

export default {
  components: {
    SideBanner,
    InputField,
    PasswordField,
    PasswordStrength,
  },
  data() {
    return {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      error: null,
      validationErrors: {},
      isSubmitting: false,
    };
  },
  methods: {
    async register() {
      // Set loading state
      this.isSubmitting = true;

      // reset error variables
      this.error = null;
      this.validationErrors = {};

      // Validate form data
      const validationErrors = validateRegisterForm({
        firstName: this.firstName,
        lastName: this.lastName,
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
        await this.$store.dispatch("user/register", {
          // dispatching action to register user in Vuex store
          firstName: this.firstName,
          lastName: this.lastName,
          email: this.email,
          password: this.password,
        });

        // push to account verification route
        this.$router.push({ name: "VerifyAccount" });
      } catch (error) {
        this.error = error.message;
      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>
