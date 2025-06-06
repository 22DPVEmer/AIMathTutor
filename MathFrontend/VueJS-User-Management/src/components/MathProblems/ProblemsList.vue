<template>
  <div class="problems-list">
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold">{{ topicName }}</h2>
      <button
        @click="goBackToTopics"
        class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors flex items-center shadow-md"
      >
        <span class="mr-1 ">←</span> Back to Topics
      </button>
    </div>

    <!-- Loading Indicator -->
    <div v-if="loading" class="text-center py-8">
      <div
        class="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"
      ></div>
      <p class="mt-2 text-gray-600">Loading problems...</p>
    </div>

    <!-- Problems List -->
    <div v-else>
      <div v-if="problems.length > 0">
        <h3 class="font-bold mb-4">Available Problems for {{ topicName }}</h3>

        <!-- Desktop Table View -->
        <div class="hidden lg:block overflow-x-auto">
          <table class="min-w-full bg-white rounded-lg overflow-hidden">
            <thead class="bg-gray-100">
              <tr>
                <th class="py-3 px-4 text-left">Problem</th>
                <th class="py-3 px-4 text-left">Difficulty</th>
                <th class="py-3 px-4 text-left">Max Points</th>
                <th class="py-3 px-4 text-left">Earned/Total</th>
                <th class="py-3 px-4 text-left">Action</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(problem, index) in problems"
                :key="index"
                class="border-t hover:bg-gray-50"
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
                      'bg-green-100 text-green-800': problem.directlyCompleted,
                      'bg-blue-100 text-blue-800': problem.indirectlyCompleted,
                      'bg-yellow-100 text-yellow-800':
                        problem.attempted && !problem.completed,
                      'bg-gray-100 text-gray-800':
                        !problem.attempted && !problem.completed,
                    }"
                    :title="
                      problem.directlyCompleted
                        ? 'You have completed this problem'
                        : problem.indirectlyCompleted
                          ? 'You have completed a similar problem'
                          : problem.attempted
                            ? 'You have attempted this problem'
                            : 'Not attempted'
                    "
                  >
                    {{
                      problem.directlyCompleted ? problem.pointsEarned : "0"
                    }}/{{ problem.pointValue }}
                  </span>
                </td>
                <td class="py-3 px-4">
                  <div class="flex space-x-2">
                    <button
                      @click="solveProblem(problem)"
                      class="px-3 py-1 bg-blue-600 text-white rounded-md hover:bg-blue-700 text-xs"
                    >
                      Solve
                    </button>
                    <button
                      v-if="isTeacherOrAdmin"
                      @click="editProblem(problem)"
                      class="px-3 py-1 bg-blue-600 text-white rounded-md hover:bg-blue-700 text-xs"
                    >
                      Edit
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Mobile Card View -->
        <div class="lg:hidden space-y-4">
          <div
            v-for="(problem, index) in problems"
            :key="index"
            class="bg-white rounded-lg shadow-sm border border-gray-200 p-4"
          >
            <div class="flex justify-between items-start mb-3">
              <div class="flex-1 pr-3">
                <p class="text-sm text-gray-900 font-medium leading-5">
                  {{ problem.statement.substring(0, 80)
                  }}{{ problem.statement.length > 80 ? "..." : "" }}
                </p>
              </div>
              <span
                class="difficulty-badge px-2 py-1 rounded-full text-xs flex-shrink-0"
                :class="getDifficultyClass(problem.difficulty)"
              >
                {{ getDifficultyLabel(problem.difficulty) }}
              </span>
            </div>

            <div class="flex justify-between items-center mb-3">
              <div class="flex flex-col space-y-1 text-sm text-gray-600">
                <span>
                  <span class="font-medium">Points:</span> {{ problem.pointValue }}
                </span>
                <span>
                  <span class="font-medium">Progress:</span>
                  <span
                    class="px-2 py-1 rounded-full text-xs ml-1"
                    :class="{
                      'bg-green-100 text-green-800': problem.directlyCompleted,
                      'bg-blue-100 text-blue-800': problem.indirectlyCompleted,
                      'bg-yellow-100 text-yellow-800':
                        problem.attempted && !problem.completed,
                      'bg-gray-100 text-gray-800':
                        !problem.attempted && !problem.completed,
                    }"
                  >
                    {{
                      problem.directlyCompleted ? problem.pointsEarned : "0"
                    }}/{{ problem.pointValue }}
                  </span>
                </span>
              </div>
            </div>

            <div class="flex space-x-2">
              <button
                @click="solveProblem(problem)"
                class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors text-sm font-medium"
              >
                Solve Problem
              </button>
              <button
                v-if="isTeacherOrAdmin"
                @click="editProblem(problem)"
                class="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors text-sm font-medium"
              >
                Edit
              </button>
            </div>
          </div>
        </div>
      </div>
      <div v-else class="text-center py-8 text-gray-500">
        No problems available for this topic.
      </div>
    </div>

    <!-- Edit Problem Dialog -->
    <EditMathProblem
      v-model:show="editingProblem"
      :problem="editedProblem"
      :topics="topics"
      :is-teacher-or-admin="isTeacherOrAdmin"
      :is-published-problem="true"
      @problem-saved="handleProblemSaved"
      @problem-deleted="handleProblemDeleted"
      @cancel="cancelEdit"
    />
  </div>
