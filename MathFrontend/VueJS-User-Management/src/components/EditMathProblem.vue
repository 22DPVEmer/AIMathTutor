<template>
  <div
    v-if="show"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-2 sm:p-4 z-50"
  >
    <div
      class="bg-white rounded-lg shadow-xl border border-gray-200 w-full max-w-4xl max-h-[95vh] sm:max-h-[90vh] overflow-y-auto mx-4 sm:mx-0"
    >
      <div class="p-4 sm:p-6">
        <div class="flex justify-between items-center mb-6">
          <h2 class="text-2xl font-bold">Edit Math Problem</h2>
          <button @click="cancel" class="text-gray-500 hover:text-gray-700 hover:bg-gray-100 rounded-full p-2 transition-colors duration-200">
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
            <label for="problemName" class="block mb-2 font-medium"
              >Problem Name</label
            >
            <input
              id="problemName"
              v-model="editedProblem.name"
              class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
              placeholder="Enter a name for this problem"
            />
          </div>

          <div>
            <label for="topicName" class="block mb-2 font-medium">Topic</label>
            <div class="flex gap-2">
              <select
                id="topicName"
                v-model="editedProblem.topicId"
                class="flex-grow p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
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

          <!-- Difficulty dropdown for teachers -->
          <div v-if="isTeacherOrAdmin">
            <label for="difficulty" class="block mb-2 font-medium"
              >Difficulty</label
            >
            <select
              id="difficulty"
              v-model="editedProblem.difficulty"
              class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
            >
              <option value="Easy">Easy</option>
              <option value="Medium">Medium</option>
              <option value="Hard">Hard</option>
            </select>
          </div>

          <!-- Point Value for teachers -->
          <div v-if="isTeacherOrAdmin">
            <label for="pointValue" class="block mb-2 font-medium"
              >Point Value</label
            >
            <input
              id="pointValue"
              v-model.number="editedProblem.pointValue"
              type="number"
              min="1"
              max="10"
              class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
              placeholder="Points for this problem"
            />
          </div>

          <div>
            <label for="statement" class="block mb-2 font-medium"
              >Problem Statement</label
            >
            <textarea
              id="statement"
              v-model="editedProblem.statement"
              rows="4"
              class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base resize-vertical"
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
              class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
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
              class="w-full p-3 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base resize-vertical"
              placeholder="Enter the explanation"
            ></textarea>
          </div>

          <!-- Only show Correct? for user math problems, not published problems -->
          <div v-if="!isPublished">
            <label class="block mb-2 font-medium">Correct?</label>
            <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
              <label class="inline-flex items-center cursor-pointer gap-2 py-1 min-h-[44px] sm:min-h-[40px]">
                <input
                  type="radio"
                  v-model="editedProblem.isCorrect"
                  :value="true"
                  class="radio-input w-5 h-5 sm:w-[18px] sm:h-[18px] border-2 border-gray-300 rounded-full appearance-none bg-white checked:bg-blue-600 checked:border-blue-600 focus:ring-2 focus:ring-blue-500 focus:ring-offset-0 transition-all duration-200 flex-shrink-0"
                />
                <span class="text-sm sm:text-base font-medium text-gray-700 select-none">Correct</span>
              </label>
              <label class="inline-flex items-center cursor-pointer gap-2 py-1 min-h-[44px] sm:min-h-[40px]">
                <input
                  type="radio"
                  v-model="editedProblem.isCorrect"
                  :value="false"
                  class="radio-input w-5 h-5 sm:w-[18px] sm:h-[18px] border-2 border-gray-300 rounded-full appearance-none bg-white checked:bg-blue-600 checked:border-blue-600 focus:ring-2 focus:ring-blue-500 focus:ring-offset-0 transition-all duration-200 flex-shrink-0"
                />
                <span class="text-sm sm:text-base font-medium text-gray-700 select-none">Incorrect</span>
              </label>
            </div>
          </div>
        </div>

        <!-- Button section with improved mobile layout -->
        <div class="mt-6 space-y-4">
          <!-- Publish button for teachers (only for user math problems, not published problems) -->
          <div v-if="isTeacherOrAdmin && !isPublished" class="flex justify-center">
            <button
              @click="publish"
              class="btn-primary w-full sm:w-auto px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 hover:-translate-y-0.5 hover:shadow-lg transition-all duration-200 font-medium text-sm sm:text-base min-h-[44px] flex items-center justify-center shadow-md disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none disabled:shadow-md"
              :disabled="isPublishing || isSaving"
            >
              {{ isPublishing ? "Publishing..." : "Publish as Math Problem" }}
            </button>
          </div>

          <!-- Action buttons row -->
          <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
            <button
              @click="deleteProblem"
              class="flex-1 sm:flex-none px-6 py-3 bg-gray-500 text-white rounded-md hover:bg-gray-600 hover:-translate-y-0.5 hover:shadow-lg transition-all duration-200 font-medium text-sm sm:text-base min-h-[44px] flex items-center justify-center shadow-md"
            >
              Delete Problem
            </button>

            <div class="flex flex-col sm:flex-row gap-3 sm:gap-4 flex-1 sm:flex-none">
              <button
                @click="cancel"
                class="flex-1 sm:flex-none px-6 py-3 border-2 border-blue-300 text-blue-600 bg-white rounded-md hover:bg-blue-50 hover:-translate-y-0.5 hover:shadow-lg transition-all duration-200 font-medium text-sm sm:text-base min-h-[44px] flex items-center justify-center shadow-md"
              >
                Cancel
              </button>
              <button
                @click="save"
                class="btn-primary flex-1 sm:flex-none px-6 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 hover:-translate-y-0.5 hover:shadow-lg transition-all duration-200 font-medium text-sm sm:text-base min-h-[44px] flex items-center justify-center shadow-md disabled:opacity-60 disabled:cursor-not-allowed disabled:transform-none disabled:shadow-md"
                :disabled="isSaving"
              >
                {{ isSaving ? "Saving..." : "Save Changes" }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, watch } from "vue";
