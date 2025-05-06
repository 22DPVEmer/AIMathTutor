<template>
  <div class="subtopics-list">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold">{{ parentTopicName }}</h2>
      <button
        @click="goBackToTopics"
        class="px-4 py-2 bg-gray-200 text-gray-700 rounded-md hover:bg-gray-300 transition-colors flex items-center"
      >
        <span class="mr-1">←</span> Back to Topics
      </button>
    </div>

    <!-- Loading Indicator -->
    <div v-if="loading" class="text-center py-8">
      <div
        class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"
      ></div>
      <p class="mt-2 text-gray-600">Loading subtopics...</p>
    </div>

    <!-- Parent Topic Info -->
    <div v-else-if="parentTopic" class="mb-8">
      <div class="parent-topic p-4 bg-gray-100 rounded-lg mb-4">
        <div class="flex flex-col space-y-2">
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

          <!-- Progress bar -->
          <div
            class="w-full bg-gray-200 rounded-full h-2.5 dark:bg-gray-700"
            v-if="parentTopic.percentageCompleted !== undefined"
          >
            <div
              class="bg-blue-600 h-2.5 rounded-full"
              :style="{ width: parentTopic.percentageCompleted + '%' }"
              :class="{
                'bg-green-600': parentTopic.percentageCompleted >= 100,
              }"
            ></div>
          </div>

          <p class="text-gray-600 text-sm mt-2">
            {{ parentTopic.description }}
          </p>
        </div>
      </div>

      <!-- Subtopics Grid -->
      <h3 class="font-bold text-lg mb-4">Subtopics</h3>
      <div v-if="subtopics.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div
          v-for="(subtopic, subtopicIndex) in subtopics"
          :key="subtopicIndex"
          @click="navigateToTopic(subtopic)"
          class="subtopic-card border rounded-lg p-4 cursor-pointer hover:border-blue-500 transition-colors"
        >
          <div class="flex justify-between items-start">
            <h3 class="font-bold text-lg">{{ subtopic.name }}</h3>
            <div class="flex flex-col items-end space-y-1">
              <!-- Completion percentage -->
              <span
                class="completion-badge px-2 py-1 rounded-full text-xs bg-blue-100 text-blue-800"
                v-if="subtopic.percentageCompleted !== undefined"
              >
                {{ subtopic.percentageCompleted }}% complete
              </span>
              <!-- Debug info -->
              <span
                class="debug-info text-xs text-gray-500"
                v-if="subtopic.pointsEarned !== undefined"
              >
                ({{ subtopic.pointsEarned }}/{{
                  subtopic.totalPointsPossible
                }}
                pts)
              </span>
              <!-- Difficulty badge -->
              <span
                class="difficulty-badge px-2 py-1 rounded-full text-xs"
                :class="getDifficultyClass(subtopic.difficulty)"
              >
                {{ getDifficultyLabel(subtopic.difficulty) }}
              </span>
            </div>
          </div>
          <p class="text-gray-600 text-sm mt-2">{{ subtopic.description }}</p>
          <div class="flex items-center mt-4">
            <div class="flex-grow">
              <div class="h-2 bg-gray-200 rounded-full">
                <div
                  class="h-2 bg-green-500 rounded-full"
                  :style="`width: ${subtopic.percentageCompleted || 0}%`"
                ></div>
              </div>
            </div>
            <span class="text-xs text-gray-600 ml-2">
              {{ subtopic.pointsEarned || 0 }}/{{
                subtopic.totalPointsPossible || 0
              }}
              points
            </span>
          </div>
        </div>
      </div>
      <div v-else class="text-center py-8 text-gray-500">
        No subtopics available for this topic.
      </div>
    </div>

    <!-- No Parent Topic Found Message -->
    <div v-else class="text-center py-8 text-gray-500">
      Parent topic not found. Please go back and select a different topic.
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { api } from "@/api/user";
import { getTopicCompletion } from "@/api/math";

export default {
  name: "SubtopicsList",
  props: {
    parentId: {
      type: [Number, String],
      required: true
    }
  },

  setup(props) {
    const router = useRouter();
    const loading = ref(true);
    const parentTopic = ref(null);
    const parentTopicName = ref("");
    const subtopics = ref([]);
    const allTopics = ref([]);

    async function fetchTopics() {
      loading.value = true;
      try {
        // Fetch the parent topic details
        const parentResponse = await api.get(`/mathtopic/${props.parentId}`);
        parentTopic.value = parentResponse.data;
        parentTopicName.value = parentResponse.data.name;

        // Fetch all topics to get the subtopics
        const schoolClassId = parentTopic.value.schoolClassId;
        const topicsResponse = await api.get(`/mathtopic/schoolclass/${schoolClassId}`);
        allTopics.value = topicsResponse.data;

        // Filter subtopics for this parent
        subtopics.value = allTopics.value.filter(
          topic => topic.parentTopicId === parseInt(props.parentId)
        );

        // Fetch topic completion data
        try {
          const token = localStorage.getItem("token");
          if (token) {
            const completionData = await getTopicCompletion();

            // Update parent topic with completion data
            if (completionData && completionData.length > 0) {
              const parentCompletion = completionData.find(
                tc => tc.topicId === parseInt(props.parentId)
              );
              
              if (parentCompletion) {
                parentTopic.value = {
                  ...parentTopic.value,
                  totalPointsPossible: parentCompletion.totalPointsPossible,
                  pointsEarned: parentCompletion.pointsEarned,
                  percentageCompleted: parentCompletion.percentageCompleted
                };
              }

              // Update subtopics with completion data
              subtopics.value = subtopics.value.map(subtopic => {
                const subtopicCompletion = completionData.find(
                  tc => tc.topicId === subtopic.id
                );
                
                if (subtopicCompletion) {
                  return {
                    ...subtopic,
                    totalPointsPossible: subtopicCompletion.totalPointsPossible,
                    pointsEarned: subtopicCompletion.pointsEarned,
                    percentageCompleted: subtopicCompletion.percentageCompleted
                  };
                }
                
                return subtopic;
              });
            }
          }
        } catch (completionError) {
          console.warn("Error fetching topic completion data:", completionError);
        }
      } catch (error) {
        console.error("Error fetching topics:", error);
        parentTopic.value = null;
        subtopics.value = [];
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

    function navigateToTopic(topic) {
      router.push({
        name: 'TopicProblems',
        params: { topicId: topic.id }
      });
    }

    function goBackToTopics() {
      router.push({ name: 'TopicsList' });
    }

    onMounted(() => {
      fetchTopics();
    });

    return {
      loading,
      parentTopic,
      parentTopicName,
      subtopics,
      getDifficultyClass,
      getDifficultyLabel,
      navigateToTopic,
      goBackToTopics
    };
  },
};
</script>

<style scoped>
.subtopics-list {
  max-width: 1000px;
  margin: 0 auto;
}

.parent-topic {
  border-left: 4px solid #4f46e5;
  transition: all 0.2s ease;
}

.subtopic-card {
  transition: all 0.2s ease;
}

.subtopic-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
}
</style>
