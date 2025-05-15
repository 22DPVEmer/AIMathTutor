<template>
  <main>
    <section class="p-3 p-md-4 p-xl-5">
      <div class="container">
        <div class="card border-light-subtle shadow-sm">
          <div class="row g-0">
            <!-- Left Side: Image and Introduction -->
            <SideBanner />

            <!-- Right Side: Login Form -->
            <div class="col-12 col-md-6">
              <div class="card-body p-3 p-md-4 p-xl-5">
                <div class="mb-4 mt-4">
                  <h1 class="text-3xl">Login</h1>
                  <p>
                    Enter your account or
                    <router-link class="text-primary-400" to="/register"
                      >create</router-link
                    >
                    a new account.
                  </p>
                </div>

                <!-- Login Form -->
                <form @submit.prevent="login" autocomplete="off">
                  <div class="row gy-3 gy-md-4 overflow-hidden">
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
                    <div class="col-12 flex flex-col">
                      <PasswordField
                        v-model="password"
                        id="password"
                        class="mt-3"
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
                    <div class="col-6">
                      <div class="form-check">
                        <input
                          class="form-check-input"
                          type="checkbox"
                          v-model="rememberMe"
                          id="remember_me"
                        />
                        <label
                          class="form-check-label text-secondary"
                          for="remember_me"
                          >Remember me</label
                        >
                      </div>
                    </div>
                    <div class="col-6 text-end">
                      <router-link
                        to="/forgot-password"
                        class="link-secondary text-decoration-none"
                        >Forget password?</router-link
                      >
                    </div>
                    <div
                      v-if="error"
                      class="mt-3 p-3 rounded-lg bg-red-50 border border-red-200 shadow-sm"
                    >
                      <div class="flex items-center">
                        <svg
                          class="h-5 w-5 text-red-500 mr-2"
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
                    <div class="d-grid mt-3">
                      <button
                        class="btn bsb-btn-xl btn-primary"
                        type="submit"
                        :disabled="isSubmitting"
                      >
                        <span v-if="isSubmitting">
                          <i
                            class="spinner-border spinner-border-sm"
                            role="status"
                            aria-hidden="true"
                          ></i>
                        </span>
                        <span v-else>Login</span>
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
      rememberMe: false,
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
