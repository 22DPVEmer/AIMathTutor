<template>
  <div class="mb-3">
    <label :for="id" class="form-label"
      >{{ label }} <span class="text-danger" v-if="required">*</span></label
    >
    <div class="relative flex items-center">
      <input
        :type="showPassword ? 'text' : 'password'"
        :id="id"
        class="form-control w-full pr-10"
        :value="modelValue"
        @input="$emit('update:modelValue', $event.target.value)"
        :disabled="disabled"
      />
      <div
        class="absolute right-3 cursor-pointer flex items-center justify-center"
        @click="togglePasswordVisibility"
      >
        <i :class="showPassword ? 'bi bi-eye-slash' : 'bi bi-eye'"></i>
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
