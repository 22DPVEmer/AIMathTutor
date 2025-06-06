<template>
  <div class="math-problem-view p-4 md:p-6 justify-between flex flex-col items-center">
    <div class="text-center mb-6 md:mb-8">
      <h1 class="text-2xl md:text-3xl font-bold mb-4">Math Problem Generator</h1>
      <p class="text-sm md:text-base text-gray-600 max-w-2xl">
        Generate personalized math problems on any topic with different difficulty
        levels. Test your knowledge and get instant feedback!
      </p>
    </div>

    <MathProblemGenerator ref="problemGenerator" class="w-full max-w-4xl" />
  </div>
</template>

<script>
import MathProblemGenerator from "@/components/MathProblems/MathProblemGenerator.vue";
import { ref } from "vue";

export default {
  name: "MathProblemView",
  components: {
    MathProblemGenerator,
  },

  setup() {
    const problemGenerator = ref(null);

    return {
      problemGenerator,
    };
  },
};
</script>

<style scoped>
.math-problem-view {
  max-width: 1200px;
  margin: 0 auto;
}
</style>
