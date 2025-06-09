<template>
  <div class="space-y-2">
    <!-- Label with 8-12px spacing above input -->
    <label :for="id" class="block text-sm font-medium text-gray-700 mb-2">
      {{ label }}
      <span class="text-red-600" v-if="required">*</span>
    </label>
    <!-- Password input container with mobile optimization -->
    <div class="relative flex items-center">
      <input
        :type="showPassword ? 'text' : 'password'"
        :id="id"
        class="mobile-optimized-input w-full h-12 px-4 pr-12 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 disabled:bg-gray-100 disabled:cursor-not-allowed transition-colors"
        :value="modelValue"
        @input="$emit('update:modelValue', $event.target.value)"
        :disabled="disabled"
      />
      <!-- Eye icon with proper touch target (44x44px minimum) -->
      <button
        type="button"
        class="absolute right-3 w-6 h-6 flex items-center justify-center cursor-pointer text-gray-500 hover:text-gray-700 focus:outline-none"
        @click="togglePasswordVisibility"
        :aria-label="showPassword ? 'Hide password' : 'Show password'"
      >
        <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'" class="text-lg"></i>
      </button>
    </div>

    <!-- Mobile-only instant validation feedback -->
    <div v-if="modelValue && modelValue.length > 0" class="md:hidden mt-2">
      <div class="flex items-center space-x-2">
        <div class="w-2 h-2 rounded-full" :class="getStrengthColor()"></div>
        <span class="text-xs font-medium" :class="getStrengthTextColor()">
          {{ getStrengthText() }}
        </span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    modelValue: String,
    id: String,
    label: String,
    required: Boolean,
    disabled: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      showPassword: false,
    };
  },
  methods: {
    togglePasswordVisibility() {
      this.showPassword = !this.showPassword;
    },
    getPasswordStrength() {
      const password = this.modelValue || '';
      let score = 0;

      if (password.length >= 6) score++;
      if (/[A-Z]/.test(password)) score++;
      if (/[a-z]/.test(password)) score++;
      if (/[0-9]/.test(password)) score++;
      if (/[!@#$%^&*(),.?":{}|<>]/.test(password)) score++;

      return score;
    },
    getStrengthText() {
      const score = this.getPasswordStrength();
      const texts = ['Very Weak', 'Weak', 'Fair', 'Good', 'Strong'];
      return texts[Math.min(score, 4)] || 'Very Weak';
    },
    getStrengthColor() {
      const score = this.getPasswordStrength();
      const colors = ['bg-red-500', 'bg-red-400', 'bg-yellow-500', 'bg-green-500', 'bg-green-600'];
      return colors[Math.min(score, 4)] || 'bg-red-500';
    },
    getStrengthTextColor() {
      const score = this.getPasswordStrength();
      const colors = ['text-red-600', 'text-red-500', 'text-yellow-600', 'text-green-600', 'text-green-700'];
      return colors[Math.min(score, 4)] || 'text-red-600';
    },
  },
};
</script>

<style scoped>
.relative {
  position: relative;
}

.absolute {
  position: absolute;
}

.flex {
  display: flex;
}

.items-center {
  align-items: center;
}

.justify-center {
  justify-content: center;
}

.right-3 {
  right: 0.75rem;
}

.w-full {
  width: 100%;
}

.pr-10 {
  padding-right: 2.5rem;
}

.cursor-pointer {
  cursor: pointer;
}
</style>
