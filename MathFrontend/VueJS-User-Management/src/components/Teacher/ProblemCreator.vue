<template>
  <div class="problem-creator p-6 bg-white rounded-lg shadow">
    <h2 class="text-2xl font-bold mb-6">Create Math Problem</h2>

    <div
      v-if="successMessage"
      class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4"
    >
      {{ successMessage }}
    </div>

    <div
      v-if="errorMessage"
      class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4"
    >
      {{ errorMessage }}
    </div>

    <form @submit.prevent="submitProblem">
      <!-- Topic Selection -->
      <div class="mb-4">
        <label for="topic" class="block text-sm font-medium text-gray-700 mb-1"
          >Topic</label
        >
        <select
          id="topic"
          v-model="form.topicId"
          class="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          required
        >
          <option value="" disabled>Select a topic</option>
          <option
            v-for="category in categories"
            :key="'category-' + category.id"
            disabled
          >
            {{ category.name }}
          </option>
          <option
            v-for="topic in topics"
            :key="topic.id"
            :value="topic.id"
            :style="{ paddingLeft: getTopicIndent(topic) }"
          >
            {{ getTopicPrefix(topic) }} {{ topic.name }}
          </option>
        </select>
      </div>

      <!-- Problem Statement -->
      <div class="mb-4">
        <label
          for="statement"
          class="block text-sm font-medium text-gray-700 mb-1"
          >Problem Statement</label
        >
        <textarea
          id="statement"
          v-model="form.statement"
          rows="4"
          class="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Enter the math problem statement"
          required
        ></textarea>
      </div>

      <!-- Solution -->
      <div class="mb-4">
        <label
          for="solution"
          class="block text-sm font-medium text-gray-700 mb-1"
          >Solution</label
        >
        <textarea
          id="solution"
          v-model="form.solution"
          rows="2"
          class="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Enter the correct answer"
          required
        ></textarea>
      </div>

      <!-- Explanation -->
      <div class="mb-4">
        <label
          for="explanation"
          class="block text-sm font-medium text-gray-700 mb-1"
          >Explanation</label
        >
        <textarea
          id="explanation"
          v-model="form.explanation"
          rows="4"
          class="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Explain how to solve this problem step by step"
          required
        ></textarea>
      </div>

      <!-- Difficulty -->
      <div class="mb-4">
        <label class="block text-sm font-medium text-gray-700 mb-1"
          >Difficulty</label
        >
        <div class="flex space-x-4">
          <label
            v-for="option in difficulties"
            :key="option.value"
            class="flex items-center"
          >
            <input
              type="radio"
              :value="option.value"
              v-model="form.difficulty"
              class="mr-2"
              required
            />
            <span>{{ option.label }}</span>
          </label>
        </div>
      </div>

      <!-- Point Value -->
      <div class="mb-6">
        <label
          for="pointValue"
          class="block text-sm font-medium text-gray-700 mb-1"
          >Point Value (1-5)</label
        >
        <input
          id="pointValue"
          v-model.number="form.pointValue"
          type="number"
          min="1"
          max="5"
          class="w-full px-4 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          required
        />
      </div>

      <!-- Preview Section -->
      <div
        v-if="isPreviewVisible"
        class="mb-6 p-4 border rounded-md bg-gray-50"
      >
        <div class="flex justify-between items-center mb-4">
          <h3 class="font-bold">Problem Preview</h3>
          <span
            class="px-2 py-1 rounded-full text-xs"
            :class="getDifficultyClass(form.difficulty)"
          >
            {{ getDifficultyLabel(form.difficulty) }}
          </span>
        </div>

        <div class="mb-4">
          <p class="font-medium">Statement:</p>
          <p>{{ form.statement }}</p>
        </div>

        <div class="mb-4">
          <p class="font-medium">Solution:</p>
          <p>{{ form.solution }}</p>
        </div>

        <div>
          <p class="font-medium">Explanation:</p>
          <p>{{ form.explanation }}</p>
        </div>
      </div>

      <div class="flex justify-between">
        <button
          type="button"
          @click="togglePreview"
          class="px-4 py-2 bg-gray-200 text-gray-800 rounded-md hover:bg-gray-300"
        >
          {{ isPreviewVisible ? "Hide Preview" : "Show Preview" }}
        </button>

        <div>
          <button
            type="button"
            @click="resetForm"
            class="px-4 py-2 bg-gray-200 text-gray-800 rounded-md hover:bg-gray-300 mr-2"
          >
            Reset
          </button>

          <button
            type="submit"
            :disabled="isSubmitting"
            class="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
          >
            {{ isSubmitting ? "Submitting..." : "Publish Problem" }}
          </button>
        </div>
      </div>
    </form>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from "vue";
import axios from "axios";

