<template>
  <!-- Desktop: Full banner with image -->
  <div class="hidden md:flex md:w-1/2 bg-primary-600 text-white items-center justify-center p-8">
    <div class="max-w-md text-center">
      <img
        class="w-32 h-32 sm:w-40 sm:h-40 lg:w-48 lg:h-48 mx-auto rounded-lg mb-6 object-cover"
        loading="lazy"
        :src="imgSrc"
        :alt="altText"
      />
      <h2 class="text-2xl lg:text-3xl font-bold mb-4">{{ title }}</h2>
      <p class="text-base lg:text-lg opacity-90 leading-relaxed">{{ description }}</p>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    imgSrc: {
      type: String,
      default: '/src/assets/images/ai-math-tutor.png',
    },
    altText: {
      type: String,
      default: 'Math Tutor',
    },
    title: {
      type: String,
      default: 'Math Tutor',
    },
    description: {
      type: String,
      default: 'Experience the future of math tutoring with our innovative platform designed to simplify your math life.',
    },
  },
};
</script>

<style scoped>
/* You can add any specific styles if needed here */
</style>
