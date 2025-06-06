<template>
    <label :for="id" class="form-label text-sm md:text-base font-medium">{{ label }} <span class="text-danger" v-if="required">*</span></label>
    <input
        :type="type"
        :id="id"
        :placeholder="placeholder"
        class="form-control w-full px-3 py-3 md:py-2 text-base md:text-sm border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 disabled:bg-gray-100 disabled:cursor-not-allowed"
        :value="modelValue"
        @input="$emit('update:modelValue', $event.target.value)"
        :disabled="disabled"
        autocomplete="off"
    />
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