export default {
  name: "ProblemCreator",

  setup() {
    const categories = ref([]);
    const topics = ref([]);
    const isSubmitting = ref(false);
    const isPreviewVisible = ref(false);
    const successMessage = ref("");
    const errorMessage = ref("");

    const form = reactive({
      topicId: "",
      statement: "",
      solution: "",
      explanation: "",
      difficulty: "medium",
      pointValue: 1,
    });

    const difficulties = [
      { value: "easy", label: "Easy" },
      { value: "medium", label: "Medium" },
      { value: "hard", label: "Hard" },
    ];

    // Fetch categories and topics on component mount
    onMounted(async () => {
      try {
        // Fetch categories
        const categoriesResponse = await axios.get(
          "http://localhost:5000/api/mathcategory"
        );
        categories.value = categoriesResponse.data;

        // Fetch all topics
        const topicsResponse = await axios.get(
          "http://localhost:5000/api/mathtopic"
        );
        topics.value = topicsResponse.data;

        // Sort topics by hierarchy
        organizeTopics();
      } catch (error) {
        console.error("Error fetching data:", error);
        errorMessage.value = "Failed to load categories and topics";
      }
    });

    // Function to organize topics for hierarchical display
    function organizeTopics() {
      // First sort by category ID
      topics.value.sort((a, b) => a.categoryId - b.categoryId);

      // Then ensure parent topics come before their children
      const parentTopics = topics.value.filter((t) => !t.parentTopicId);
      const childTopics = topics.value.filter((t) => t.parentTopicId);

      // Assign a depth level to each topic
      topics.value.forEach((topic) => {
        topic.depth = getTopicDepth(topic);
      });

      // Sort by depth
      topics.value.sort((a, b) => {
        if (a.categoryId !== b.categoryId) return a.categoryId - b.categoryId;
        return a.depth - b.depth;
      });
    }

    // Calculate topic depth in hierarchy
    function getTopicDepth(topic) {
      let depth = 0;
      let current = topic;

      while (current.parentTopicId) {
        depth++;
        current = topics.value.find((t) => t.id === current.parentTopicId) || {
          parentTopicId: null,
        };
      }

      return depth;
    }

    // Get indentation for topic based on depth
    function getTopicIndent(topic) {
      const depth = topic.depth || 0;
      return `${depth * 20}px`;
    }

    // Get prefix for topic name based on depth
    function getTopicPrefix(topic) {
      const depth = topic.depth || 0;
      return depth > 0 ? "└─".repeat(depth) : "";
    }

    function getDifficultyClass(level) {
      switch (level) {
        case "easy":
          return "bg-green-100 text-green-800";
        case "medium":
          return "bg-yellow-100 text-yellow-800";
        case "hard":
          return "bg-red-100 text-red-800";
        default:
          return "bg-gray-100 text-gray-800";
      }
    }

    function getDifficultyLabel(level) {
      return level.charAt(0).toUpperCase() + level.slice(1);
    }

    function togglePreview() {
      isPreviewVisible.value = !isPreviewVisible.value;
    }

    function resetForm() {
      Object.assign(form, {
        topicId: "",
        statement: "",
        solution: "",
        explanation: "",
        difficulty: "medium",
        pointValue: 1,
      });

      successMessage.value = "";
      errorMessage.value = "";
      isPreviewVisible.value = false;
    }

    async function submitProblem() {
      // Clear previous messages
      successMessage.value = "";
      errorMessage.value = "";

      // Validate form
      if (
        !form.topicId ||
        !form.statement ||
        !form.solution ||
        !form.explanation ||
        !form.difficulty ||
        !form.pointValue
      ) {
        errorMessage.value = "Please fill out all fields";
        return;
      }

      // Convert point value to a number
      const pointValue = parseInt(form.pointValue);
      if (isNaN(pointValue) || pointValue < 1 || pointValue > 5) {
        errorMessage.value = "Point value must be between 1 and 5";
        return;
      }

      isSubmitting.value = true;

      try {
        // Submit problem to API
        const response = await axios.post(
          "http://localhost:5000/api/mathproblem",
          {
            topicId: parseInt(form.topicId),
            statement: form.statement,
            solution: form.solution,
            explanation: form.explanation,
            difficulty: form.difficulty,
            pointValue: pointValue,
          }
        );

        successMessage.value = "Problem published successfully!";
        resetForm();
      } catch (error) {
        console.error("Error submitting problem:", error);
        errorMessage.value =
          error.response?.data?.message || "Failed to publish problem";
      } finally {
        isSubmitting.value = false;
      }
    }

    return {
      categories,
      topics,
      form,
      difficulties,
      isSubmitting,
      isPreviewVisible,
      successMessage,
      errorMessage,
      getTopicIndent,
      getTopicPrefix,
      getDifficultyClass,
      getDifficultyLabel,
      togglePreview,
      resetForm,
      submitProblem,
    };
  },
};
</script>
