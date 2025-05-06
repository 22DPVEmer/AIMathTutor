import { Module } from "vuex";
import { RootState } from "..";
import * as mathApi from "@/api/math";

// Define the MathState interface
export interface MathState {
  topics: any[];
  topicCompletion: any[];
  userProblems: any[];
  recommendations: any[];
  loading: boolean;
  error: string | null;
}

// Create the Vuex module with proper typing
const mathModule: Module<MathState, RootState> = {
  namespaced: true,

  state: (): MathState => ({
    topics: [],
    topicCompletion: [],
    userProblems: [],
    recommendations: [],
    loading: false,
    error: null,
  }),

  mutations: {
    SET_TOPICS(state: MathState, topics: any[]): void {
      state.topics = topics;
    },

    SET_TOPIC_COMPLETION(state: MathState, completion: any[]): void {
      state.topicCompletion = completion;
    },

    SET_USER_PROBLEMS(state: MathState, problems: any[]): void {
      state.userProblems = problems;
    },

    SET_RECOMMENDATIONS(state: MathState, recommendations: any[]): void {
      state.recommendations = recommendations;
    },

    SET_LOADING(state: MathState, loading: boolean): void {
      state.loading = loading;
    },

    SET_ERROR(state: MathState, error: string | null): void {
      state.error = error;
    },
  },

  actions: {
    async fetchTopics({ commit }): Promise<void> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const topics = await mathApi.getAllTopics();
        commit("SET_TOPICS", topics);
        commit("SET_LOADING", false);
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error ? error.message : "Failed to fetch topics";
        commit("SET_ERROR", errorMessage);
        console.error("Error fetching topics:", error);
      }
    },

    async fetchTopicCompletion({ commit }): Promise<void> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const completion = await mathApi.getTopicCompletion();
        commit("SET_TOPIC_COMPLETION", completion);
        commit("SET_LOADING", false);
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error
            ? error.message
            : "Failed to fetch topic completion";
        commit("SET_ERROR", errorMessage);
        console.error("Error fetching topic completion:", error);
      }
    },

    async fetchUserProblems({ commit }): Promise<void> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        const problems = await mathApi.getCurrentUserMathProblems();
        commit("SET_USER_PROBLEMS", problems);
        commit("SET_LOADING", false);
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error
            ? error.message
            : "Failed to fetch user problems";
        commit("SET_ERROR", errorMessage);
        console.error("Error fetching user problems:", error);
      }
    },

    async fetchRecommendations({ commit }): Promise<void> {
      commit("SET_LOADING", true);
      commit("SET_ERROR", null);

      try {
        // This is a placeholder - in a real app, you would call an API endpoint
        // that returns personalized recommendations based on the user's progress
        const mockRecommendations = [
          {
            title: "Practice Algebra",
            description:
              "Based on your recent progress, we recommend practicing more algebra problems.",
            link: "/practice?topic=algebra",
          },
          {
            title: "Try Geometry",
            description: "You haven't explored geometry much. Give it a try!",
            link: "/practice?topic=geometry",
          },
        ];

        // Simulate API delay
        await new Promise((resolve) => setTimeout(resolve, 500));

        commit("SET_RECOMMENDATIONS", mockRecommendations);
        commit("SET_LOADING", false);
      } catch (error) {
        commit("SET_LOADING", false);
        const errorMessage =
          error instanceof Error
            ? error.message
            : "Failed to fetch recommendations";
        commit("SET_ERROR", errorMessage);
        console.error("Error fetching recommendations:", error);
      }
    },
  },

  getters: {
    getTopics(state: MathState): any[] {
      return state.topics;
    },

    getTopicCompletion(state: MathState): any[] {
      // Sort topic completion by percentage completed (descending)
      return [...state.topicCompletion].sort((a, b) => {
        // First sort by progress percentage (descending)
        if (b.percentageCompleted !== a.percentageCompleted) {
          return b.percentageCompleted - a.percentageCompleted;
        }
        // Then by points earned (descending)
        if (b.pointsEarned !== a.pointsEarned) {
          return b.pointsEarned - a.pointsEarned;
        }
        // Finally by name (alphabetical)
        return a.topicName.localeCompare(b.topicName);
      });
    },

    getUserProblems(state: MathState): any[] {
      return state.userProblems;
    },

    getRecommendations(state: MathState): any[] {
      return state.recommendations;
    },

    getTopicById: (state: MathState) => (id: number) => {
      return state.topics.find((topic) => topic.id === id);
    },

    getTopicCompletionById: (state: MathState) => (id: number) => {
      return state.topicCompletion.find((topic) => topic.topicId === id);
    },

    getOverallProgress(state: MathState): number {
      if (state.topicCompletion.length === 0) return 0;

      const totalPoints = state.topicCompletion.reduce(
        (sum, topic) => sum + topic.totalPointsPossible,
        0
      );
      const earnedPoints = state.topicCompletion.reduce(
        (sum, topic) => sum + topic.pointsEarned,
        0
      );

      return totalPoints > 0
        ? Math.round((earnedPoints / totalPoints) * 100)
        : 0;
    },

    getMasteredTopics(state: MathState): number {
      return state.topicCompletion.filter(
        (topic) => topic.percentageCompleted >= 100
      ).length;
    },

    getTotalTopics(state: MathState): number {
      return state.topicCompletion.length;
    },

    getProblemsSolved(state: MathState): number {
      // This is an approximation - in a real app, you would have a more accurate way to count
      return state.topicCompletion.reduce((sum, topic) => {
        // Assuming each problem is worth 1 point on average
        return sum + Math.ceil(topic.pointsEarned);
      }, 0);
    },

    getRecentTopics(state: MathState): any[] {
      // Get topics with any progress, sorted by completion percentage
      const sortedTopics = [...state.topicCompletion]
        .filter((topic) => topic.pointsEarned > 0)
        .sort((a, b) => {
          // First sort by progress percentage (descending)
          if (b.percentageCompleted !== a.percentageCompleted) {
            return b.percentageCompleted - a.percentageCompleted;
          }
          // Then by points earned (descending)
          return b.pointsEarned - a.pointsEarned;
        })
        .slice(0, 3);

      // Map to include topic descriptions
      return sortedTopics.map((completion) => {
        const topic = state.topics.find((t) => t.id === completion.topicId) || {
          id: completion.topicId,
          name: completion.topicName,
          description: "No description available",
        };

        return {
          id: topic.id,
          name: topic.name,
          description:
            topic.description || `Practice problems in ${completion.topicName}`,
          progress: completion.percentageCompleted,
          pointsEarned: completion.pointsEarned,
          totalPointsPossible: completion.totalPointsPossible,
        };
      });
    },
  },
};

export default mathModule;
