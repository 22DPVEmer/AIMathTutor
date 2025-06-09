<template>
  <div class="topics-list">
    <h2 class="text-2xl font-bold mb-6">Math Topics</h2>

    <!-- School Class Navigation Tabs -->
    <div class="school-class-tabs mb-6">
      <!-- Mobile Dropdown -->
      <div class="md:hidden">
        <select
          v-model="selectedSchoolClass"
          @change="selectSchoolClass(selectedSchoolClass)"
          class="w-full p-3 border border-gray-300 rounded-md text-base bg-white"
        >
          <option
            v-for="(schoolClass, index) in schoolClasses"
            :key="index"
            :value="schoolClass"
          >
            {{ schoolClass.name }}
          </option>
        </select>
      </div>

      <!-- Desktop Tabs -->
      <div class="hidden md:flex border-b overflow-x-auto">
        <button
          v-for="(schoolClass, index) in schoolClasses"
          :key="index"
          @click="selectSchoolClass(schoolClass)"
          :class="[
            'px-4 py-2 text-center whitespace-nowrap flex-shrink-0',
            selectedSchoolClass.id === schoolClass.id
              ? 'border-b-2 border-blue-500 text-blue-600 font-medium'
              : 'text-gray-600 hover:text-blue-500',
          ]"
        >
          {{ schoolClass.name }}
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

    <!-- Topic Hierarchy Display -->
    <div v-else class="topics-hierarchy">
      <!-- Parent Topics Section -->
      <div
        v-for="(parentTopic, parentIndex) in parentTopics"
        :key="parentIndex"
        class="mb-8"
      >
        <div
          class="parent-topic p-4 bg-gray-100 rounded-lg mb-4 cursor-pointer hover:bg-gray-50 transition-colors"
          @click="navigateToParentTopic(parentTopic)"
        >
          <div class="flex flex-col space-y-3">
            <!-- Mobile Layout -->
            <div class="md:hidden">
              <div class="flex flex-col space-y-2">
                <div class="flex justify-between items-start">
                  <h3 class="font-bold text-lg flex-1 pr-2">{{ parentTopic.name }}</h3>
                  <span
                    class="difficulty-badge px-2 py-1 rounded-full text-xs flex-shrink-0"
                    :class="getDifficultyClass(parentTopic.difficulty)"
                  >
                    {{ getDifficultyLabel(parentTopic.difficulty) }}
                  </span>
                </div>

                <!-- Progress info for mobile -->
                <div class="flex justify-between items-center text-sm">
                  <span
                    class="completion-badge px-2 py-1 rounded-full text-xs bg-blue-100 text-blue-800"
                    v-if="parentTopic.percentageCompleted !== undefined"
                  >
                    {{ parentTopic.percentageCompleted }}% complete
                  </span>
                  <span class="px-2 py-1 bg-gray-200 text-gray-700 rounded-full text-xs">
                    {{ getSubtopics(parentTopic.id).length }} subtopics
                  </span>
                </div>
              </div>
            </div>

            <!-- Desktop Layout -->
            <div class="hidden md:block">
              <div class="flex justify-between items-center">
                <h3 class="font-bold text-lg">{{ parentTopic.name }}</h3>
                <div class="flex items-center space-x-2">
                  <!-- Completion percentage -->
                  <span
                    class="completion-badge px-2 py-1 rounded-full text-xs bg-blue-100 text-blue-800"
                    v-if="parentTopic.percentageCompleted !== undefined"
                  >
                    {{ parentTopic.percentageCompleted }}% complete
                  </span>
                  <!-- Debug info -->
                  <span
                    class="debug-info text-xs text-gray-500"
                    v-if="parentTopic.pointsEarned !== undefined"
                  >
                    ({{ parentTopic.pointsEarned }}/{{
                      parentTopic.totalPointsPossible
                    }}
                    pts)
                  </span>
                  <!-- Difficulty badge -->
                  <span
                    class="difficulty-badge px-2 py-1 rounded-full text-xs"
                    :class="getDifficultyClass(parentTopic.difficulty)"
                  >
                    {{ getDifficultyLabel(parentTopic.difficulty) }}
                  </span>
                </div>
              </div>
            </div>

            <!-- Progress bar -->
            <div
              class="w-full bg-gray-200 rounded-full h-2.5"
              v-if="parentTopic.percentageCompleted !== undefined"
            >
              <div
                class="bg-blue-600 h-2.5 rounded-full transition-all duration-300"
                :style="{ width: parentTopic.percentageCompleted + '%' }"
                :class="{
                  'bg-green-600': parentTopic.percentageCompleted >= 100,
                }"
              ></div>
            </div>

            <p class="text-gray-600 text-sm">
              {{ parentTopic.description }}
            </p>

            <!-- Subtopics count badge for desktop -->
            <div class="hidden md:flex justify-end">
              <span class="px-2 py-1 bg-gray-200 text-gray-700 rounded-full text-xs">
                {{ getSubtopics(parentTopic.id).length }} subtopics
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- No topics message -->
      <div
        v-if="parentTopics.length === 0"
        class="text-center py-8 text-gray-500"
      >
        No topics available for this class.
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { api } from "@/api/user";
import { getTopicCompletion } from "@/api/math";