import {
  updateUserMathProblem,
  publishUserMathProblem,
  updateMathProblem,
  deleteUserMathProblem,
  deleteMathProblem,
} from "@/api/math";

export default {
  name: "EditMathProblem",
  props: {
    show: {
      type: Boolean,
      default: false,
    },
    problem: {
      type: Object,
      default: () => ({}),
    },
    topics: {
      type: Array,
      default: () => [],
    },
    isTeacherOrAdmin: {
      type: Boolean,
      default: false,
    },
    // Flag to indicate if this is a published math problem (vs user math problem)
    isPublishedProblem: {
      type: Boolean,
      default: false,
    },
  },
  emits: [
    "update:show",
    "problem-saved",
    "problem-published",
    "problem-deleted",
    "cancel",
  ],
  setup(props, { emit }) {
    const editedProblem = ref({});
    const isSaving = ref(false);
    const isPublishing = ref(false);

    // Computed property to determine if this is a published problem
    const isPublished = computed(() => {
      return props.isPublishedProblem || false;
    });

    // Watch for changes in the problem prop to update the local copy
    watch(
      () => props.problem,
      (newProblem) => {
        if (newProblem && Object.keys(newProblem).length > 0) {
          editedProblem.value = { ...newProblem };

          // If this is a UserMathProblem (not published), try to get the name from localStorage
          if (!isPublished.value && newProblem.id) {
            const savedName = localStorage.getItem(
              `userMathProblem_${newProblem.id}_name`
            );
            if (savedName) {
              editedProblem.value.name = savedName;
            } else if (!editedProblem.value.name) {
              // Set a default name based on topic if no saved name exists
              editedProblem.value.name = `${editedProblem.value.topicName || "Math"} Problem`;
            }
          }
        }
      },
      { immediate: true, deep: true }
    );

    const cancel = () => {
      emit("update:show", false);
      emit("cancel");
    };

    const save = async () => {
      isSaving.value = true;

      try {
        // Update topic name based on selected topic ID
        const selectedTopic = props.topics.find(
          (t) => t.id === editedProblem.value.topicId
        );
        if (selectedTopic) {
          editedProblem.value.topicName = selectedTopic.name;
        }

        // Ensure problem has a name (use topic name as fallback)
        if (
          !editedProblem.value.name ||
          editedProblem.value.name.trim() === ""
        ) {
          editedProblem.value.name = `${editedProblem.value.topicName || "Math"} Problem`;
        }

        if (isPublished.value) {
          // For published math problems, use the updateMathProblem API
          const updateData = {
            name: editedProblem.value.name,
            statement: editedProblem.value.statement,
            solution: editedProblem.value.solution,
            explanation: editedProblem.value.explanation,
            difficulty: editedProblem.value.difficulty,
            topicId: editedProblem.value.topicId,
            pointValue: editedProblem.value.pointValue || 1,
          };

          await updateMathProblem(editedProblem.value.id, updateData);
        } else {
          // For user math problems, use the updateUserMathProblem API
          // Create a copy of the problem with the name property
          // This is our workaround since UserMathProblem doesn't have a name field
          const userProblemData = { ...editedProblem.value };

          // Store the name in localStorage for this problem ID
          localStorage.setItem(
            `userMathProblem_${editedProblem.value.id}_name`,
            editedProblem.value.name
          );

          await updateUserMathProblem(editedProblem.value.id, userProblemData);
        }

        emit("problem-saved", editedProblem.value);
        emit("update:show", false);
      } catch (error) {
        console.error("Error saving problem:", error);
        alert("Failed to save changes. Please try again.");
      } finally {
        isSaving.value = false;
      }
    };

    const publish = async () => {
      // Validate required fields
      if (!editedProblem.value.topicId) {
        alert("Please select a topic before publishing.");
        return;
      }

      // Ensure point value is set and valid
      if (
        !editedProblem.value.pointValue ||
        editedProblem.value.pointValue < 1
      ) {
        alert("Please set a valid point value (minimum 1) before publishing.");
        return;
      }

      // Ensure problem has a name (use topic name as fallback)
      if (!editedProblem.value.name || editedProblem.value.name.trim() === "") {
        const selectedTopic = props.topics.find(
          (t) => t.id === editedProblem.value.topicId
        );
        const topicName = selectedTopic ? selectedTopic.name : "Math";
        editedProblem.value.name = `${topicName} Problem`;
      }

      isPublishing.value = true;

      try {
        // Store the name in localStorage before saving
        localStorage.setItem(
          `userMathProblem_${editedProblem.value.id}_name`,
          editedProblem.value.name
        );

        // First save any changes to the problem
        await updateUserMathProblem(
          editedProblem.value.id,
          editedProblem.value
        );

        // Then publish it as a curated MathProblem
        const response = await publishUserMathProblem(editedProblem.value.id);

        if (response && response.success) {
          alert("Problem successfully published as a curated Math Problem!");
          emit("problem-published", editedProblem.value);
          emit("update:show", false);
        } else {
          alert("Failed to publish problem. " + (response?.message || ""));
        }
      } catch (error) {
        console.error("Error publishing problem:", error);
        alert("Failed to publish problem. Please try again.");
      } finally {
        isPublishing.value = false;
      }
    };

    const deleteProblem = async () => {
      if (
        confirm(
          "Are you sure you want to delete this problem? This action cannot be undone."
        )
      ) {
        try {
          if (isPublished.value) {
            // Delete published math problem
            await deleteMathProblem(editedProblem.value.id);
          } else {
            // Delete user math problem
            await deleteUserMathProblem(editedProblem.value.id);
          }

          // Remove the name from localStorage if it exists
          if (!isPublished.value && editedProblem.value.id) {
            localStorage.removeItem(
              `userMathProblem_${editedProblem.value.id}_name`
            );
          }

          alert("Problem successfully deleted!");
          emit("problem-deleted", editedProblem.value.id);
          emit("update:show", false);
        } catch (error) {
          console.error("Error deleting problem:", error);
          alert("Failed to delete problem. Please try again.");
        }
      }
    };

    return {
      editedProblem,
      isSaving,
      isPublishing,
      isPublished,
      cancel,
      save,
      publish,
      deleteProblem,
    };
  },
};
</script>

<style scoped>
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

/* Blue gradient for primary buttons - enhanced theme */
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
