<template>
  <div class="math-problem-view p-6 justify-between flex flex-col items-center">
    <h1 class="text-3xl font-bold mb-6">Math Problem Generator</h1>
    <p class="mb-6 text-gray-600">
      Generate personalized math problems on any topic with different difficulty
      levels. Test your knowledge and get instant feedback!
    </p>

    <MathProblemGenerator ref="problemGenerator" class="w-full" />
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
