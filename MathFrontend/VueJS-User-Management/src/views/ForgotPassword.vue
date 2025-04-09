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
                  <h3>Forgot Password</h3>
                  <p>Enter your email to reset your password.</p>
                </div>

                <!-- Register Form -->
                <form @submit.prevent="requestResetPassword" autoComplete="off">
                  <div class="row gy-3 gy-md-4 overflow-hidden">
                    <div class="col-12">
                      <InputField
                          v-model="email"
                          id="email"
                          type="email"
                          label="Email"
                          placeholder="name@example.com"
                          :disabled="isSubmitting"
                          required />
                      <div v-if="validationErrors.email" class="alert-message"><span class="text-danger">{{ validationErrors.email }}</span></div>
                    </div>

                    <div v-if="note" class="alert-message"><span class="text-danger">Note:</span> You will receive an Email with a reset password code.</div>

                    <div v-if="error" class="alert-message"><span class="text-danger">Error:</span> {{ error }}</div>
                    <div class="d-grid mt-3">
                      <button
                          class="btn bsb-btn-xl btn-primary"
                          type="submit"
                          :disabled="isSubmitting"
                      >
                        <span v-if="isSubmitting">
                          <i class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></i>
                        </span>
                        <span v-else>Submit</span>
                      </button>
                    </div>
                  </div>
                </form>
                <hr class="mt-5 mb-4 border-secondary-subtle">
                <div class="row">
                  <div class="col-6 col-md-6">
                    <router-link to="/login" class="link-secondary text-decoration-none">Login page</router-link>
                  </div>
                  <div class="col-6 col-md-6">
                    <div class="d-flex gap-2 gap-md-4 flex-column flex-md-row justify-content-md-end">
                      <router-link to="/register" class="link-secondary text-decoration-none">Register page</router-link>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </main>
</template>

<script>
import InputField from '../components/Common/InputField.vue';
import SideBanner from "@/components/Common/SideBanner.vue";
import { validateForgotPasswordForm } from '@/utils/forgotPasswordValidation';

export default {
  components: {
    SideBanner,
    InputField,
  },
  data() {
    return {
      email: '',
      note: true,
      error: null,
      validationErrors: {},
      isSubmitting: false,
    };
  },
  methods: {
    async requestResetPassword() {

      // Set loading state
      this.isSubmitting = true;

      // Hide the note
      this.note = false;

      // reset error variables
      this.error = null;
      this.validationErrors = {};

      // Validate form data
      const validationErrors = validateForgotPasswordForm({
        email: this.email,
      });

      // Check for validation errors
      if (Object.values(validationErrors).some(error => error !== null)) {
        this.validationErrors = validationErrors;
        this.isSubmitting = false;
        return;
      }

      try {

        await this.$store.dispatch(
            'user/forgotPassword',
            this.email
        );

        // push to login route
        this.$router.push({ name: 'Login' });

      } catch (error) {
        this.error = error.message;
      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>
