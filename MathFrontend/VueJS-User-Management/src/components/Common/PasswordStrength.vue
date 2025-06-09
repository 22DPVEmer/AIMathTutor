<template>
  <div class="space-y-4 w-full">
    <!-- Strength Indicator -->
    <div class="bg-white rounded-lg p-4 shadow-sm border border-gray-100">
      <div class="flex justify-between items-center mb-2">
        <div class="flex items-center space-x-2">
          <div class="w-2 h-2 rounded-full" :class="strengthDotClass"></div>
          <span class="text-sm font-semibold" :class="strengthTextClass">{{ strengthText }}</span>
        </div>
        <span class="text-xs font-medium px-2 py-1 rounded-full" 
              :class="strengthBadgeClass">
          {{ strengthPercentage }}%
        </span>
      </div>
      
      <!-- Strength Bar -->
      <div class="h-2 bg-gray-100 rounded-full overflow-hidden">
        <div
          class="h-full transition-all duration-500 ease-out rounded-full"
          :class="strengthBarClass"
          :style="{ width: `${strengthPercentage}%` }"
        ></div>
      </div>
    </div>

    <!-- Requirements List - Hidden on mobile, shown on desktop/tablet -->
    <div class="hidden md:block bg-white rounded-lg shadow-sm border border-gray-100 p-4">
      <h6 class="text-sm font-semibold text-gray-700 mb-3 flex items-center">
        <svg class="w-2 h-2 mr-2 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
        </svg>
        Password Requirements
      </h6>
      <div class="space-y-3">
        <div v-for="(isValid, type) in passwordValidation" :key="type"
             class="flex items-center space-x-3 p-2 rounded-lg transition-colors duration-200"
             :class="isValid ? 'bg-green-50' : 'bg-gray-50'">
          <div class="flex-shrink-0">
            <svg v-if="isValid" class="h-5 w-5 text-green-500" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
            <svg v-else class="h-5 w-5 text-gray-400" viewBox="0 0 20 20" fill="currentColor">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
            </svg>
          </div>
          <span class="text-sm" :class="isValid ? 'text-green-700 font-medium' : 'text-gray-600'">
            {{ getRequirementText(type) }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PasswordStrength',
  props: {
    password: {
      type: String,
      required: true
    }
  },
  data() {
    return {
      passwordValidation: {
        length: false,
        uppercase: false,
        lowercase: false,
        number: false,
        special: false
      }
    }
  },
  computed: {
    strengthScore() {
      const validations = Object.values(this.passwordValidation);
      return validations.filter(v => v).length;
    },
    strengthPercentage() {
      return Math.min(100, (this.strengthScore / 5) * 100);
    },
    strengthText() {
      if (this.password === '') return 'Empty';
      const scores = ['Very Weak', 'Weak', 'Fair', 'Good', 'Strong'];
      return scores[Math.min(this.strengthScore, 4)];
    },
    strengthTextClass() {
      const classes = {
        'Empty': 'text-gray-400',
        'Very Weak': 'text-red-600',
        'Weak': 'text-red-600',
        'Fair': 'text-yellow-600',
        'Good': 'text-green-600',
        'Strong': 'text-green-700'
      };
      return classes[this.strengthText];
    },
    strengthBarClass() {
      const classes = {
        'Empty': 'bg-gray-200',
        'Very Weak': 'bg-gradient-to-r from-red-500 to-red-600',
        'Weak': 'bg-gradient-to-r from-red-400 to-red-500',
        'Fair': 'bg-gradient-to-r from-yellow-400 to-yellow-500',
        'Good': 'bg-gradient-to-r from-green-400 to-green-500',
        'Strong': 'bg-gradient-to-r from-green-500 to-green-600'
      };
      return classes[this.strengthText];
    },
    strengthBadgeClass() {
      const classes = {
        'Empty': 'bg-gray-100 text-gray-600',
        'Very Weak': 'bg-red-100 text-red-700',
        'Weak': 'bg-red-100 text-red-700',
        'Fair': 'bg-yellow-100 text-yellow-700',
        'Good': 'bg-green-100 text-green-700',
        'Strong': 'bg-green-100 text-green-800'
      };
      return classes[this.strengthText];
    },
    strengthDotClass() {
      const classes = {
        'Empty': 'bg-gray-400',
        'Very Weak': 'bg-red-500',
        'Weak': 'bg-red-500',
        'Fair': 'bg-yellow-500',
        'Good': 'bg-green-500',
        'Strong': 'bg-green-600'
      };
      return classes[this.strengthText];
    }
  },
  methods: {
    validatePassword() {
      const password = this.password;
      this.passwordValidation = {
        length: password.length >= 6,
        uppercase: /[A-Z]/.test(password),
        lowercase: /[a-z]/.test(password),
        number: /[0-9]/.test(password),
        special: /[!@#$%^&*(),.?":{}|<>]/.test(password)
      };
    },
    getRequirementText(type) {
      const texts = {
        length: 'At least 6 characters',
        uppercase: 'One uppercase letter',
        lowercase: 'One lowercase letter',
        number: 'One number',
        special: 'One special character'
      };
      return texts[type];
    }
  },
  watch: {
    password: {
      handler() {
        this.validatePassword();
      },
      immediate: true
    }
  }
};
</script>

<style scoped>
.strength-bar-transition {
  transition: all 0.3s ease-in-out;
}
</style> 