<template>
  <div class="problem-view">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold">{{ topicName }}</h2>
      <button
        @click="goBackToProblems"
        class="px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 transition-colors flex items-center"
      >
        <span class="mr-1">←</span> Back to Problems
      </button>
    </div>

    <!-- Loading Indicator -->
    <div v-if="loading" class="text-center py-8">
      <div
        class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"
      ></div>
      <p class="mt-2 text-gray-600">Loading problem...</p>
    </div>

    <!-- Problem Display -->
    <div v-else-if="problem" class="bg-white shadow-md rounded-lg p-6">
      <div class="flex justify-between items-center mb-4">
        <h3 class="text-xl font-bold">Problem</h3>
        <span
          class="px-3 py-1 rounded-full text-sm"
          :class="getDifficultyClass(problem.difficulty)"
        >
          {{ getDifficultyLabel(problem.difficulty) }}
        </span>
      </div>

      <div class="mb-6">
        <h4 class="font-semibold mb-2">Problem Statement:</h4>
        <div
          class="p-4 bg-gray-50 rounded-md"
          v-html="formattedStatement"
        ></div>
      </div>

      <div class="mb-4">
        <label for="answer" class="block mb-2 font-medium">Your Answer:</label>
        <input
          id="answer"
          v-model="userAnswer"
          class="w-full p-2 border rounded-md"
          placeholder="Enter your answer here"
        />
      </div>

      <div class="flex flex-wrap gap-2 mb-4">
        <button
          @click="checkAnswer"
          class="bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700"
          :disabled="!userAnswer || isChecking"
        >
          <span v-if="isChecking">Checking...</span>
          <span v-else>Check Answer</span>
        </button>
      </div>

      <div
        v-if="evaluation"
        class="mt-6 p-4 rounded-md"
        :class="evaluation.isCorrect ? 'bg-green-50' : 'bg-red-50'"
      >
        <h4 class="font-semibold mb-2">Feedback:</h4>
        <p>{{ evaluation.feedback }}</p>
      </div>

      <div v-if="solutionVisible" class="mt-6">
        <h4 class="font-semibold mb-2">Solution:</h4>
        <div class="p-4 bg-gray-50 rounded-md" v-html="formattedSolution"></div>

        <h4 class="font-semibold mt-4 mb-2">Explanation:</h4>
        <div
          class="p-4 bg-gray-50 rounded-md"
          v-html="formattedExplanation"
        ></div>
      </div>

      <!-- Ask for Guidance section -->
      <div v-if="evaluation" class="mt-6 border-t pt-6">
        <h4 class="font-semibold mb-4">Need Additional Help?</h4>
        <div class="mb-4">
          <textarea
            v-model="guidanceQuestion"
            class="w-full p-3 border rounded-md"
            rows="3"
            placeholder="Ask a specific question about this problem or request additional guidance..."
          ></textarea>
        </div>
        <div class="flex gap-2">
          <button
            @click="askForGuidance"
            class="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700"
            :disabled="!guidanceQuestion || isRequestingGuidance"
          >
            <span v-if="isRequestingGuidance">Requesting...</span>
            <span v-else>Ask for Guidance</span>
          </button>
        </div>

        <div v-if="guidance" class="mt-4 p-4 bg-blue-50 rounded-md">
          <h5 class="font-semibold mb-2">Guidance:</h5>
          <div v-html="formattedGuidance"></div>
        </div>
      </div>
    </div>

    <!-- No Problem Found Message -->
    <div v-else class="text-center py-8 text-gray-500">
      Problem not found. Please go back and select a different problem.
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { api } from "@/api/user";
import {
  evaluateMathAnswer,
  saveProblemAttempt,
  getGuidance,
  evaluateAndSave,
} from "@/api/math";

