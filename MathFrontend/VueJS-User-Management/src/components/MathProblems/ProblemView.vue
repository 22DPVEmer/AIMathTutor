<template>
  <div class="problem-view">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold">{{ topicName }}</h2>
      <div class="flex gap-2">
        <button
          @click="goBackToProblems"
          class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors flex items-center shadow-md"
        >
          <span class="mr-2 font-bold">←</span> Back to Problems
        </button>
        <button
          v-if="hasNextProblem"
          @click="goToNextProblem"
          @mousedown="console.log('Next button mousedown')"
          class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors flex items-center shadow-md"
        >
          Next Problem <span class="ml-1"> →</span>
        </button>
      </div>
    </div>

    <!-- Loading Indicator -->
    <div v-if="loading" class="text-center py-8">
      <div
        class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"
      ></div>
      <p class="mt-2 text-gray-600">Loading problem...</p>
    </div>

    <!-- Problem Display -->
    <div
      v-else-if="problem"
      class="bg-white shadow-md rounded-lg overflow-hidden"
    >
      <!-- Task/Assignment Header - Before Evaluation -->
      <div v-if="!evaluation" class="bg-blue-600 text-white p-4 w-full">
        <div class="flex justify-between items-center w-full">
          <h3 class="text-xl font-bold">{{ problem.name }}</h3>
          <span class="font-bold">{{ problem.pointValue || 1 }} p.</span>
        </div>
      </div>

      <!-- Results Header - After Evaluation -->
      <div v-if="evaluation" class="bg-blue-600 text-white p-4 w-full">
        <div class="flex justify-between items-center w-full">
          <h3 class="text-xl font-bold">Results:</h3>
          <span class="font-bold"
            >{{ evaluation.pointsEarned }} / {{ evaluation.maxPoints }} p.</span
          >
        </div>
      </div>

      <!-- Results Details - After Evaluation -->
      <div v-if="evaluation" class="border-b border-gray-300 p-4">
        <div class="flex items-center">
          <span v-if="evaluation.isCorrect" class="text-green-600 mr-2">✓</span>
          <span v-else class="text-red-600 mr-2">✗</span>
          <span class="font-medium">
            {{
              evaluation.isCorrect
                ? "Problem solved correctly!"
                : "Problem solved incorrectly, try again!"
            }}
          </span>
        </div>
        <div
          v-if="evaluation.isCorrect"
          class="flex items-center text-green-600 mt-2"
        >
          <span class="mr-1">✓</span>
          <span>Maximum points earned</span>
          <span class="ml-1 text-gray-400">ⓘ</span>
        </div>
      </div>

      <div class="p-6">
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
          <div class="p-4 rounded-md" v-html="formattedStatement"></div>
        </div>

        <div class="mb-4">
          <label for="answer" class="block mb-2 font-medium"
            >Your Answer:</label
          >
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
            class="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700"
            :disabled="!userAnswer || isChecking"
          >
            <span v-if="isChecking">Checking...</span>
            <span v-else>Check Answer</span>
          </button>

          <button
            v-if="evaluation || solutionVisible"
            @click="showSolution"
            class="bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700 flex items-center"
          >
            <span>{{
              solutionVisible ? "Hide Solution" : "Show Solution"
            }}</span>
            <svg
              class="ml-1"
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            >
              <polyline
                points="6 9 12 15 18 9"
                v-if="!solutionVisible"
              ></polyline>
              <polyline points="18 15 12 9 6 15" v-else></polyline>
            </svg>
          </button>
        </div>

        <div
          v-if="evaluation && evaluation.feedback"
          class="mt-6 p-4 rounded-md"
          :class="evaluation.isCorrect ? 'bg-green-50' : 'bg-red-50'"
        >
          <h4 class="font-semibold mb-2">Feedback:</h4>
          <p>{{ evaluation.feedback }}</p>
        </div>
      </div>

      <!-- Solution Section -->
      <div v-if="solutionVisible">
        <div class="bg-green-600 text-white p-4 w-full">
          <div class="flex justify-between items-center w-full">
            <h3 class="text-xl font-bold">Solution:</h3>
          </div>
        </div>
        <div class="border-b border-gray-300 p-4">
          <div
            class="p-4 bg-gray-50 rounded-md"
            v-html="formattedSolution"
          ></div>

          <h4 class="font-semibold mt-4 mb-2">Explanation:</h4>
          <div
            class="p-4 bg-gray-50 rounded-md"
            v-html="formattedExplanation"
          ></div>
        </div>
      </div>

      <!-- Ask for Guidance section -->
      <div v-if="evaluation">
        <div class="bg-green-600 text-white p-4 w-full">
          <div class="flex justify-between items-center w-full">
            <h3 class="text-xl font-bold">Need Additional Help?</h3>
          </div>
        </div>
        <div class="border-b border-gray-300 p-4">
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
    </div>

    <!-- No Problem Found Message -->
    <div v-if="!loading && !problem" class="text-center py-8 text-gray-500">
      Problem not found. Please go back and select a different problem.
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import { api } from "@/api/user";
import { getGuidance, evaluateAndSave } from "@/api/math";

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
    const route = useRoute();
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
    const topicProblems = ref([]);
    const currentProblemIndex = ref(-1);
    const hasNextProblem = computed(() => {
      const result =
        currentProblemIndex.value >= 0 &&
        currentProblemIndex.value < topicProblems.value.length - 1;
      console.log(
        "hasNextProblem:",
        result,
        "currentIndex:",
        currentProblemIndex.value,
        "total problems:",
        topicProblems.value.length
      );
      return result;
    });

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

    // Helper function to get difficulty value for sorting
    function getDifficultyValue(difficulty) {
      if (typeof difficulty === "string") {
        switch (difficulty.toLowerCase()) {
          case "easy":
            return 1;
          case "medium":
            return 2;
          case "hard":
            return 3;
          default:
            return 0;
        }
      } else {
        return difficulty || 0;
      }
    }

    async function fetchProblem() {
      loading.value = true;
      // Reset user input and evaluation when loading a new problem
      userAnswer.value = "";
      evaluation.value = null;
      solutionVisible.value = false;
      guidance.value = null;
      guidanceQuestion.value = "";

      try {
        // Fetch topic details to get the name
        const topicResponse = await api.get(`/mathtopic/${props.topicId}`);
        topicName.value = topicResponse.data.name;

        // Fetch all problems for this topic
        const problemsResponse = await api.get(
          `/mathproblem/topic/${props.topicId}`
        );
        console.log("Problems fetched:", problemsResponse.data.length);

        // Sort problems by difficulty
        const sortedProblems = problemsResponse.data.sort((a, b) => {
          return (
            getDifficultyValue(a.difficulty) - getDifficultyValue(b.difficulty)
          );
        });
        topicProblems.value = sortedProblems;
        console.log(
          "Problems sorted by difficulty:",
          topicProblems.value.map((p) => ({
            id: p.id,
            difficulty: p.difficulty,
          }))
        );

        // Debug problem ID types
        topicProblems.value.forEach((p, i) => {
          console.log(`Problem ${i}: id=${p.id} (${typeof p.id})`);
        });
        console.log(
          `Current problemId from route: ${props.problemId} (${typeof props.problemId})`
        );

        // Find the index of the current problem in the list
        currentProblemIndex.value = topicProblems.value.findIndex(
          (p) => String(p.id) === String(props.problemId)
        );
        console.log(
          "Current problem ID:",
          props.problemId,
          "Found at index:",
          currentProblemIndex.value
        );

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
          pointsEarned: result.isCorrect ? problem.value.pointValue || 1 : 0,
          maxPoints: problem.value.pointValue || 1,
        };

        if (result.hasExistingCorrectAttempt) {
          evaluation.value.feedback +=
            "\n\nNote: You've already correctly solved this problem before.";
        }

        if (!result.isCorrect) {
          solutionVisible.value = true;
        }

        if (problem.value) {
          problem.value.attempted = true;

          if (result.isCorrect || result.hasExistingCorrectAttempt) {
            problem.value.completed = true;
            problem.value.pointsEarned = problem.value.pointValue || 1;
          }
        }
      } catch (error) {
        console.error("Error checking answer:", error);

        alert("Failed to check answer. Please try again.");
      } finally {
        isChecking.value = false;
      }
    }

    function showSolution() {
      // Toggle solution visibility
      solutionVisible.value = !solutionVisible.value;
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

    function goToNextProblem() {
      console.log("goToNextProblem called");
      console.log("hasNextProblem:", hasNextProblem.value);
      console.log("currentProblemIndex:", currentProblemIndex.value);
      console.log("topicProblems length:", topicProblems.value.length);

      if (!hasNextProblem.value) {
        console.log("No next problem available");
        return;
      }

      const nextProblem = topicProblems.value[currentProblemIndex.value + 1];
      console.log("Next problem:", nextProblem);

      if (nextProblem) {
        console.log("Navigating to next problem ID:", nextProblem.id);

        // Force navigation with replacement to ensure a clean state
        router.replace({
          name: "ProblemView",
          params: {
            topicId: String(props.topicId),
            problemId: String(nextProblem.id),
          },
        });
      } else {
        console.log("No next problem found, going back to problems list");
        // If no next problem, go back to problems list
        goBackToProblems();
      }
    }

    onMounted(() => {
      console.log("Component mounted, fetching problem");
      fetchProblem();
    });

    // Watch for route parameter changes to reload problem when navigating
    watch(
      () => route.params.problemId,
      (newProblemId, oldProblemId) => {
        if (newProblemId !== oldProblemId) {
          console.log(
            `Route changed: problemId changed from ${oldProblemId} to ${newProblemId}`
          );
          fetchProblem();
        }
      }
    );

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
      goToNextProblem,
      hasNextProblem,
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
