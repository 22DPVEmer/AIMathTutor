<template>
  <div
    v-if="show"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50"
  >
    <div
      class="bg-white rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-y-auto"
    >
      <div class="p-6">
        <div class="flex justify-between items-center mb-6">
          <h2 class="text-2xl font-bold">Edit Math Problem</h2>
          <button
            @click="cancel"
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

          <!-- Difficulty dropdown for teachers -->
          <div v-if="isTeacherOrAdmin">
            <label for="difficulty" class="block mb-2 font-medium"
              >Difficulty</label
            >
            <select
              id="difficulty"
              v-model="editedProblem.difficulty"
              class="w-full p-2 border rounded-md"
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
              class="w-full p-2 border rounded-md"
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

        <div class="flex justify-between gap-2 mt-6">
          <!-- Publish button for teachers -->
          <div v-if="isTeacherOrAdmin">
            <div class="flex gap-2">
              <button
                @click="publish"
                class="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700"
                :disabled="isPublishing || isSaving"
              >
                {{
                  isPublishing ? "Publishing..." : "Publish as Math Problem"
                }}
              </button>
            </div>
          </div>
          <div v-else class="flex-grow"></div>

          <div class="flex gap-2">
            <button
              @click="cancel"
              class="px-4 py-2 border border-gray-300 rounded-md hover:bg-gray-100"
            >
              Cancel
            </button>
            <button
              @click="save"
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
import { ref, computed, watch } from "vue";
import { updateUserMathProblem, publishUserMathProblem } from "@/api/math";

export default {
  name: "EditMathProblem",
  props: {
    show: {
      type: Boolean,
      default: false
    },
    problem: {
      type: Object,
      default: () => ({})
    },
    topics: {
      type: Array,
      default: () => []
    },
    isTeacherOrAdmin: {
      type: Boolean,
      default: false
    }
  },
  emits: ["update:show", "problem-saved", "problem-published", "cancel"],
  setup(props, { emit }) {
    const editedProblem = ref({});
    const isSaving = ref(false);
    const isPublishing = ref(false);

    // Watch for changes in the problem prop to update the local copy
    watch(() => props.problem, (newProblem) => {
      if (newProblem && Object.keys(newProblem).length > 0) {
        editedProblem.value = { ...newProblem };
      }
    }, { immediate: true, deep: true });

    const cancel = () => {
      emit("update:show", false);
      emit("cancel");
    };

    const save = async () => {
      isSaving.value = true;

      try {
        await updateUserMathProblem(
          editedProblem.value.id,
          editedProblem.value
        );

        // Update topic name based on selected topic ID
        const selectedTopic = props.topics.find(
          (t) => t.id === editedProblem.value.topicId
        );
        if (selectedTopic) {
          editedProblem.value.topicName = selectedTopic.name;
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

      isPublishing.value = true;

      try {
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

    return {
      editedProblem,
      isSaving,
      isPublishing,
      cancel,
      save,
      publish
    };
  }
};
</script>
