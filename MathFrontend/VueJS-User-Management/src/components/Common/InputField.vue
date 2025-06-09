<template>
  <div class="">
    <!-- Label with 8-12px spacing above input -->
    <label :for="id" class="block text-sm font-medium text-gray-700 mb-2">
      {{ label }}
      <span class="text-red-600" v-if="required">*</span>
    </label>
    <!-- Input optimized for mobile: 16px font prevents iOS zoom, 48px height for touch targets -->
    <input
        :type="type"
        :id="id"
        :placeholder="placeholder"
        class="mobile-optimized-input w-full h-12 px-4 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 disabled:bg-gray-100 disabled:cursor-not-allowed transition-colors"
        :value="modelValue"
        @input="$emit('update:modelValue', $event.target.value)"
        :disabled="disabled"
        autocomplete="off"
    />
  </div>
</template>

<script>
export default {
  props: {
    modelValue: String,
    type: {
      type: String,
      default: 'text',
    },
    col: {
      type: String,
      default: 'col-12',
    },
    id: String,
    label: String,
    placeholder: String,
    required: Boolean,
    disabled: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
