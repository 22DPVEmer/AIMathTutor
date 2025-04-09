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
                  <h3>Verify Account</h3>
                  <p>Enter the confirmation code, to activate your account.</p>
                </div>

                <!-- Email Verification Form -->
                <form @submit.prevent="verify" autocomplete="off">
                  <div class="row gy-3 gy-md-4 overflow-hidden">
                    <div class="col-12">
                      <InputField
                        v-model="email"
                        id="email"
                        type="email"
                        label="Email"
                        placeholder="name@example.com"
                        disabled
                        required
                      />
                    </div>
                    <div class="col-12">
                      <InputField
                        v-model="code"
                        id="code"
                        type="text"
                        label="Code"
                        placeholder=""
                        :disabled="isSubmitting"
                        required
                      />
                      <div v-if="validationErrors.code" class="alert-message">
                        <span class="text-danger">{{
                          validationErrors.code
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
                        <span v-else>Confirm</span>
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
import { validateAccountVerificationForm } from "@/utils/accountVerificationValidation.ts";

export default {
  components: {
    SideBanner,
    InputField,
  },
  data() {
    return {
      code: "",
      error: null,
      validationErrors: {},
      isSubmitting: false,
    };
  },
  methods: {
    async verify() {
      // Set loading state
      this.isSubmitting = true;

      // reset error variables
      this.error = null;
      this.validationErrors = {};

      // Validate form data
      const validationErrors = validateAccountVerificationForm({
        email: this.email,
        code: this.code,
      });

      // Check for validation errors
      if (Object.values(validationErrors).some((error) => error !== null)) {
        this.validationErrors = validationErrors;
        this.isSubmitting = false;
        return;
      }

      try {
        await this.$store.dispatch("user/verifyAccount", {
          email: this.email,
          code: this.code,
        });

        // push to login route
        this.$router.push({ name: "Login" });
      } catch (error) {
        this.error = error.message;
      } finally {
        this.isSubmitting = false;
      }
    },
  },
  computed: {
    email() {
      return this.$store.getters["user/userData"].email;
    },
  },
};
</script>
