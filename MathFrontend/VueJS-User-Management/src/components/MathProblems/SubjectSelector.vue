<template>
  <div class="topic-selector">
    <h2 class="text-2xl font-bold mb-6">Math Topics</h2>

    <!-- School Class Navigation Tabs -->
    <div class="school-class-tabs mb-6">
      <div class="flex border-b">
        <button
          v-for="(schoolClass, index) in schoolClasses"
          :key="index"
          @click="selectSchoolClass(schoolClass)"
          :class="[
            'px-4 py-2 text-center',
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
          class="parent-topic p-4 bg-gray-100 rounded-lg mb-4 cursor-pointer"
          @click="toggleParentTopic(parentTopic)"
        >
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
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div
            v-for="(subtopic, subtopicIndex) in getSubtopics(parentTopic.id)"
            :key="subtopicIndex"
            @click="selectTopic(subtopic)"
            class="subtopic-card border rounded-lg p-4 cursor-pointer hover:border-blue-500 transition-colors"
            :class="{
              'border-blue-500 bg-blue-50': selectedTopic.id === subtopic.id,
            }"
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
      </div>

      <!-- No topics message -->
      <div
        v-if="parentTopics.length === 0"
        class="text-center py-8 text-gray-500"
      >
        No topics available for this class.
      </div>
    </div>

    <!-- Difficulty Selector -->

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
              <th class="py-3 px-4 text-left">Max Points</th>
              <th class="py-3 px-4 text-left">Earned/Total</th>
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
import { api } from "@/api/user";
import { getTopicCompletion } from "@/api/math";