export default {
  name: "TopicsList",

  setup() {
    const router = useRouter();
    const loading = ref(false);
    const schoolClasses = ref([]);
    const topics = ref([]);
    const selectedSchoolClass = ref({});
    const selectedTopic = ref({});
    const expandedParentTopics = ref(new Set());

    const filteredTopics = computed(() => {
      if (!selectedSchoolClass.value || !selectedSchoolClass.value.id)
        return [];
      return topics.value.filter(
        (topic) => topic.schoolClassId === selectedSchoolClass.value.id
      );
    });

    // Parent topics are those without a parentTopicId
    const parentTopics = computed(() => {
      return filteredTopics.value.filter((topic) => !topic.parentTopicId);
    });

    // Get subtopics for a specific parent topic
    function getSubtopics(parentId) {
      return filteredTopics.value.filter(
        (topic) => topic.parentTopicId === parentId
      );
    }

    function navigateToParentTopic(parentTopic) {
      const hasSubtopics = getSubtopics(parentTopic.id).length > 0;

      if (hasSubtopics) {
        // If it has subtopics, navigate to the subtopics view
        router.push({
          name: 'SubtopicsList',
          params: { parentId: parentTopic.id }
        });
      } else {
        // If it doesn't have subtopics, navigate directly to problems
        navigateToTopic(parentTopic);
      }
    }

    function navigateToTopic(topic) {
      selectedTopic.value = topic;
      router.push({
        name: 'TopicProblems',
        params: { topicId: topic.id }
      });
    }

    async function fetchSchoolClasses() {
      loading.value = true;
      try {
        const response = await api.get("/schoolclass");
        schoolClasses.value = response.data;
        if (schoolClasses.value.length > 0) {
          selectedSchoolClass.value = schoolClasses.value[0];
          await fetchTopics();
        }
      } catch (error) {
        // Error fetching school classes
      } finally {
        loading.value = false;
      }
    }

    async function fetchTopics() {
      if (!selectedSchoolClass.value || !selectedSchoolClass.value.id) return;

      loading.value = true;
      try {
        // Fetch topics for the selected school class
        const response = await api.get(
          `/mathtopic/schoolclass/${selectedSchoolClass.value.id}`
        );
        topics.value = response.data;

        // Fetch topic completion data
        try {
          const token = localStorage.getItem("token");
          if (token) {
            const completionData = await getTopicCompletion();

            // Update topics with completion data
            if (completionData && completionData.length > 0) {

              // Log total points and earned points across all topics
              const totalPointsPossible = completionData.reduce(
                (sum, tc) => sum + tc.totalPointsPossible,
                0
              );
              const totalPointsEarned = completionData.reduce(
                (sum, tc) => sum + tc.pointsEarned,
                0
              );
              const overallPercentage =
                totalPointsPossible > 0
                  ? Math.round((totalPointsEarned / totalPointsPossible) * 100)
                  : 0;

              // Update each topic with its completion data
              topics.value = topics.value.map((topic) => {
                const topicCompletion = completionData.find(
                  (tc) => tc.topicId === topic.id
                );
                if (topicCompletion) {
                  return {
                    ...topic,
                    totalPointsPossible: topicCompletion.totalPointsPossible,
                    pointsEarned: topicCompletion.pointsEarned,
                    percentageCompleted: topicCompletion.percentageCompleted,
                  };
                } else {
                  return topic;
                }
              });
            }
          }
        } catch (completionError) {
          // Error fetching topic completion data
        }
      } catch (error) {
        // Error fetching topics
      } finally {
        loading.value = false;
      }
    }

    function selectSchoolClass(schoolClass) {
      selectedSchoolClass.value = schoolClass;
      selectedTopic.value = {};
      fetchTopics();
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

    // Initialize by fetching school classes and topics on component mount
    onMounted(async () => {
      await fetchSchoolClasses();

      // After topics are loaded, calculate completion data
      if (topics.value.length > 0) {
        try {
          // Check if user is logged in
          const token = localStorage.getItem("token");
          if (token) {
            // Get all user attempts
            const userAttemptsResponse = await api.get("/mathproblemAttempt");
            updateTopicCompletionData(userAttemptsResponse.data);
          } else {
            // Update without user attempts
            updateTopicCompletionData();
          }
        } catch (error) {
          updateTopicCompletionData();
        }
      }
    });

    // Calculate and update completion data for all topics
    function updateTopicCompletionData(userAttempts = []) {
      // Process all topics to calculate completion percentages
      const processTopics = async () => {
        // Process parent topics first
        for (const parentTopic of parentTopics.value) {
          await calculateTopicCompletion(parentTopic, userAttempts);

          // Process subtopics
          const subtopics = getSubtopics(parentTopic.id);
          for (const subtopic of subtopics) {
            await calculateTopicCompletion(subtopic, userAttempts);
          }
        }
      };

      // Start processing
      processTopics();
    }

    // Calculate completion percentage for a single topic
    async function calculateTopicCompletion(topic, userAttempts = []) {
      try {
        // Fetch problems for this topic if we don't already have them
        let topicProblems = [];

        // Fetch the problems for this topic
        try {
          const response = await api.get(`/mathproblem/topic/${topic.id}`);
          topicProblems = response.data;
        } catch (error) {
          topicProblems = [];
        }

        // Calculate total points possible
        const totalPointsPossible = topicProblems.reduce(
          (sum, problem) => sum + (problem.pointValue || 1),
          0
        );

        // Calculate points earned
        let pointsEarned = 0;

        if (userAttempts && userAttempts.length > 0) {

          // First, create maps to track which statements have been completed
          const completedStatements = new Map();

          // Process all attempts to build maps of completed statements
          userAttempts.forEach((attempt) => {
            if (!attempt.problemStatement || !attempt.isCorrect) return;

            // Normalize the statement for consistent comparison
            const normalizedStatement = attempt.problemStatement
              .toLowerCase()
              .replace(/\s+/g, "");

            // Track completed statements with their points
            if (attempt.isCorrect) {
              // If we already have this statement, keep the one with higher points
              if (
                !completedStatements.has(normalizedStatement) ||
                completedStatements.get(normalizedStatement).pointsEarned <
                  attempt.pointsEarned
              ) {
                completedStatements.set(normalizedStatement, {
                  pointsEarned: attempt.pointsEarned,
                  problemId: attempt.problemId,
                });
              }
            }
          });

          // Create a map of problem IDs to track which ones we've counted
          const countedProblemIds = new Set();
          // Also track normalized statements we've counted to avoid duplicates
          const countedStatements = new Set();

          // For each problem, find the best attempt
          topicProblems.forEach((problem) => {
            // Normalize the problem statement for comparison
            const normalizedStatement = problem.statement
              .toLowerCase()
              .replace(/\s+/g, "");

            // Skip if we've already counted a problem with this statement
            if (countedStatements.has(normalizedStatement)) {
              return;
            }

            // Find successful attempts for this problem by ID first
            let successfulAttempt = userAttempts.find(
              (a) => a.problemId === problem.id && a.isCorrect
            );

            // If no direct match, check if any problem with the same statement has been completed
            if (
              !successfulAttempt &&
              completedStatements.has(normalizedStatement)
            ) {
              // Get the points earned for this statement
              const completionInfo =
                completedStatements.get(normalizedStatement);

              // Find the actual attempt for logging purposes
              successfulAttempt = userAttempts.find(
                (a) => a.problemId === completionInfo.problemId && a.isCorrect
              );

              // If we can't find the exact attempt, create a synthetic one with the points
              if (!successfulAttempt) {
                successfulAttempt = {
                  pointsEarned: completionInfo.pointsEarned,
                  isCorrect: true,
                };
              }
            }

            if (successfulAttempt) {
              // Only count each problem once by tracking its ID and statement
              if (!countedProblemIds.has(problem.id)) {
                const points =
                  successfulAttempt.pointsEarned || problem.pointValue || 1;
                pointsEarned += points;
                countedProblemIds.add(problem.id);
                countedStatements.add(normalizedStatement);
              }
            }
          });
        }

        // Calculate percentage
        const percentageCompleted =
          totalPointsPossible > 0
            ? Math.round((pointsEarned / totalPointsPossible) * 100)
            : 0;

        // Update the topic with completion data
        const updatedTopic = {
          ...topic,
          totalPointsPossible,
          pointsEarned,
          percentageCompleted,
        };

        // Update the topic in the topics array
        const index = topics.value.findIndex((t) => t.id === topic.id);
        if (index !== -1) {
          topics.value[index] = updatedTopic;
        }

        return updatedTopic;
      } catch (error) {
        return {
          ...topic,
          totalPointsPossible: 0,
          pointsEarned: 0,
          percentageCompleted: 0,
        };
      }
    }

    return {
      loading,
      schoolClasses,
      topics,
      filteredTopics,
      parentTopics,
      selectedSchoolClass,
      selectedTopic,
      expandedParentTopics,
      getSubtopics,
      navigateToParentTopic,
      selectSchoolClass,
      navigateToTopic,
      getDifficultyClass,
      getDifficultyLabel,
      updateTopicCompletionData,
      calculateTopicCompletion,
    };
  },
};
</script>

<style scoped>
.topics-list {
  max-width: 1000px;
  margin: 0 auto;
}

.parent-topic {
  border-left: 4px solid #4f46e5;
  transition: all 0.2s ease;
}

.parent-topic:hover {
  background-color: #f5f5ff;
}

.subtopic-card {
  transition: all 0.2s ease;
}

.subtopic-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
}
</style>
