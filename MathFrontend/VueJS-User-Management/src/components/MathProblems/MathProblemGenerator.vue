<template>
  <div class="math-problem-generator w-full px-4 sm:px-6 lg:px-8">
    <div class="bg-white shadow-md rounded-lg p-4 sm:p-6 md:p-8 mb-6 w-full">
      <div class="flex items-center justify-center mb-6">
        <h2 class="card-title text-2xl font-bold text-purple-800 relative pb-2">Create Your Problem</h2>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <div>
          <label for="topicType" class="block mb-2 font-medium text-gray-700"
            >Topic Selection</label
          >
          <div class="flex flex-col sm:flex-row gap-3 sm:gap-4 mb-3">
            <label class="inline-flex items-center cursor-pointer gap-2 py-1 min-h-[44px] sm:min-h-[40px]">
              <input
                type="radio"
                v-model="topicSelectionType"
                value="existing"
                class="radio-input w-5 h-5 sm:w-[18px] sm:h-[18px] border-2 border-gray-300 rounded-full appearance-none bg-white checked:bg-blue-600 checked:border-blue-600 focus:ring-2 focus:ring-blue-500 focus:ring-offset-0 transition-all duration-200 flex-shrink-0"
              />
              <span class="text-sm sm:text-base font-medium text-gray-700 select-none">Use Existing Topic</span>
            </label>
            <label class="inline-flex items-center cursor-pointer gap-2 py-1 min-h-[44px] sm:min-h-[40px]">
              <input
                type="radio"
                v-model="topicSelectionType"
                value="custom"
                class="radio-input w-5 h-5 sm:w-[18px] sm:h-[18px] border-2 border-gray-300 rounded-full appearance-none bg-white checked:bg-blue-600 checked:border-blue-600 focus:ring-2 focus:ring-blue-500 focus:ring-offset-0 transition-all duration-200 flex-shrink-0"
              />
              <span class="text-sm sm:text-base font-medium text-gray-700 select-none">Custom Topic</span>
            </label>
          </div>

          <div v-if="topicSelectionType === 'existing'" class="mt-3">
            <!-- Topic Pills for existing topics -->
            <div class="max-h-48 sm:max-h-60 overflow-y-auto overflow-x-hidden pr-1 sm:pr-2 border border-gray-200 rounded-lg p-2 bg-gray-50 scrollbar-thin scrollbar-thumb-blue-500 scrollbar-track-gray-100">
              <div class="flex flex-wrap gap-2">
                <div
                  v-for="topic in topics"
                  :key="topic.id"
                  class="topic-pill px-3 py-2 bg-gray-200 text-gray-700 rounded-full text-sm font-medium cursor-pointer transition-all duration-200 hover:bg-blue-100 hover:text-blue-700 min-h-[44px] sm:min-h-[40px] flex items-center justify-center select-none active:scale-95"
                  :class="{
                    'bg-blue-600 text-white hover:bg-blue-700 hover:text-white': selectedTopicId == topic.id,
                    'bg-gray-200 text-gray-700': selectedTopicId != topic.id
                  }"
                  @click="selectTopic(topic.id)"
                >
                  {{ topic.name }}
                  <span v-if="topic.problemCount > 0" class="ml-1 text-xs opacity-75"
                    >({{ topic.problemCount }})</span
                  >
                </div>
              </div>
            </div>
          </div>

          <div v-else class="mt-3">
            <input
              id="customTopic"
              v-model="formData.topic"
              class="w-full p-3 border-2 border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base transition-all duration-200"
              placeholder="e.g. Algebra, Calculus, Geometry"
            />
          </div>
        </div>

        <div>
          <label for="difficulty" class="block mb-2 font-medium text-gray-700"
            >Difficulty</label
          >
          <select
            id="difficulty"
            v-model="formData.difficulty"
            class="w-full p-3 border-2 border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base transition-all duration-200 bg-white"
          >
            <option value="Easy">Easy</option>
            <option value="Medium">Medium</option>
            <option value="Hard">Hard</option>
          </select>
        </div>
      </div>

      <div class="flex justify-center mt-6">
        <button
          @click="generateProblem"
          class="btn-primary px-8 py-4 bg-blue-600 text-white rounded-md hover:bg-blue-700 hover:-translate-y-0.5 hover:shadow-lg transition-all duration-200 font-medium text-lg w-full sm:w-auto min-h-[56px] flex items-center justify-center shadow-md disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none disabled:shadow-md"
          :disabled="isLoading"
        >
          <span v-if="isLoading">Generating...</span>
          <span v-else>Generate Problem</span>
        </button>
      </div>
    </div>

    <div
      v-if="generatedProblem"
      class="bg-white rounded-xl shadow-lg border border-gray-200 p-6 sm:p-8 mt-8 w-full"
    >
      <div class="flex justify-between items-center mb-6 pb-4 border-b border-gray-200">
        <h3 class="text-xl sm:text-2xl font-bold text-purple-800">{{ formData.topic }} Problem</h3>
        <span class="px-3 py-1 bg-blue-500 text-white rounded-full text-sm font-semibold">{{ formData.difficulty }}</span>
      </div>

      <div class="mb-6 text-lg leading-relaxed">
        <div v-html="formattedStatement" class="prose max-w-none"></div>
      </div>

      <div class="mb-4">
        <label for="answer" class="block mb-2 font-medium text-gray-700"
          >Your Answer:</label
        >
        <input
          id="answer"
          v-model="userAnswer"
          class="w-full p-3 border-2 border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base transition-all duration-200"
          placeholder="Enter your answer here"
        />
      </div>

      <div class="flex flex-wrap gap-3 mb-4">
        <button
          @click="checkAnswer"
          class="btn-primary px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 hover:-translate-y-0.5 hover:shadow-lg transition-all duration-200 font-medium text-sm sm:text-base min-h-[44px] flex items-center justify-center shadow-md disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none disabled:shadow-md"
          :disabled="!userAnswer || isChecking"
        >
          <span v-if="isChecking">Checking...</span>
          <span v-else>Check Answer</span>
        </button>

        <button
          @click="showSolution"
          class="px-4 py-3 bg-transparent border-none text-blue-600 hover:text-blue-800 hover:bg-blue-50 rounded-md transition-all duration-200 font-semibold text-sm sm:text-base min-h-[44px] flex items-center justify-center gap-2"
          v-if="evaluation || solutionVisible"
        >
          <span>{{ solutionVisible ? "Hide Solution" : "Show Solution" }}</span>
          <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="transition-transform duration-200"
            :class="{ 'rotate-180': solutionVisible }"
          >
            <polyline points="6 9 12 15 18 9"></polyline>
          </svg>
        </button>

        <button
          v-if="evaluation"
          @click="saveAttempt"
          class="btn-primary px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 hover:-translate-y-0.5 hover:shadow-lg transition-all duration-200 font-medium text-sm sm:text-base min-h-[44px] flex items-center justify-center shadow-md disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none disabled:shadow-md"
          :disabled="isSaving || attemptSaved"
        >
          <span v-if="isSaving">Saving...</span>
          <span v-else-if="attemptSaved">Saved!</span>
          <span v-else>Save to my problems</span>
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

      <div
        v-if="solutionVisible"
        class="mt-6 p-6 bg-gray-50 rounded-lg border border-gray-200"
      >
        <h4 class="font-semibold mb-3 text-lg text-gray-800">Solution:</h4>
        <div v-html="formattedSolution" class="mb-4 p-4 bg-white rounded-md border border-gray-100"></div>

        <h4 class="font-semibold mt-6 mb-3 text-lg text-gray-800">Explanation:</h4>
        <div v-html="formattedExplanation" class="p-4 bg-white rounded-md border border-gray-100"></div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import {
  generateMathProblem,
  evaluateMathAnswer,
  saveUserMathProblem,
  getAllTopics,
} from "@/api/math";

