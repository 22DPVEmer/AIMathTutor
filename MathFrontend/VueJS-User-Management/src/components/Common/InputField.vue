<template>
    <label :for="id" class="form-label">{{ label }} <span class="text-danger" v-if="required">*</span></label>
    <input
        :type="type"
        :id="id"
        :placeholder="placeholder"
        class="form-control"
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
