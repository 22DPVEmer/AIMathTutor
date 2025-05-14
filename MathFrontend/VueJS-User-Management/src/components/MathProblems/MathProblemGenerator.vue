<template>
  <div class="math-problem-generator w-full px-4 sm:px-6 lg:px-8">
    <div class="bg-white shadow-md rounded-lg p-4 sm:p-6 md:p-8 mb-6 w-full">
      <div class="card-header">
        <h2 class="card-title">Create Your Problem</h2>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <div>
          <label for="topicType" class="block mb-2 font-medium text-gray-700"
            >Topic Selection</label
          >
          <div class="flex gap-4 mb-3">
            <label class="inline-flex items-center">
              <input
                type="radio"
                v-model="topicSelectionType"
                value="existing"
                class="mr-2 text-blue-600"
              />
              Use Existing Topic
            </label>
            <label class="inline-flex items-center">
              <input
                type="radio"
                v-model="topicSelectionType"
                value="custom"
                class="mr-2 text-blue-600"
              />
              Custom Topic
            </label>
          </div>

          <div v-if="topicSelectionType === 'existing'" class="mt-3">
            <!-- Topic Pills for existing topics -->
            <div class="topic-pills max-h-60 overflow-y-auto pr-2">
              <div
                v-for="topic in topics"
                :key="topic.id"
                class="topic-pill"
                :class="{ active: selectedTopicId == topic.id }"
                @click="selectTopic(topic.id)"
              >
                {{ topic.name }}
                <span v-if="topic.problemCount > 0" class="problem-count"
                  >({{ topic.problemCount }})</span
                >
              </div>
            </div>
          </div>

          <div v-else class="mt-3">
            <div class="input-container">
              <input
                id="customTopic"
                v-model="formData.topic"
                class="input-animated w-full"
                placeholder=" "
              />
              <label for="customTopic" class="input-label"
                >e.g. Algebra, Calculus, Geometry</label
              >
            </div>
          </div>
        </div>

        <div>
          <label for="difficulty" class="block mb-2 font-medium text-gray-700"
            >Difficulty</label
          >
          <div class="dropdown">
            <select
              id="difficulty"
              v-model="formData.difficulty"
              class="dropdown-select w-full"
            >
              <option value="Easy">Easy</option>
              <option value="Medium">Medium</option>
              <option value="Hard">Hard</option>
            </select>
          </div>
        </div>
      </div>

      <div class="actions">
        <button
          @click="generateProblem"
          class="btn btn-primary btn-lg w-full sm:w-auto"
          :disabled="isLoading"
        >
          <span v-if="isLoading">Generating...</span>
          <span v-else>Generate Problem</span>
        </button>
      </div>
    </div>

    <div
      v-if="generatedProblem"
      class="problem-card w-full"
      style="display: block"
    >
      <div class="problem-header">
        <h3 class="problem-title">{{ formData.topic }} Problem</h3>
        <span class="problem-badge">{{ formData.difficulty }}</span>
      </div>

      <div class="problem-content">
        <div v-html="formattedStatement" class="prose max-w-none"></div>
      </div>

      <div class="mb-4">
        <label for="answer" class="block mb-2 font-medium text-gray-700"
          >Your Answer:</label
        >
        <input
          id="answer"
          v-model="userAnswer"
          class="input-animated w-full"
          placeholder="Enter your answer here"
        />
      </div>

      <div class="flex flex-wrap gap-3 mb-4">
        <button
          @click="checkAnswer"
          class="btn btn-primary"
          :disabled="!userAnswer || isChecking"
        >
          <span v-if="isChecking">Checking...</span>
          <span v-else>Check Answer</span>
        </button>

        <button
          @click="showSolution"
          class="solution-toggle"
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
          >
            <polyline
              points="6 9 12 15 18 9"
              v-if="!solutionVisible"
            ></polyline>
            <polyline points="18 15 12 9 6 15" v-else></polyline>
          </svg>
        </button>

        <button
          v-if="evaluation"
          @click="saveAttempt"
          class="btn btn-primary"
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
        class="solution-content"
        style="display: block"
      >
        <h4 class="font-semibold mb-2">Solution:</h4>
        <div v-html="formattedSolution"></div>

        <h4 class="font-semibold mt-4 mb-2">Explanation:</h4>
        <div v-html="formattedExplanation"></div>
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
.math-problem-generator {
  max-width: 1000px;
  width: 100%;
  margin: 0 auto;
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1.5rem;
}

