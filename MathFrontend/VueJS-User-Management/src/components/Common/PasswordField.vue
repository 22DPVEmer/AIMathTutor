<template>
  <div class="mb-3 position-relative">
    <label :for="id" class="form-label">{{ label }} <span class="text-danger" v-if="required">*</span></label>
    <input
        :type="showPassword ? 'text' : 'password'"
        :id="id"
        class="form-control"
        :value="modelValue"
        @input="$emit('update:modelValue', $event.target.value)"
        :disabled="disabled"
    />
    <button type="button" class="btn position-absolute end-0 translate-middle-y me-3 password-eye"
            @click="togglePasswordVisibility">
      <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
    </button>
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
  },
};
</script>