export default {
  name: "ProblemView",
  props: {
    topicId: {
      type: [Number, String],
      required: true,
    },
    problemId: {
      type: [Number, String],
      required: true,
    },
  },

  setup(props) {
    const router = useRouter();
    const loading = ref(true);
    const problem = ref(null);
    const topicName = ref("");
    const userAnswer = ref("");
    const evaluation = ref(null);
    const isChecking = ref(false);
    const solutionVisible = ref(false);
    const guidanceQuestion = ref("");
    const guidance = ref(null);
    const isRequestingGuidance = ref(false);

    const formattedStatement = computed(() => {
      return problem.value?.statement.replace(/\n/g, "<br>");
    });

    const formattedSolution = computed(() => {
      return problem.value?.solution.replace(/\n/g, "<br>");
    });

    const formattedExplanation = computed(() => {
      return problem.value?.explanation.replace(/\n/g, "<br>");
    });

    const formattedGuidance = computed(() => {
      return guidance.value?.replace(/\n/g, "<br>");
    });

    async function fetchProblem() {
      loading.value = true;
      try {
        // Fetch topic details to get the name
        const topicResponse = await api.get(`/mathtopic/${props.topicId}`);
        topicName.value = topicResponse.data.name;

        // Fetch the specific problem
        const response = await api.get(`/mathproblem/${props.problemId}`);
        problem.value = response.data;
      } catch (error) {
        console.error("Error fetching problem:", error);
        problem.value = null;
      } finally {
        loading.value = false;
      }
    }

    function getDifficultyClass(level) {
      if (typeof level === "string") {
        switch (level.toLowerCase()) {
          case "easy":
            return "bg-green-100 text-green-800";
          case "medium":
            return "bg-yellow-100 text-yellow-800";
          case "hard":
            return "bg-red-100 text-red-800";
          default:
            return "bg-gray-100 text-gray-800";
        }
      } else {
        switch (level) {
          case 1:
            return "bg-green-100 text-green-800";
          case 2:
            return "bg-yellow-100 text-yellow-800";
          case 3:
            return "bg-red-100 text-red-800";
          default:
            return "bg-gray-100 text-gray-800";
        }
      }
    }

    function getDifficultyLabel(level) {
      if (typeof level === "string") {
        return level.charAt(0).toUpperCase() + level.slice(1);
      } else {
        switch (level) {
          case 1:
            return "Easy";
          case 2:
            return "Medium";
          case 3:
            return "Hard";
          default:
            return "Unknown";
        }
      }
    }

    async function checkAnswer() {
      if (!userAnswer.value) {
        alert("Please enter your answer");
        return;
      }

      isChecking.value = true;

      try {
        // Convert difficulty to string if it's a number
        const difficultyStr =
          typeof problem.value.difficulty === "string"
            ? problem.value.difficulty
            : getDifficultyLabel(problem.value.difficulty);

        // Use the combined evaluate-and-save endpoint for regular math section problems
        // This ensures attempts are properly tracked and saved in MathProblemAttempts
        const result = await evaluateAndSave({
          problem: problem.value.statement,
          userAnswer: userAnswer.value,
          solution: problem.value.solution,
          explanation: problem.value.explanation,
          name: problem.value.name || `${topicName.value} Problem`,
          difficulty: difficultyStr,
          topic: topicName.value,
          topicId: problem.value.topicId,
        });

        // Set the evaluation result
        evaluation.value = {
          isCorrect: result.isCorrect,
          feedback: result.feedback,
        };

        // If the user already had a correct attempt, add a note to the feedback
        if (result.hasExistingCorrectAttempt) {
          evaluation.value.feedback +=
            "\n\nNote: You've already correctly solved this problem before.";
        }

        // Automatically show solution if answer is incorrect
        if (!result.isCorrect) {
          solutionVisible.value = true;
        }

        // Mark the problem as attempted regardless of correctness
        if (problem.value) {
          problem.value.attempted = true;

          // If the answer is correct or there was an existing correct attempt,
          // update the completed status and points earned
          if (result.isCorrect || result.hasExistingCorrectAttempt) {
            problem.value.completed = true;
            problem.value.pointsEarned = problem.value.pointValue || 1;
          }
        }

        // If we received updated problems and attempts, we could update the UI here
        // This would require state management, which is beyond the scope of this change
      } catch (error) {
        console.error("Error checking answer:", error);

        // Fallback to the old approach if the combined endpoint fails
        try {
          console.log("Falling back to separate evaluate and save calls");

          // First, evaluate the answer
          evaluation.value = await evaluateMathAnswer({
            problem: problem.value.statement,
            userAnswer: userAnswer.value,
          });

          // Automatically show solution if answer is incorrect
          if (!evaluation.value.isCorrect) {
            solutionVisible.value = true;
          }

          // Mark the problem as attempted regardless of correctness
          if (problem.value) {
            problem.value.attempted = true;
          }

          // Save the attempt to create a MathProblemAttempt entity
          if (problem.value.id) {
            // Convert difficulty to string if it's a number
            const difficultyStr =
              typeof problem.value.difficulty === "string"
                ? problem.value.difficulty
                : getDifficultyLabel(problem.value.difficulty);

            // If the answer is correct, update the completed status and points earned
            if (evaluation.value.isCorrect) {
              problem.value.completed = true;
              problem.value.pointsEarned = problem.value.pointValue || 1;
            }

            await saveProblemAttempt({
              name: problem.value.name || `${topicName.value} Problem`,
              statement: problem.value.statement,
              solution: problem.value.solution,
              explanation: problem.value.explanation,
              userAnswer: userAnswer.value,
              isCorrect: evaluation.value.isCorrect,
              difficulty: difficultyStr,
              topic: topicName.value,
              topicId: problem.value.topicId,
            });
          }
        } catch (fallbackError) {
          console.error("Fallback also failed:", fallbackError);
          alert("Failed to check answer. Please try again.");
        }
      } finally {
        isChecking.value = false;
      }
    }

    function showSolution() {
      solutionVisible.value = true;
    }

    async function askForGuidance() {
      if (!guidanceQuestion.value) {
        alert("Please enter your question");
        return;
      }

      isRequestingGuidance.value = true;

      try {
        console.log(
          "Asking for guidance with question:",
          guidanceQuestion.value
        );

        // Prepare the request data
        const requestData = {
          problem: problem.value.statement,
          solution: problem.value.solution,
          userAnswer: userAnswer.value || "",
          question: guidanceQuestion.value,
        };

        console.log("Guidance request data:", requestData);

        // Call the API to get guidance
        const response = await getGuidance(requestData);
        console.log("Guidance response received:", response);

        if (response && response.guidance) {
          guidance.value = response.guidance;
        } else {
          console.error("Invalid guidance response format:", response);
          alert("Received an invalid response format. Please try again.");
        }
      } catch (error) {
        console.error("Error getting guidance:", error);

        // More detailed error logging
        if (error.response) {
          console.error("Error status:", error.response.status);
          console.error("Error data:", error.response.data);
        }

        alert(
          "Failed to get guidance. Please try again. Error: " +
            (error.message || "Unknown error")
        );
      } finally {
        isRequestingGuidance.value = false;
      }
    }

    function goBackToProblems() {
      router.push({
        name: "TopicProblems",
        params: { topicId: props.topicId },
      });
    }

    onMounted(() => {
      fetchProblem();
    });

    return {
      loading,
      problem,
      topicName,
      userAnswer,
      evaluation,
      isChecking,
      solutionVisible,
      guidanceQuestion,
      guidance,
      isRequestingGuidance,
      formattedStatement,
      formattedSolution,
      formattedExplanation,
      formattedGuidance,
      getDifficultyClass,
      getDifficultyLabel,
      checkAnswer,
      showSolution,
      askForGuidance,
      goBackToProblems,
    };
  },
};
</script>

<style scoped>
.problem-view {
  max-width: 1000px;
  margin: 0 auto;
}
</style>
