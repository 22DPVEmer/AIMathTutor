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
                  <h3>Login</h3>
                  <p>Enter your account or <router-link to="/register">create</router-link> a new account.</p>
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
                      <div v-if="validationErrors.email" class="alert-message"><span class="text-danger">{{ validationErrors.email }}</span></div>
                    </div>
                    <div class="col-12">
                      <PasswordField
                          v-model="password"
                          id="password"
                          label="Password"
                          :disabled="isSubmitting"
                           />
                      <div v-if="validationErrors.password" class="alert-message"><span class="text-danger">{{ validationErrors.password }}</span></div>
                    </div>
                    <div class="col-6">
                      <div class="form-check">
                        <input class="form-check-input" type="checkbox" v-model="rememberMe" id="remember_me" />
                        <label class="form-check-label text-secondary" for="remember_me">Remember me</label>
                      </div>
                    </div>
                    <div class="col-6 text-end">
                      <router-link to="/forgot-password" class="link-secondary text-decoration-none">Forget password?</router-link>
                    </div>
                    <div v-if="error" class="alert-message"><span class="text-danger">Error:</span> {{ error }} <router-link v-if="emailExists" to="/verify-account">Verify Account</router-link></div>
                    <div class="d-grid mt-3">
                      <button
                          class="btn bsb-btn-xl btn-primary"
                          type="submit"
                          :disabled="isSubmitting"
                      >
                      <span v-if="isSubmitting">
                        <i class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></i>
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
import SideBanner from '@/components/Common/SideBanner.vue';
import InputField from '@/components/Common/InputField.vue';
import PasswordField from '@/components/Common/PasswordField.vue';
import { validateLoginForm } from '@/utils/loginValidation';

export default {
  components: {
    SideBanner,
    InputField,
    PasswordField,
  },
  data() {
    return {
      email: '',
      password: '',
      rememberMe: false,
      error: null,
      validationErrors: {},
      isSubmitting: false,
      emailExists: false
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
      this.$store.dispatch('user/logout');
      this.emailExists = false;

      // Validate form data
      const validationErrors = validateLoginForm({
        email: this.email,
        password: this.password,
      });

      // Check for validation errors
      if (Object.values(validationErrors).some(error => error !== null)) {
        this.validationErrors = validationErrors;
        this.isSubmitting = false;
        return;
      }

      try {

        await this.$store.dispatch(
            'user/login',
            {
              email: this.email,
              password: this.password
            }
        );

        // push to profile route
        this.$router.push({name: 'Profile'});

      }  catch (error) {
        this.error = error.message;

        if(this.$store.getters['user/userData'].email){
          this.emailExists = true;
        }

      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>
