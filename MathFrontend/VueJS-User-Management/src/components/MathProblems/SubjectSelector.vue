<template>
  <div class="subject-selector">
    <h2 class="text-2xl font-bold mb-6">Math Topics</h2>

    <!-- Subject Navigation Tabs -->
    <div class="subject-tabs mb-6">
      <div class="flex border-b">
        <button
          v-for="(subject, index) in subjects"
          :key="index"
          @click="selectSubject(subject)"
          :class="[
            'px-4 py-2 text-center',
            selectedSubject.id === subject.id
              ? 'border-b-2 border-blue-500 text-blue-600 font-medium'
              : 'text-gray-600 hover:text-blue-500',
          ]"
        >
          {{ subject.name }}
        </button>
      </div>
    </div>

    <!-- Loading Indicator -->
    <div v-if="loading" class="text-center py-8">
      <div
        class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"
      ></div>
      <p class="mt-2 text-gray-600">Loading topics...</p>
    </div>

    <!-- Topic Cards -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div
        v-for="(topic, index) in filteredTopics"
        :key="index"
        @click="selectTopic(topic)"
        class="topic-card border rounded-lg p-4 cursor-pointer hover:border-blue-500 transition-colors"
        :class="{ 'border-blue-500 bg-blue-50': selectedTopic.id === topic.id }"
      >
        <div class="flex justify-between items-start">
          <h3 class="font-bold text-lg">{{ topic.name }}</h3>
          <span
            class="difficulty-badge px-2 py-1 rounded-full text-xs"
            :class="getDifficultyClass(topic.difficulty)"
          >
            {{ getDifficultyLabel(topic.difficulty) }}
          </span>
        </div>
        <p class="text-gray-600 text-sm mt-2">{{ topic.description }}</p>
        <div class="flex items-center mt-4">
          <div class="flex-grow">
            <div class="h-2 bg-gray-200 rounded-full">
              <div
                class="h-2 bg-green-500 rounded-full"
                :style="`width: ${topic.percentageCompleted || 0}%`"
              ></div>
            </div>
          </div>
          <span class="text-xs text-gray-600 ml-2">
            {{ topic.pointsEarned || 0 }}/{{
              topic.totalPointsPossible || 0
            }}
            points
          </span>
        </div>
      </div>
    </div>

    <!-- Difficulty Selector -->
    <div
      class="difficulty-selector mt-8 p-4 border rounded-lg"
      v-if="selectedTopic.id"
    >
      <h3 class="font-bold mb-4">
        Select difficulty for {{ selectedTopic.name }}
      </h3>

      <div class="flex flex-wrap gap-3">
        <button
          v-for="difficulty in difficulties"
          :key="difficulty.value"
          @click="selectDifficulty(difficulty.value)"
          class="px-4 py-2 rounded-md transition-colors"
          :class="[
            selectedDifficulty === difficulty.value
              ? 'bg-blue-600 text-white'
              : 'bg-gray-100 hover:bg-gray-200',
          ]"
        >
          {{ difficulty.label }}
        </button>
      </div>

      <button
        @click="generateProblem"
        class="mt-6 bg-green-600 text-white px-6 py-3 rounded-md hover:bg-green-700 transition-colors w-full"
        :disabled="!canGenerateProblem"
      >
        Generate Problem
      </button>
    </div>

    <!-- Problem List for Selected Topic -->
    <div
      class="problems-list mt-8"
      v-if="selectedTopic.id && problems.length > 0"
    >
      <h3 class="font-bold mb-4">
        Available Problems for {{ selectedTopic.name }}
      </h3>

      <div class="overflow-x-auto">
        <table class="min-w-full bg-white rounded-lg overflow-hidden">
          <thead class="bg-gray-100">
            <tr>
              <th class="py-3 px-4 text-left">Problem</th>
              <th class="py-3 px-4 text-left">Difficulty</th>
              <th class="py-3 px-4 text-left">Points</th>
              <th class="py-3 px-4 text-left">Status</th>
              <th class="py-3 px-4 text-left">Action</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(problem, index) in problems"
              :key="index"
              class="border-t"
            >
              <td class="py-3 px-4">
                {{ problem.statement.substring(0, 40)
                }}{{ problem.statement.length > 40 ? "..." : "" }}
              </td>
              <td class="py-3 px-4">
                <span
                  class="difficulty-badge px-2 py-1 rounded-full text-xs"
                  :class="getDifficultyClass(problem.difficulty)"
                >
                  {{ getDifficultyLabel(problem.difficulty) }}
                </span>
              </td>
              <td class="py-3 px-4">{{ problem.pointValue }}</td>
              <td class="py-3 px-4">
                <span
                  class="px-2 py-1 rounded-full text-xs"
                  :class="{
                    'bg-green-100 text-green-800': problem.completed,
                    'bg-gray-100 text-gray-800': !problem.completed,
                  }"
                >
                  {{ problem.completed ? "Completed" : "Not Attempted" }}
                </span>
              </td>
              <td class="py-3 px-4">
                <button
                  @click="solveProblem(problem)"
                  class="px-3 py-1 bg-blue-600 text-white rounded-md hover:bg-blue-700 text-xs"
                >
                  Solve
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import axios from "axios";