.card-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #3a0ca3;
  position: relative;
  padding-bottom: 0.5rem;
}

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

.topic-pills {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-top: 1rem;
}

.topic-pill {
  background-color: #e9ecef;
  color: #212529;
  padding: 0.5rem 1rem;
  border-radius: 30px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.topic-pill:hover {
  background-color: #4895ef;
  color: white;
}

.topic-pill.active {
  background-color: #4361ee;
  color: white;
}

.problem-count {
  font-size: 0.8rem;
  opacity: 0.8;
  margin-left: 3px;
}

.input-container {
  position: relative;
  margin-top: 0.5rem;
}

.input-animated {
  width: 100%;
  padding: 0.75rem 1rem;
  font-size: 1rem;
  border: 2px solid #e9ecef;
  border-radius: 12px;
  transition: all 0.3s ease;
}

.input-animated:focus {
  outline: none;
  border-color: #4361ee;
}

.input-animated:focus + .input-label,
.input-animated:not(:placeholder-shown) + .input-label {
  top: -10px;
  left: 10px;
  font-size: 0.8rem;
  padding: 0 5px;
  background-color: white;
}

.input-label {
  position: absolute;
  left: 15px;
  top: 50%;
  transform: translateY(-50%);
  color: #6c757d;
  pointer-events: none;
  transition: all 0.3s ease;
}

.dropdown {
  position: relative;
}

.dropdown-select {
  width: 100%;
  padding: 0.75rem 1rem;
  font-size: 1rem;
  border: 2px solid #e9ecef;
  border-radius: 12px;
  background-color: white;
  cursor: pointer;
  appearance: none;
  -webkit-appearance: none;
  transition: all 0.3s ease;
}

.dropdown-select:focus {
  outline: none;
  border-color: #4895ef;
  box-shadow: 0 0 0 3px rgba(67, 97, 238, 0.25);
}

.dropdown::after {
  content: "▼";
  font-size: 12px;
  color: #6c757d;
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
}

.btn {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
  font-weight: 600;
  text-align: center;
  text-decoration: none;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-primary {
  background: linear-gradient(45deg, #4361ee, #4895ef);
  color: white;
}

.btn-primary:hover {
  background: linear-gradient(45deg, #4895ef, #4361ee);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(67, 97, 238, 0.3);
}

.btn-lg {
  padding: 1rem 2rem;
  font-size: 1.1rem;
}

.actions {
  display: flex;
  justify-content: center;
  margin-top: 1.5rem;
}

.problem-card {
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  padding: 2rem;
  margin-top: 2rem;
}

.problem-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 0.5rem;
  border-bottom: 1px solid #e9ecef;
}

.problem-title {
  font-size: 1.3rem;
  font-weight: 700;
  color: #3a0ca3;
}

.problem-badge {
  background-color: #4895ef;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.problem-content {
  margin-bottom: 1.5rem;
  font-size: 1.1rem;
}

.solution-toggle {
  background: none;
  border: none;
  color: #4361ee;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.solution-toggle:hover {
  color: #3a0ca3;
}

.solution-content {
  background-color: #e9ecef;
  padding: 1.5rem;
  border-radius: 12px;
  margin-top: 1rem;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 0.5s ease forwards;
}

@media (max-width: 768px) {
  .topic-pills {
    flex-direction: column;
    gap: 0.5rem;
  }
}
</style>
