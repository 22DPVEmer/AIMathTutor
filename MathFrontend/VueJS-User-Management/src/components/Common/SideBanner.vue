<template>
  <div class="col-12 col-md-6 text-bg-primary d-flex align-items-center justify-content-center">
    <div class="col-10 col-xl-8 py-3">
      <img class="img-fluid rounded mb-4" loading="lazy" :src="imgSrc" width="200" height="200" :alt="altText" />
      <h2 class="h1 mb-4">{{ title }}</h2>
      <p class="lead m-0">{{ description }}</p>
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