</template>

<script>
import { ref, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { api } from "@/api/user";
import { useStore } from "@/store";
import EditMathProblem from "@/components/EditMathProblem.vue";
import { getAllTopics, updateMathProblem } from "@/api/math";

export default {
  name: "ProblemsList",
  components: {
    EditMathProblem,
  },
  props: {
    topicId: {
      type: [Number, String],
      required: true,
    },
  },

  setup(props) {
    const router = useRouter();
    const store = useStore();
    const loading = ref(true);
    const problems = ref([]);
    const topicName = ref("");
    const topics = ref([]);
    const editingProblem = ref(false);
    const editedProblem = ref({});

    // Check if user is teacher or admin
    const isTeacherOrAdmin = computed(() => {
      const userData = store.getters["user/userData"];
      return (
        userData &&
        userData.roles &&
        (userData.roles.includes("Teacher") || userData.roles.includes("Admin"))
      );
    });

    // Helper function to get difficulty value for sorting
    function getDifficultyValue(difficulty) {
      if (typeof difficulty === "string") {
        switch (difficulty.toLowerCase()) {
          case "easy":
            return 1;
          case "medium":
            return 2;
          case "hard":
            return 3;
          default:
            return 0;
        }
      } else {
        return difficulty || 0;
      }
    }

    async function fetchProblems() {
      loading.value = true;
      try {
        // Fetch topic details to get the name
        const topicResponse = await api.get(`/mathtopic/${props.topicId}`);
        topicName.value = topicResponse.data.name;

        // Fetch problems for this topic
        const response = await api.get(`/mathproblem/topic/${props.topicId}`);
        let fetchedProblems = response.data;

        // Sort problems by difficulty (easy → medium → hard)
        fetchedProblems.sort((a, b) => {
          return (
            getDifficultyValue(a.difficulty) - getDifficultyValue(b.difficulty)
          );
        });

        problems.value = fetchedProblems;

        try {
          // Check if user is logged in before trying to fetch attempts
          const token = localStorage.getItem("token");
          if (!token) {
            console.log("User not logged in, skipping attempts fetch");
            // Set default values for problems
            problems.value = problems.value.map((problem) => ({
              ...problem,
              completed: false,
              attempted: false,
              pointsEarned: 0,
            }));
            return; // Exit early
          }

          // Get user attempts for these problems
          const userAttempts = await api.get("/mathproblemAttempt");

          // First, create maps to track which statements have been completed or attempted
          const completedStatements = new Map();
          const attemptedStatements = new Map();
          const directlyCompletedProblemIds = new Set();

          // Process all attempts to build maps of completed and attempted statements
          userAttempts.data.forEach((attempt) => {
            if (!attempt.problemStatement) return;

            // Track directly completed problems by ID
            if (attempt.isCorrect) {
              directlyCompletedProblemIds.add(attempt.problemId);
            }

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

            // Track attempted statements
            if (!attemptedStatements.has(normalizedStatement)) {
              attemptedStatements.set(normalizedStatement, true);
            }
          });

          console.log(
            `Found ${directlyCompletedProblemIds.size} directly completed problems, ${completedStatements.size} unique completed statements, and ${attemptedStatements.size} attempted statements`
          );

          // Mark problems as completed or attempted based on user attempts
          problems.value = problems.value.map((problem) => {
            // Normalize the problem statement
            const normalizedStatement = problem.statement
              .toLowerCase()
              .replace(/\s+/g, "");

            // Check if this problem was directly completed by the user
            const isDirectlyCompleted = directlyCompletedProblemIds.has(
              problem.id
            );

            // First try to find a successful attempt by exact ID match
            let successfulAttempt = userAttempts.data.find(
              (a) => a.problemId === problem.id && a.isCorrect
            );

            // If no direct match, check if any problem with the same statement has been completed
            let isIndirectlyCompleted = false;
            if (
              !successfulAttempt &&
              completedStatements.has(normalizedStatement)
            ) {
              isIndirectlyCompleted = true;

              // Get the points earned for this statement
              const completionInfo =
                completedStatements.get(normalizedStatement);

              // Find the actual attempt for logging purposes
              successfulAttempt = userAttempts.data.find(
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

            // Check for any attempt by ID
            let anyAttempt = !successfulAttempt
              ? userAttempts.data.find((a) => a.problemId === problem.id)
              : null;

            // If no direct attempt match, check if any problem with the same statement has been attempted
            if (!anyAttempt && attemptedStatements.has(normalizedStatement)) {
              anyAttempt = { isCorrect: false };
            }

            // Use the successful attempt if available, otherwise use any attempt
            const bestAttempt = successfulAttempt || anyAttempt;

            // Log for debugging
            if (successfulAttempt) {
              console.log(
                `Problem ${problem.id} (${problem.statement.substring(0, 20)}...) has a ${isDirectlyCompleted ? "direct" : "indirect"} successful attempt. Points earned: ${isDirectlyCompleted ? successfulAttempt.pointsEarned : 0}`
              );
            }

            return {
              ...problem,
              completed: !!successfulAttempt,
              directlyCompleted: isDirectlyCompleted,
              indirectlyCompleted: isIndirectlyCompleted,
              attempted: !!bestAttempt,
              pointsEarned: isDirectlyCompleted
                ? successfulAttempt?.pointsEarned || 0
                : 0,
            };
          });
        } catch (attemptError) {
          console.warn("Could not fetch user attempts:", attemptError);

          // Check if this is an authentication error
          if (attemptError.response && attemptError.response.status === 401) {
            console.warn(
              "Authentication required to view attempts. User may not be logged in."
            );
          }

          // Continue without user attempt data
          problems.value = problems.value.map((problem) => ({
            ...problem,
            completed: false,
            attempted: false,
            pointsEarned: 0,
          }));
        }
      } catch (error) {
        console.error("Error fetching problems:", error);
        problems.value = []; // Clear problems on error
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

    function solveProblem(problem) {
      router.push({
        name: "ProblemView",
        params: {
          topicId: props.topicId,
          problemId: problem.id,
        },
      });
    }

    async function goBackToTopics() {
      try {
        // Fetch topic details to check if it has a parent
        const topicResponse = await api.get(`/mathtopic/${props.topicId}`);
        const topic = topicResponse.data;

        if (topic.parentTopicId) {
          // If it has a parent, go back to the subtopics view
          router.push({
            name: "SubtopicsList",
            params: { parentId: topic.parentTopicId },
          });
        } else {
          // If it's a parent topic, go back to the main topics list
          router.push({ name: "TopicsList" });
        }
      } catch (error) {
        console.error("Error fetching topic details:", error);
        // Fallback to main topics list if there's an error
        router.push({ name: "TopicsList" });
      }
    }

    // Edit problem functions
    const editProblem = (problem) => {
      // Fetch topics if not already loaded
      if (topics.value.length === 0) {
        fetchTopics();
      }

      // Convert the problem to the format expected by EditMathProblem
      editedProblem.value = {
        id: problem.id,
        statement: problem.statement,
        solution: problem.solution,
        explanation: problem.explanation,
        difficulty: problem.difficulty,
        topicId: problem.topicId,
        topicName: problem.topicName,
        pointValue: problem.pointValue || 1,
      };

      editingProblem.value = true;
    };

    const cancelEdit = () => {
      editingProblem.value = false;
      editedProblem.value = {};
    };

    const handleProblemSaved = async (savedProblem) => {
      try {
        // Convert to the format expected by the API
        const updateData = {
          name: savedProblem.name || savedProblem.topicName + " Problem",
          statement: savedProblem.statement,
          solution: savedProblem.solution,
          explanation: savedProblem.explanation,
          difficulty: savedProblem.difficulty,
          topicId: savedProblem.topicId,
          pointValue: savedProblem.pointValue || 1,
        };

        // Update the problem in the database
        await updateMathProblem(savedProblem.id, updateData);

        // Update the problem in the local array without fetching from the server
        const index = problems.value.findIndex((p) => p.id === savedProblem.id);
        if (index !== -1) {
          problems.value[index] = {
            ...problems.value[index],
            ...savedProblem,
          };
        }

        // Close the edit dialog
        editingProblem.value = false;
      } catch (error) {
        console.error("Error saving problem:", error);
        alert("Failed to save changes. Please try again.");
      }
    };

    // Handler for when a problem is deleted in the EditMathProblem component
    const handleProblemDeleted = (problemId) => {
      // Remove the problem from the local array
      problems.value = problems.value.filter(
        (problem) => problem.id !== problemId
      );
    };

    // Fetch all topics for the edit form
    const fetchTopics = async () => {
      try {
        topics.value = await getAllTopics();
      } catch (error) {
        console.error("Error fetching topics:", error);
        topics.value = [];
      }
    };

    onMounted(async () => {
      // Make sure user profile is loaded to check roles
      try {
        await store.dispatch("user/getUserProfile");
      } catch (error) {
        console.error("Error loading user profile:", error);
      }

      fetchProblems();
    });

    return {
      loading,
      problems,
      topicName,
      getDifficultyClass,
      getDifficultyLabel,
      solveProblem,
      goBackToTopics,
      isTeacherOrAdmin,
      editProblem,
      editingProblem,
      editedProblem,
      cancelEdit,
      handleProblemSaved,
      handleProblemDeleted,
      topics,
    };
  },
};
</script>

<style scoped>
.problems-list {
  max-width: 1000px;
  margin: 0 auto;
}
</style>
