<template>
  <div class="my-problems-view p-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold">My Math Problems</h1>
      <button
        @click="refreshProblems"
        class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 flex items-center"
        :disabled="isRefreshing"
      >
        <span v-if="isRefreshing">Refreshing...</span>
        <span v-else>Refresh</span>
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white shadow-md rounded-lg p-6 mb-6">
      <h2 class="text-xl font-bold mb-4">Filters</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label for="topicFilter" class="block mb-2 font-medium">Topic</label>
          <select
            id="topicFilter"
            v-model="filters.topic"
            class="w-full p-2 border rounded-md"
          >
            <option value="">All Topics</option>
            <option
              v-for="topic in availableTopics"
              :key="topic"
              :value="topic"
            >
              {{ topic }}
            </option>
          </select>
        </div>
        <div>
          <label for="statusFilter" class="block mb-2 font-medium"
            >Status</label
          >
          <select
            id="statusFilter"
            v-model="filters.status"
            class="w-full p-2 border rounded-md"
          >
            <option value="">All</option>
            <option value="correct">Correct</option>
            <option value="incorrect">Incorrect</option>
          </select>
        </div>
        <div>
          <label for="sortBy" class="block mb-2 font-medium">Sort By</label>
          <select
            id="sortBy"
            v-model="filters.sortBy"
            class="w-full p-2 border rounded-md"
          >
            <option value="newest">Newest First</option>
            <option value="oldest">Oldest First</option>
            <option value="topic">Topic</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Problems List -->
    <div v-if="isLoading" class="flex justify-center my-8">
      <div class="loader"></div>
    </div>

    <div
      v-else-if="filteredProblems.length === 0"
      class="bg-white shadow-md rounded-lg p-6 text-center"
    >
      <p class="text-gray-500">
        No problems found. Try generating some problems in the Math Generator!
      </p>
      <router-link
        to="/math-problems"
        class="inline-block mt-4 bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700"
      >
        Go to Math Generator
      </router-link>
    </div>

    <div v-else class="grid grid-cols-1 gap-6">
      <div
        v-for="problem in filteredProblems"
        :key="problem.id"
        class="bg-white shadow-md rounded-lg p-6 transition-all duration-200"
      >
        <div class="flex justify-between">
          <span class="text-sm text-gray-500">{{
            formatDate(problem.createdAt)
          }}</span>
          <div class="flex space-x-2">
            <span
              class="text-sm font-semibold px-3 py-1 rounded-full"
              :class="
                problem.isCorrect
                  ? 'bg-green-100 text-green-800'
                  : 'bg-red-100 text-red-800'
              "
            >
              {{ problem.isCorrect ? "Correct" : "Incorrect" }}
            </span>
            <button
              v-if="isTeacherOrAdmin === true"
              @click="editProblem(problem)"
              class="text-sm font-semibold px-3 py-1 rounded-full bg-blue-100 text-blue-800 hover:bg-blue-200"
            >
              Edit
            </button>
          </div>
        </div>

        <h3 class="text-lg font-bold mt-2 mb-1">
          {{ getProblemName(problem) }}
        </h3>
        <div class="text-sm text-gray-600 mb-2">{{ problem.topicName }}</div>
        <div
          class="mb-4 p-4 bg-gray-50 rounded-md"
          v-html="formatText(problem.statement)"
        ></div>

        <div class="mb-4">
          <div class="flex items-center">
            <span class="font-semibold mr-2">Your Previous Answer:</span>
            <span>{{ problem.userAnswer }}</span>
          </div>

          <!-- Only show solution and explanation if the answer is correct -->
          <div v-if="problem.isCorrect" class="mt-4">
            <div class="flex items-center mt-2">
              <span class="font-semibold mr-2">Correct Solution:</span>
              <span>{{ problem.solution }}</span>
            </div>

            <div class="mt-4">
              <h4 class="font-semibold mb-2">Explanation:</h4>
              <div
                class="p-4 bg-gray-50 rounded-md"
                v-html="formatText(problem.explanation)"
              ></div>
            </div>
          </div>
        </div>

        <!-- Retry section for incorrect answers -->
        <div v-if="!problem.isCorrect" class="mt-4">
          <button
            v-if="!problem.isRetrying"
            @click="startRetry(problem)"
            class="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 mr-2"
          >
            Retry This Problem
          </button>

          <div v-else class="mt-4 p-6 bg-white shadow-md rounded-lg">
            <div
              class="problem-header flex justify-between items-center mb-4 pb-2 border-b"
            >
              <h3 class="font-bold text-xl">Retry Problem</h3>
              <span
                class="px-3 py-1 bg-blue-100 text-blue-800 rounded-full text-sm font-semibold"
              >
                {{ problem.difficulty }}
              </span>
            </div>

            <div class="mb-4">
              <label
                for="retryAnswer"
                class="block mb-2 font-medium text-gray-700"
                >Your Answer:</label
              >
              <input
                id="retryAnswer"
                v-model="problem.retryAnswer"
                class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter your answer here"
                @keyup.enter="submitRetry(problem)"
              />
            </div>

            <div class="flex gap-3">
              <button
                @click="submitRetry(problem)"
                class="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 transition-colors"
                :disabled="!problem.retryAnswer || problem.isSubmitting"
              >
                <span v-if="problem.isSubmitting">Checking...</span>
                <span v-else>Check Answer</span>
              </button>

              <button
                @click="cancelRetry(problem)"
                class="bg-gray-200 text-gray-700 py-2 px-4 rounded-md hover:bg-gray-300 transition-colors"
                :disabled="problem.isSubmitting"
              >
                Cancel
              </button>
            </div>

            <div
              v-if="problem.feedback"
              class="mt-6 p-4 rounded-md"
              :class="{
                'bg-green-50': problem.isCorrect === true,
                'bg-red-50': problem.isCorrect === false,
                'bg-yellow-50': problem.isCorrect === undefined,
              }"
            >
              <h4 class="font-semibold mb-2">Feedback:</h4>
              <p>{{ problem.feedback }}</p>
            </div>

            <!-- Show solution only if the retry was correct -->
            <div v-if="problem.isCorrect" class="mt-6 solution-content">
              <div class="bg-green-600 text-white p-4 rounded-t-lg">
                <h3 class="text-xl font-bold">Solution:</h3>
              </div>
              <div class="border border-t-0 border-gray-300 p-4 rounded-b-lg">
                <div class="p-4 bg-gray-50 rounded-md">
                  {{ problem.solution }}
                </div>

                <h4 class="font-semibold mt-4 mb-2">Explanation:</h4>
                <div
                  class="p-4 bg-gray-50 rounded-md"
                  v-html="formatText(problem.explanation)"
                ></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Problem Dialog -->
    <EditMathProblem
      v-model:show="editingProblem"
      :problem="editedProblem"
      :topics="topics"
      :is-teacher-or-admin="isTeacherOrAdmin"
      @problem-saved="handleProblemSaved"
      @problem-published="handleProblemPublished"
      @cancel="cancelEdit"
    />
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import {
  getCurrentUserMathProblems,
  retryUserMathProblem,
  getAllTopics,
} from "@/api/math";
import { useStore } from "@/store";
import EditMathProblem from "@/components/EditMathProblem.vue";

