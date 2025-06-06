<template>
  <main>
    <section class="p-3 p-md-4 p-xl-5">
      <div class="container">
        <div class="card border-light-subtle shadow-sm">
          <div class="row g-0">
            <!-- Left Side: Image and Introduction Component -->
            <SideBanner />

            <!-- Right Side: Register Form -->
            <div class="col-12 col-md-6">
              <div class="card-body p-4 md:p-6 lg:p-8">
                <div class="mb-6 mt-4">
                  <h1 class="text-2xl md:text-3xl font-bold">Register</h1>
                  <p class="text-base mt-2">
                    Create your new account or
                    <router-link class="text-primary-600 hover:text-primary-700 font-medium" to="/login">login</router-link> with existing
                    account.
                  </p>
                </div>

                <!-- Register Form -->
                <form @submit.prevent="register" autoComplete="off">
                  <div class="row gy-3 gy-md-4 overflow-hidden">
                    <!-- Mobile: Full width, Desktop: Half width -->
                    <div class="col-12 md:col-6">
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
                    <div class="col-12 md:col-6">
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
                    <div class="col-12">
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
                    <div class="col-12">
                      <PasswordField
                        v-model="password"
                        id="password"
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
                    <div v-if="error" class="alert-message">
                      <span class="text-danger">Error:</span> {{ error }}
                    </div>
                    <div class="d-grid mt-4">
                      <button
                        class="w-full py-3 px-4 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-md transition-colors text-base md:text-lg"
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
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
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
