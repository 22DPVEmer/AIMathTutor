<template>
  <div class="my-problems-view p-6">
    <h1 class="text-3xl font-bold mb-6">My Math Problems</h1>

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

        <h3 class="text-lg font-bold mt-2 mb-1">{{ problem.topicName }}</h3>
        <div
          class="mb-4 p-4 bg-gray-50 rounded-md"
          v-html="formatText(problem.statement)"
        ></div>

        <div class="mb-4">
          <div class="flex items-center">
            <span class="font-semibold mr-2">Your Answer:</span>
            <span>{{ problem.userAnswer }}</span>
          </div>

          <div class="flex items-center mt-2">
            <span class="font-semibold mr-2">Correct Solution:</span>
            <span>{{ problem.solution }}</span>
          </div>
        </div>

        <div class="mb-4">
          <h4 class="font-semibold mb-2">Explanation:</h4>
          <div
            class="p-4 bg-gray-50 rounded-md"
            v-html="formatText(problem.explanation)"
          ></div>
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

          <div v-else class="border-t pt-4 mt-4">
            <h4 class="font-semibold mb-3">Try Again:</h4>
            <div class="mb-4">
              <input
                v-model="problem.retryAnswer"
                class="w-full p-2 border rounded-md"
                placeholder="Enter your new answer"
              />
            </div>

            <div class="flex gap-2">
              <button
                @click="submitRetry(problem)"
                class="bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700"
                :disabled="!problem.retryAnswer || problem.isSubmitting"
              >
                <span v-if="problem.isSubmitting">Submitting...</span>
                <span v-else>Submit</span>
              </button>

              <button
                @click="cancelRetry(problem)"
                class="bg-gray-300 text-gray-700 py-2 px-4 rounded-md hover:bg-gray-400"
                :disabled="problem.isSubmitting"
              >
                Cancel
              </button>
            </div>

            <div
              v-if="problem.feedback"
              class="mt-4 p-4 rounded-md"
              :class="problem.isCorrect ? 'bg-green-50' : 'bg-red-50'"
            >
              <h4 class="font-semibold mb-2">Feedback:</h4>
              <p>{{ problem.feedback }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Problem Dialog -->
    <div
      v-if="editingProblem"
      class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50"
    >
      <div
        class="bg-white rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-y-auto"
      >
        <div class="p-6">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-2xl font-bold">Edit Math Problem</h2>
            <button
              @click="cancelEdit"
              class="text-gray-500 hover:text-gray-700"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-6 w-6"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>

          <div class="grid grid-cols-1 gap-6">
            <div>
              <label for="topicName" class="block mb-2 font-medium"
                >Topic</label
              >
              <div class="flex gap-2">
                <select
                  id="topicName"
                  v-model="editedProblem.topicId"
                  class="flex-grow p-2 border rounded-md"
                >
                  <option
                    v-for="topic in topics"
                    :key="topic.id"
                    :value="topic.id"
                  >
                    {{ topic.name }}
                  </option>
                </select>
              </div>
            </div>

            <div>
              <label for="statement" class="block mb-2 font-medium"
                >Problem Statement</label
              >
              <textarea
                id="statement"
                v-model="editedProblem.statement"
                rows="4"
                class="w-full p-2 border rounded-md"
                placeholder="Enter the problem statement"
              ></textarea>
            </div>

            <div>
              <label for="solution" class="block mb-2 font-medium"
                >Solution</label
              >
              <input
                id="solution"
                v-model="editedProblem.solution"
                class="w-full p-2 border rounded-md"
                placeholder="Enter the correct solution"
              />
            </div>

            <div>
              <label for="explanation" class="block mb-2 font-medium"
                >Explanation</label
              >
              <textarea
                id="explanation"
                v-model="editedProblem.explanation"
                rows="4"
                class="w-full p-2 border rounded-md"
                placeholder="Enter the explanation"
              ></textarea>
            </div>

            <div>
              <label class="block mb-2 font-medium">Correct?</label>
              <div class="flex gap-4">
                <label class="inline-flex items-center">
                  <input
                    type="radio"
                    v-model="editedProblem.isCorrect"
                    :value="true"
                    class="form-radio"
                  />
                  <span class="ml-2">Correct</span>
                </label>
                <label class="inline-flex items-center">
                  <input
                    type="radio"
                    v-model="editedProblem.isCorrect"
                    :value="false"
                    class="form-radio"
                  />
                  <span class="ml-2">Incorrect</span>
                </label>
              </div>
            </div>
          </div>

          <div class="flex justify-end gap-2 mt-6">
            <button
              @click="cancelEdit"
              class="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-100"
            >
              Cancel
            </button>
            <button
              @click="saveProblem"
              class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
              :disabled="isSaving"
            >
              {{ isSaving ? "Saving..." : "Save Changes" }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import {
  getCurrentUserMathProblems,
  retryUserMathProblem,
  updateUserMathProblem,
  getAllTopics,
} from "@/api/math";
import { useStore } from "@/store";

export default {
  name: "MyProblemsView",

  setup() {
    const store = useStore();
    const problems = ref([]);
    const isLoading = ref(true);
    const topics = ref([]);
    const editingProblem = ref(false);
    const editedProblem = ref({});
    const isSaving = ref(false);

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

    // Retry functions
    const startRetry = (problem) => {
      problem.isRetrying = true;
      problem.retryAnswer = "";
      problem.feedback = "";
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

        // Update the problem with the new data
        const index = problems.value.findIndex((p) => p.id === problem.id);
        if (index !== -1) {
          const updatedProblem = {
            ...response.problem,
            isRetrying: problem.isCorrect ? false : true,
            retryAnswer: "",
            isSubmitting: false,
            feedback: response.feedback,
          };

          problems.value[index] = updatedProblem;
        }
      } catch (error) {
        console.error("Error retrying problem:", error);
        problem.feedback = "Failed to submit your answer. Please try again.";
      } finally {
        problem.isSubmitting = false;
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

    const saveProblem = async () => {
      isSaving.value = true;

      try {
        await updateUserMathProblem(
          editedProblem.value.id,
          editedProblem.value
        );

        // Update the problem in the list
        const index = problems.value.findIndex(
          (p) => p.id === editedProblem.value.id
        );
        if (index !== -1) {
          // Update topic name based on selected topic ID
          const selectedTopic = topics.value.find(
            (t) => t.id === editedProblem.value.topicId
          );
          if (selectedTopic) {
            editedProblem.value.topicName = selectedTopic.name;
          }

          problems.value[index] = {
            ...problems.value[index],
            ...editedProblem.value,
          };
        }

        // Close the edit dialog
        editingProblem.value = false;
      } catch (error) {
        console.error("Error saving problem:", error);
        alert("Failed to save changes. Please try again.");
      } finally {
        isSaving.value = false;
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
      filters,
      availableTopics,
      filteredProblems,
      formatDate,
      formatText,
      startRetry,
      cancelRetry,
      submitRetry,
      isTeacherOrAdmin,
      editProblem,
      editingProblem,
      editedProblem,
      cancelEdit,
      saveProblem,
      isSaving,
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