export default {
  name: "MyProblemsView",
  components: {
    EditMathProblem,
  },

  setup() {
    const store = useStore();
    const problems = ref([]);
    const isLoading = ref(true);
    const isRefreshing = ref(false);
    const topics = ref([]);
    const editingProblem = ref(false);
    const editedProblem = ref({});

    // Define filters
    const filters = ref({
      topic: "",
      status: "",
      sortBy: "newest",
    });

    // Check if user is teacher or admin
    const isTeacherOrAdmin = computed(() => {
      console.log("Store:", store);
      const userData = store.getters["user/userData"];
      console.log("Store userData:", userData);
      return (
        userData &&
        userData.roles &&
        (userData.roles.includes("Teacher") || userData.roles.includes("Admin"))
      );
    });

    // Fetch user problems
    const fetchProblems = async () => {
      isLoading.value = true;
      try {
        const data = await getCurrentUserMathProblems();
        if (Array.isArray(data)) {
          problems.value = data.map((problem) => ({
            ...problem,
            isRetrying: false,
            retryAnswer: "",
            isSubmitting: false,
            feedback: "",
            // Initialize solutionVisible based on whether the answer is correct
            solutionVisible: problem.isCorrect,
          }));
        } else {
          console.error("Unexpected data format:", data);
          problems.value = [];
        }
      } catch (error) {
        console.error("Error fetching problems:", error);
        problems.value = [];
      } finally {
        isLoading.value = false;
      }
    };

    // Refresh problems (for manual refresh button)
    const refreshProblems = async () => {
      if (isRefreshing.value) return;

      isRefreshing.value = true;
      try {
        const data = await getCurrentUserMathProblems();
        if (Array.isArray(data)) {
          // Preserve retry state for problems that are currently being retried
          problems.value = data.map((newProblem) => {
            const existingProblem = problems.value.find(
              (p) => p.id === newProblem.id
            );
            if (existingProblem && existingProblem.isRetrying) {
              return {
                ...newProblem,
                isRetrying: true,
                retryAnswer: existingProblem.retryAnswer || "",
                isSubmitting: false,
                feedback: existingProblem.feedback || "",
                // Preserve solutionVisible state if it exists, otherwise set based on correctness
                solutionVisible:
                  existingProblem.solutionVisible !== undefined
                    ? existingProblem.solutionVisible
                    : newProblem.isCorrect,
              };
            }
            return {
              ...newProblem,
              isRetrying: false,
              retryAnswer: "",
              isSubmitting: false,
              feedback: "",
              // Initialize solutionVisible based on whether the answer is correct
              solutionVisible: newProblem.isCorrect,
            };
          });
        } else {
          console.error("Unexpected data format:", data);
        }
      } catch (error) {
        console.error("Error refreshing problems:", error);
      } finally {
        isRefreshing.value = false;
      }
    };

    // Fetch all topics
    const fetchTopics = async () => {
      try {
        topics.value = await getAllTopics();
      } catch (error) {
        console.error("Error fetching topics:", error);
      }
    };

    // Get list of available topics
    const availableTopics = computed(() => {
      const topicList = [
        ...new Set(problems.value.map((problem) => problem.topicName)),
      ];
      return topicList.sort();
    });

    // Filter and sort problems
    const filteredProblems = computed(() => {
      let filtered = [...problems.value];

      // Apply topic filter
      if (filters.value.topic) {
        filtered = filtered.filter(
          (problem) => problem.topicName === filters.value.topic
        );
      }

      // Apply status filter
      if (filters.value.status === "correct") {
        filtered = filtered.filter((problem) => problem.isCorrect);
      } else if (filters.value.status === "incorrect") {
        filtered = filtered.filter((problem) => !problem.isCorrect);
      }

      // Apply sorting
      if (filters.value.sortBy === "newest") {
        filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
      } else if (filters.value.sortBy === "oldest") {
        filtered.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt));
      } else if (filters.value.sortBy === "topic") {
        filtered.sort((a, b) => a.topicName.localeCompare(b.topicName));
      }

      return filtered;
    });

    // Format date
    const formatDate = (dateString) => {
      const date = new Date(dateString);
      return date.toLocaleDateString("en-US", {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
      });
    };

    // Format text (replace newlines with HTML line breaks)
    const formatText = (text) => {
      return text?.replace(/\n/g, "<br>") || "";
    };

    // Get problem name from localStorage or generate a default one
    const getProblemName = (problem) => {
      // Try to get the name from localStorage
      const savedName = localStorage.getItem(
        `userMathProblem_${problem.id}_name`
      );
      if (savedName) {
        return savedName;
      }

      // If no saved name, generate a default one
      return `${problem.topicName} Problem`;
    };

    // Retry functions
    const startRetry = (problem) => {
      // Reset the problem state for retry
      problem.isRetrying = true;
      problem.retryAnswer = "";
      problem.feedback = "";

      // Hide any previous solution or feedback
      if (!problem.isCorrect) {
        problem.solutionVisible = false;
      }
    };

    const cancelRetry = (problem) => {
      problem.isRetrying = false;
      problem.retryAnswer = "";
      problem.feedback = "";
    };

    const submitRetry = async (problem) => {
      problem.isSubmitting = true;
      problem.feedback = "";

      try {
        const response = await retryUserMathProblem(
          problem.id,
          problem.retryAnswer
        );

        console.log("Retry response:", response);

        // Update the problem with the new data
        const index = problems.value.findIndex((p) => p.id === problem.id);
        if (index !== -1 && response) {
          // Check if we have a response with isCorrect and feedback
          if (response.isCorrect !== undefined && response.feedback) {
            // Update the existing problem with the new data
            const updatedProblem = {
              ...problems.value[index],
              isCorrect: response.isCorrect,
              userAnswer: problem.retryAnswer,
              isRetrying: !response.isCorrect, // Keep retry mode open if answer is incorrect
              retryAnswer: "",
              isSubmitting: false,
              feedback: response.feedback,
              // Show solution only if the answer is correct
              solutionVisible: response.isCorrect,
            };

            // Replace the problem in the array
            problems.value[index] = updatedProblem;

            // If the answer is correct, refresh the problems list to get the updated data
            if (response.isCorrect) {
              setTimeout(() => {
                fetchProblems();
              }, 1000);
            }
          } else if (response.problem) {
            // Handle the case where the backend returns a complete problem object
            const updatedProblem = {
              ...response.problem,
              isRetrying: !response.isCorrect,
              retryAnswer: "",
              isSubmitting: false,
              feedback: response.feedback,
              // Show solution only if the answer is correct
              solutionVisible: response.isCorrect,
            };

            problems.value[index] = updatedProblem;

            // If the answer is correct, refresh the problems list to get the updated data
            if (response.isCorrect) {
              setTimeout(() => {
                fetchProblems();
              }, 1000);
            }
          } else {
            console.error("Invalid response format", response);
            problem.isSubmitting = false;
            problem.feedback = "Failed to update problem. Please try again.";
          }
        } else {
          console.error(
            "Problem not found in list or invalid response",
            response
          );
          problem.isSubmitting = false;
          problem.feedback = "Failed to update problem. Please try again.";
        }
      } catch (error) {
        console.error("Error retrying problem:", error);
        problem.feedback = "Failed to submit your answer. Please try again.";
        problem.isSubmitting = false;

        // Try to refresh the problems list after a short delay
        setTimeout(() => {
          fetchProblems();
        }, 2000);
      }
    };

    // Edit problem functions
    const editProblem = (problem) => {
      editedProblem.value = { ...problem };
      editingProblem.value = true;
    };

    const cancelEdit = () => {
      editingProblem.value = false;
      editedProblem.value = {};
    };

    // Handler for when a problem is saved in the EditMathProblem component
    const handleProblemSaved = (savedProblem) => {
      // Update the problem in the list
      const index = problems.value.findIndex((p) => p.id === savedProblem.id);
      if (index !== -1) {
        problems.value[index] = {
          ...problems.value[index],
          ...savedProblem,
        };
      }
    };

    // Handler for when a problem is published in the EditMathProblem component
    const handleProblemPublished = (publishedProblem) => {
      // Update the problem in the list
      const index = problems.value.findIndex(
        (p) => p.id === publishedProblem.id
      );
      if (index !== -1) {
        problems.value[index] = {
          ...problems.value[index],
          ...publishedProblem,
        };
      }
    };

    onMounted(async () => {
      try {
        // Make sure user profile is loaded
        await store.dispatch("user/getUserProfile");
      } catch (error) {
        console.error("Error loading user profile:", error);
      }

      // Load problems and topics
      fetchProblems();
      fetchTopics();
    });

    return {
      problems,
      isLoading,
      isRefreshing,
      refreshProblems,
      filters,
      availableTopics,
      filteredProblems,
      formatDate,
      formatText,
      getProblemName,
      startRetry,
      cancelRetry,
      submitRetry,
      isTeacherOrAdmin,
      editProblem,
      editingProblem,
      editedProblem,
      cancelEdit,
      handleProblemSaved,
      handleProblemPublished,
      topics,
    };
  },
};
</script>

<style scoped>
.my-problems-view {
  max-width: 1200px;
  margin: 0 auto;
}

.loader {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 2s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