export default {
  name: "MathProblemGenerator",

  setup() {
    const formData = ref({
      topic: "",
      difficulty: "Medium",
      saveToDatabase: false,
    });

    const topics = ref([]);
    const topicSelectionType = ref("existing");
    const selectedTopicId = ref("");

    const isLoading = ref(false);
    const isChecking = ref(false);
    const isSaving = ref(false);
    const attemptSaved = ref(false);
    const generatedProblem = ref(null);
    const userAnswer = ref("");
    const evaluation = ref(null);
    const solutionVisible = ref(false);

    // Fetch all topics when component is mounted
    onMounted(async () => {
      try {
        const allTopics = await getAllTopics();
        // Flatten the topic hierarchy for easier selection
        const flattenedTopics = flattenTopics(allTopics);

        // Sort topics alphabetically by name
        flattenedTopics.sort((a, b) => a.name.localeCompare(b.name));

        // Filter out topics with no problems if there are enough topics with problems
        const topicsWithProblems = flattenedTopics.filter(
          (t) => t.problemCount > 0
        );
        if (topicsWithProblems.length >= 5) {
          topics.value = topicsWithProblems;
        } else {
          topics.value = flattenedTopics;
        }

        console.log("Processed topics:", topics.value);
      } catch (error) {
        console.error("Error fetching topics:", error);
      }
    });

    function flattenTopics(topicsArray, prefix = "") {
      let result = [];

      for (const topic of topicsArray) {
        const displayName = prefix ? `${prefix} > ${topic.name}` : topic.name;

        // Check if this is a valid topic to include
        // Include if it's a leaf topic (no subtopics) OR if it has problems
        const isLeafTopic = !topic.subtopics || topic.subtopics.length === 0;
        const hasProblems = topic.problemCount > 0;

        if (isLeafTopic || hasProblems) {
          result.push({
            id: topic.id,
            name: displayName,
            originalName: topic.name,
            problemCount: topic.problemCount || 0,
          });
        }
        // If topic has subtopics, only process the subtopics
        else if (topic.subtopics && topic.subtopics.length > 0) {
          // Add all subtopics
          result = result.concat(flattenTopics(topic.subtopics, displayName));
        }
      }

      return result;
    }

    // Handle topic selection change
    function handleTopicChange() {
      if (selectedTopicId.value) {
        const selectedTopic = topics.value.find(
          (t) => t.id === parseInt(selectedTopicId.value)
        );
        if (selectedTopic) {
          formData.value.topic = selectedTopic.originalName;
        }
      }
    }

    // Select a topic when clicking on a topic pill
    function selectTopic(topicId) {
      selectedTopicId.value = topicId;
      handleTopicChange();
    }

    const formattedStatement = computed(() => {
      return generatedProblem.value?.statement.replace(/\n/g, "<br>");
    });

    const formattedSolution = computed(() => {
      return generatedProblem.value?.solution.replace(/\n/g, "<br>");
    });

    const formattedExplanation = computed(() => {
      return generatedProblem.value?.explanation.replace(/\n/g, "<br>");
    });

    async function generateProblem() {
      // Validate input based on selection type
      if (topicSelectionType.value === "custom" && !formData.value.topic) {
        alert("Please enter a topic");
        return;
      } else if (
        topicSelectionType.value === "existing" &&
        !selectedTopicId.value
      ) {
        alert("Please select a topic");
        return;
      }

      isLoading.value = true;
      evaluation.value = null;
      userAnswer.value = "";
      solutionVisible.value = false;
      attemptSaved.value = false;

      try {
        // Create request object based on selection type
        const request = {
          topic: formData.value.topic,
          difficulty: formData.value.difficulty,
          saveToDatabase: formData.value.saveToDatabase,
        };

        // Add topicId if using an existing topic
        if (topicSelectionType.value === "existing" && selectedTopicId.value) {
          // Use Object.assign to add the topicId property
          Object.assign(request, { topicId: parseInt(selectedTopicId.value) });
        }

        generatedProblem.value = await generateMathProblem(request);
      } catch (error) {
        console.error("Error generating problem:", error);
        alert("Failed to generate problem. Please try again.");
      } finally {
        isLoading.value = false;
      }
    }

    async function checkAnswer() {
      if (!userAnswer.value) {
        alert("Please enter your answer");
        return;
      }

      isChecking.value = true;
      attemptSaved.value = false;

      try {
        evaluation.value = await evaluateMathAnswer({
          problem: generatedProblem.value.statement,
          userAnswer: userAnswer.value,
        });

        if (!evaluation.value.isCorrect) {
          solutionVisible.value = true;
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

    async function saveAttempt() {
      if (!evaluation.value) {
        alert("You need to check your answer before saving");
        return;
      }

      isSaving.value = true;

      try {
        const saveRequest = {
          statement: generatedProblem.value.statement,
          solution: generatedProblem.value.solution,
          explanation: generatedProblem.value.explanation,
          userAnswer: userAnswer.value,
          isCorrect: evaluation.value.isCorrect,
          difficulty: formData.value.difficulty,
          topic: formData.value.topic,
        };

        if (topicSelectionType.value === "existing" && selectedTopicId.value) {
          Object.assign(saveRequest, {
            topicId: parseInt(selectedTopicId.value),
          });
        }

        const result = await saveUserMathProblem(saveRequest);

        if (result) {
          attemptSaved.value = true;
        } else {
          alert("Failed to save the problem. Please try again.");
        }
      } catch (error) {
        console.error("Error saving problem:", error);
        alert("Failed to save the problem. Please try again.");
      } finally {
        isSaving.value = false;
      }
    }

    return {
      formData,
      isLoading,
      isChecking,
      isSaving,
      attemptSaved,
      generatedProblem,
      userAnswer,
      evaluation,
      solutionVisible,
      formattedStatement,
      formattedSolution,
      formattedExplanation,
      generateProblem,
      checkAnswer,
      showSolution,
      saveAttempt,
      // Topic selection
      topics,
      topicSelectionType,
      selectedTopicId,
      handleTopicChange,
      selectTopic,
    };
  },
};
</script>

<style scoped>
/* Container responsive adjustments */
.math-problem-generator {
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
}

@media (max-width: 1200px) {
  .math-problem-generator {
    max-width: 95%;
    padding: 0 1rem;
  }
}

@media (max-width: 768px) {
  .math-problem-generator {
    max-width: 100%;
    padding: 0 0.5rem;
  }
}

/* Title underline decoration - can't be done with Tailwind */
.card-title::after {
  content: "";
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 60px;
  height: 3px;
  background: linear-gradient(to right, #4361ee, #4cc9f0);
  border-radius: 3px;
}

/* Radio button custom styling - only what Tailwind can't handle */
.radio-input:checked::after {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: white;
}

/* Blue gradient for primary buttons */
.btn-primary {
  background: linear-gradient(45deg, #4361ee, #4895ef);
}

.btn-primary:hover:not(:disabled) {
  background: linear-gradient(45deg, #4895ef, #4361ee);
}

/* Mobile responsive radio button dot sizing */
@media (max-width: 768px) {
  .radio-input:checked::after {
    width: 6px;
    height: 6px;
  }
}

@media (max-width: 480px) {
  .radio-input:checked::after {
    width: 5px;
    height: 5px;
  }
}


</style>
