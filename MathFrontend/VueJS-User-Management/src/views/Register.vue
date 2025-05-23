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
              <div class="card-body p-3 p-md-4 p-xl-5">
                <div class="mb-4 mt-4">
                  <h1 class="text-3xl">Register</h1>
                  <p>
                    Create your new account or
                    <router-link class="text-primary-400" to="/login">login</router-link> with existing
                    account.
                  </p>
                </div>

                <!-- Register Form -->
                <form @submit.prevent="register" autoComplete="off">
                  <div class="row gy-3 gy-md-4 overflow-hidden">
                    <div class="col-6">
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
                    <div class="col-6">
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