export default {
  name: "SubjectSelector",

  emits: ["topic-selected", "generate-problem", "solve-problem"],

  setup(props, { emit }) {
    const loading = ref(false);
    const subjects = ref([]);
    const topics = ref([]);
    const problems = ref([]);

    const selectedSubject = ref({});
    const selectedTopic = ref({});
    const selectedDifficulty = ref("");

    const difficulties = [
      { value: "easy", label: "Easy" },
      { value: "medium", label: "Medium" },
      { value: "hard", label: "Hard" },
    ];

    const filteredTopics = computed(() => {
      if (!selectedSubject.value || !selectedSubject.value.id) return [];
      return topics.value.filter(
        (topic) => topic.categoryId === selectedSubject.value.id
      );
    });

    const canGenerateProblem = computed(() => {
      return selectedTopic.value.id && selectedDifficulty.value;
    });

    async function fetchCategories() {
      loading.value = true;
      try {
        const response = await axios.get(
          "http://localhost:5000/api/mathcategory"
        );
        subjects.value = response.data;
        if (subjects.value.length > 0) {
          selectedSubject.value = subjects.value[0];
          await fetchTopics();
        }
      } catch (error) {
        console.error("Error fetching categories:", error);
      } finally {
        loading.value = false;
      }
    }

    async function fetchTopics() {
      if (!selectedSubject.value || !selectedSubject.value.id) return;

      loading.value = true;
      try {
        const response = await axios.get(`http://localhost:5000/api/mathtopic`);
        topics.value = response.data;
      } catch (error) {
        console.error("Error fetching topics:", error);
      } finally {
        loading.value = false;
      }
    }

    async function fetchProblems() {
      if (!selectedTopic.value || !selectedTopic.value.id) {
        problems.value = [];
        return;
      }

      loading.value = true;
      try {
        const response = await axios.get(
          `http://localhost:5000/api/mathproblem/topic/${selectedTopic.value.id}`
        );
        problems.value = response.data;

        // Get user attempts for these problems
        const userAttempts = await axios.get(
          "http://localhost:5000/api/mathproblem/attempts"
        );

        // Mark problems as completed based on user attempts
        problems.value = problems.value.map((problem) => {
          const attempt = userAttempts.data.find(
            (a) => a.problemId === problem.id && a.isCorrect
          );
          return {
            ...problem,
            completed: !!attempt,
            pointsEarned: attempt?.pointsEarned || 0,
          };
        });
      } catch (error) {
        console.error("Error fetching problems:", error);
      } finally {
        loading.value = false;
      }
    }

    function selectSubject(subject) {
      selectedSubject.value = subject;
      selectedTopic.value = {};
      selectedDifficulty.value = "";
      problems.value = [];
    }

    async function selectTopic(topic) {
      selectedTopic.value = topic;
      selectedDifficulty.value = "";
      emit("topic-selected", topic);

      // Fetch problems for this topic
      await fetchProblems();
    }

    function selectDifficulty(difficulty) {
      selectedDifficulty.value = difficulty;
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

    function generateProblem() {
      if (!canGenerateProblem.value) return;

      emit("generate-problem", {
        topic: selectedTopic.value.name,
        difficulty: selectedDifficulty.value,
        topicId: selectedTopic.value.id,
      });
    }

    function solveProblem(problem) {
      emit("solve-problem", problem);
    }

    // Initialize by fetching categories and topics on component mount
    onMounted(() => {
      fetchCategories();
    });

    return {
      loading,
      subjects,
      topics,
      problems,
      filteredTopics,
      difficulties,
      selectedSubject,
      selectedTopic,
      selectedDifficulty,
      canGenerateProblem,
      selectSubject,
      selectTopic,
      selectDifficulty,
      getDifficultyClass,
      getDifficultyLabel,
      generateProblem,
      solveProblem,
    };
  },
};
</script>

<style scoped>
.subject-selector {
  max-width: 1000px;
  margin: 0 auto;
}

.topic-card {
  transition: all 0.2s ease;
}

.topic-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
}
</style>