export default {
  name: "SubjectSelector",

  emits: ["topic-selected", "generate-problem", "solve-problem"],

  setup(props, { emit }) {
    const loading = ref(false);
    const schoolClasses = ref([]);
    const topics = ref([]);
    const problems = ref([]);

    const selectedSchoolClass = ref({});
    const selectedTopic = ref({});
    const selectedDifficulty = ref("");
    const expandedParentTopics = ref(new Set());

    const difficulties = [
      { value: "easy", label: "Easy" },
      { value: "medium", label: "Medium" },
      { value: "hard", label: "Hard" },
    ];

    const filteredTopics = computed(() => {
      if (!selectedSchoolClass.value || !selectedSchoolClass.value.id)
        return [];
      return topics.value.filter(
        (topic) => topic.schoolClassId === selectedSchoolClass.value.id
      );
    });

    const parentTopics = computed(() => {
      return filteredTopics.value.filter((topic) => !topic.parentTopicId);
    });

    const canGenerateProblem = computed(() => {
      return selectedTopic.value.id && selectedDifficulty.value;
    });

    // Get subtopics for a specific parent topic
    function getSubtopics(parentId) {
      return filteredTopics.value.filter(
        (topic) => topic.parentTopicId === parentId
      );
    }

    function toggleParentTopic(parentTopic) {
      const hasSubtopics = getSubtopics(parentTopic.id).length > 0;

      if (hasSubtopics) {
        // If it has subtopics, just toggle the expanded state
        if (expandedParentTopics.value.has(parentTopic.id)) {
          expandedParentTopics.value.delete(parentTopic.id);
        } else {
          expandedParentTopics.value.add(parentTopic.id);
        }
      } else {
        // If it doesn't have subtopics, select it as the current topic
        selectTopic(parentTopic);
      }
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
        console.error("Error fetching school classes:", error);
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
            console.log("Fetching topic completion data from backend API...");
            console.time("Topic completion fetch");
            const completionData = await getTopicCompletion();
            console.timeEnd("Topic completion fetch");

            // Update topics with completion data
            if (completionData && completionData.length > 0) {
              console.log("Received topic completion data:", completionData);
              console.log(
                `Received completion data for ${completionData.length} topics`
              );

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

              console.log(
                `Overall progress: ${totalPointsEarned}/${totalPointsPossible} points (${overallPercentage}%)`
              );

              // Update each topic with its completion data
              topics.value = topics.value.map((topic) => {
                const topicCompletion = completionData.find(
                  (tc) => tc.topicId === topic.id
                );
                if (topicCompletion) {
                  console.log(
                    `Topic ${topic.id} (${topic.name}): ${topicCompletion.pointsEarned}/${topicCompletion.totalPointsPossible} points, ${topicCompletion.percentageCompleted}% complete`
                  );
                  return {
                    ...topic,
                    totalPointsPossible: topicCompletion.totalPointsPossible,
                    pointsEarned: topicCompletion.pointsEarned,
                    percentageCompleted: topicCompletion.percentageCompleted,
                  };
                } else {
                  console.log(
                    `No completion data found for topic ${topic.id} (${topic.name})`
                  );
                  return topic;
                }
              });
            } else {
              console.log("No topic completion data received");
            }
          } else {
            console.log("User not logged in, skipping topic completion fetch");
          }
        } catch (completionError) {
          console.warn(
            "Error fetching topic completion data:",
            completionError
          );
        }
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
        const response = await api.get(
          `/mathproblem/topic/${selectedTopic.value.id}`
        );
        problems.value = response.data;

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

            // Update topic completion data
            updateTopicCompletionData();
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

          // Update topic completion data
          updateTopicCompletionData(userAttempts.data);
        } catch (attemptError) {
          console.warn("Could not fetch user attempts:", attemptError);

          // Check if this is an authentication error
          if (attemptError.response && attemptError.response.status === 401) {
            console.warn(
              "Authentication required to view attempts. User may not be logged in."
            );
            // You could redirect to login or show a message here
          }

          // Continue without user attempt data
          problems.value = problems.value.map((problem) => ({
            ...problem,
            completed: false,
            attempted: false,
            pointsEarned: 0,
          }));

          // Update topic completion data
          updateTopicCompletionData();
        }
      } catch (error) {
        console.error("Error fetching problems:", error);
        problems.value = []; // Clear problems on error

        // Update topic completion data
        updateTopicCompletionData();
      } finally {
        loading.value = false;
      }
    }

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

        // If this is the selected topic, we already have the problems
        if (selectedTopic.value && selectedTopic.value.id === topic.id) {
          topicProblems = problems.value;
        } else {
          // Otherwise, fetch the problems for this topic
          try {
            const response = await api.get(`/mathproblem/topic/${topic.id}`);
            topicProblems = response.data;
          } catch (error) {
            console.error(
              `Error fetching problems for topic ${topic.id}:`,
              error
            );
            topicProblems = [];
          }
        }

        // Calculate total points possible
        const totalPointsPossible = topicProblems.reduce(
          (sum, problem) => sum + (problem.pointValue || 1),
          0
        );

        // Calculate points earned
        let pointsEarned = 0;

        if (userAttempts && userAttempts.length > 0) {
          console.log(
            `Calculating points for topic ${topic.id} (${topic.name})`
          );
          console.log(
            `Found ${topicProblems.length} problems and ${userAttempts.length} user attempts`
          );

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

          console.log(
            `Found ${completedStatements.size} unique completed statements`
          );

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
              console.log(
                `Problem statement "${problem.statement.substring(0, 20)}..." already counted, skipping`
              );
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

                console.log(
                  `Problem ${problem.id} (${problem.statement.substring(0, 20)}...) earned ${points} points`
                );
              } else {
                console.log(`Problem ${problem.id} already counted, skipping`);
              }
            }
          });

          console.log(
            `Total points earned for topic ${topic.id}: ${pointsEarned}`
          );
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
        console.error(
          `Error calculating completion for topic ${topic.id}:`,
          error
        );
        return {
          ...topic,
          totalPointsPossible: 0,
          pointsEarned: 0,
          percentageCompleted: 0,
        };
      }
    }

    function selectSchoolClass(schoolClass) {
      selectedSchoolClass.value = schoolClass;
      selectedTopic.value = {};
      selectedDifficulty.value = "";
      problems.value = [];
      fetchTopics();
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
          console.warn("Error calculating initial topic completion:", error);
          updateTopicCompletionData();
        }
      }

      // Listen for refresh-problems event
      document.addEventListener("refresh-problems", async (event) => {
        if (event.detail && event.detail.topicId) {
          // Find the topic with the given ID
          const topic = topics.value.find((t) => t.id === event.detail.topicId);
          if (topic) {
            // If this is already the selected topic, just refresh the problems
            if (selectedTopic.value && selectedTopic.value.id === topic.id) {
              console.log("Refreshing problems for current topic:", topic.name);
              await fetchProblems();
            } else {
              // Otherwise, select the topic which will also fetch problems
              console.log(
                "Selecting topic and refreshing problems:",
                topic.name
              );
              await selectTopic(topic);
            }
          }
        } else {
          // If no specific topic ID is provided, just refresh the current topic's problems
          if (selectedTopic.value && selectedTopic.value.id) {
            console.log(
              "Refreshing problems for current topic (no ID provided)"
            );
            await fetchProblems();
          }
        }
      });
    });

    return {
      loading,
      schoolClasses,
      topics,
      problems,
      filteredTopics,
      parentTopics,
      difficulties,
      selectedSchoolClass,
      selectedTopic,
      selectedDifficulty,
      expandedParentTopics,
      canGenerateProblem,
      getSubtopics,
      toggleParentTopic,
      selectSchoolClass,
      selectTopic,
      selectDifficulty,
      getDifficultyClass,
      getDifficultyLabel,
      generateProblem,
      solveProblem,
      updateTopicCompletionData,
      calculateTopicCompletion,
    };
  },
};
</script>

<style scoped>
.topic-selector {
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
