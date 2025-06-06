<template>
  <div class="dashboard-container">
    <!-- Sidebar -->
    <Sidebar />

    <!-- Main Content -->
    <main class="w-full pt-14 sm:pt-20 md:pt-3 px-3 px-md-4">
        <!-- Mobile Header -->
        <div class="d-block d-md-none pt-3 pb-2 mb-3 border-bottom">
          <h1 class="h3 text-center">Welcome, {{ userData.firstName }}!</h1>
          <div class="d-grid mt-3">
            <button
              type="button"
              class="btn btn-primary btn-lg"
              @click="startPractice"
            >
              Start Practice
            </button>
          </div>
        </div>

        <!-- Desktop Header -->
        <div
          class="d-none d-md-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom"
        >
          <h1 class="h2">Welcome, {{ userData.firstName }}!</h1>
          <div class="btn-toolbar mb-2 mb-md-0">
            <div class="btn-group me-2">
              <button
                type="button"
                class="btn btn-sm btn-outline-primary"
                @click="startPractice"
              >
                Start Practice
              </button>
            </div>
          </div>
        </div>

        <!-- Progress Overview -->
        <div class="row mb-4">
          <ProgressWidget
            :progress="overallProgress"
            :completed-problems="problemsSolved"
            :skill-level="getSkillLevel"
            :last-achievement="getLastAchievement"
            :topic-progress="topicCompletion"
          />
          <div class="col-12 col-md-3 mb-4">
            <div class="card shadow-sm">
              <div class="card-header bg-light">
                <h6 class="mb-0">Topics Mastered</h6>
              </div>
              <div class="card-body text-center">
                <div class="d-flex align-items-center justify-content-center">
                  <i
                    class="bi bi-check-circle-fill text-success me-2"
                    style="font-size: 1.5rem"
                  ></i>
                  <h4 class="mb-0">{{ masteredTopics }}/{{ totalTopics }}</h4>
                </div>
              </div>
            </div>
          </div>
          <div class="col-12 col-md-3 mb-4">
            <div class="card shadow-sm">
              <div class="card-header bg-light">
                <h6 class="mb-0">Problems Solved</h6>
              </div>
              <div class="card-body text-center">
                <div class="d-flex align-items-center justify-content-center">
                  <i
                    class="bi bi-lightning-fill text-warning me-2"
                    style="font-size: 1.5rem"
                  ></i>
                  <h4 class="mb-0">{{ problemsSolved }}</h4>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Access -->
        <div class="row mb-4">
          <div class="col-12">
            <h3 class="mb-3">Quick Access</h3>
            <div v-if="isLoading || loading" class="text-center py-4">
              <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
            </div>
            <div v-else-if="recentTopics.length === 0" class="alert alert-info">
              <p class="mb-0">
                You haven't started any topics yet. Explore the Topics section
                to get started!
              </p>
            </div>
            <div v-else class="row">
              <div
                class="col-md-4 mb-3"
                v-for="(topic, index) in recentTopics"
                :key="index"
              >
                <div class="card h-100">
                  <div class="card-header bg-light">
                    <h5 class="card-title mb-0">{{ topic.name }}</h5>
                  </div>
                  <div class="card-body">
                    <p class="card-text">{{ topic.description }}</p>
                    <div class="progress mb-2" style="height: 8px">
                      <div
                        class="progress-bar"
                        role="progressbar"
                        :style="{ width: topic.progress + '%' }"
                        :class="{ 'bg-success': topic.progress >= 100 }"
                        :aria-valuenow="topic.progress"
                        aria-valuemin="0"
                        aria-valuemax="100"
                      ></div>
                    </div>
                    <div
                      class="d-flex justify-content-between align-items-center mb-3"
                    >
                      <small class="text-muted"
                        >{{ topic.progress }}% complete</small
                      >
                      <small class="badge bg-light text-dark">
                        {{ topic.pointsEarned }}/{{ topic.totalPointsPossible }}
                        points
                      </small>
                    </div>
                    <div class="d-grid">
                      <button
                        class="btn btn-primary"
                        @click="startTopic(topic.id)"
                      >
                        Continue Learning
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Recommendations -->
        <div class="row">
          <div class="col-12">
            <h3 class="mb-3">AI Recommendations</h3>
            <div class="card">
              <div class="card-body">
                <div v-if="isLoading || loading" class="text-center py-4">
                  <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                  </div>
                </div>
                <div v-else-if="recommendations.length > 0">
                  <div
                    v-for="(rec, index) in recommendations"
                    :key="index"
                    class="mb-4 p-3 border-bottom"
                  >
                    <h5 class="text-primary">{{ rec.title }}</h5>
                    <p>{{ rec.description }}</p>
                    <button
                      class="btn btn-outline-primary"
                      @click="followRecommendation(rec)"
                    >
                      Get Started
                    </button>
                  </div>
                </div>
                <div v-else class="text-center py-4">
                  <div class="mb-3">
                    <i
                      class="bi bi-lightbulb text-warning"
                      style="font-size: 2rem"
                    ></i>
                  </div>
                  <h5>No recommendations yet</h5>
                  <p class="text-muted">
                    Complete more problems to get personalized recommendations.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
  </div>
</template>

<script>
import { mapState, mapGetters, mapActions } from "vuex";
import Sidebar from "@/components/Home/Sidebar.vue";
import ProgressWidget from "@/components/Home/MathProgressWidget.vue";

export default {
  name: "Dashboard",
  components: {
    Sidebar,
    ProgressWidget,
  },
  data() {
    return {
      isLoading: false,
    };
  },
  computed: {
    ...mapState("user", ["userData"]),
    ...mapState("math", [
      "loading",
      "recommendations",
      "topics",
      "topicCompletion",
    ]),
    ...mapGetters("math", [
      "getOverallProgress",
      "getMasteredTopics",
      "getTotalTopics",
      "getProblemsSolved",
      "getRecentTopics",
    ]),

    overallProgress() {
      return this.getOverallProgress;
    },

    masteredTopics() {
      return this.getMasteredTopics;
    },

    totalTopics() {
      return this.getTotalTopics;
    },

    problemsSolved() {
      return this.getProblemsSolved;
    },

    recentTopics() {
      return this.getRecentTopics;
    },

    getSkillLevel() {
      // Determine skill level based on overall progress
      if (this.overallProgress >= 80) return "Advanced";
      if (this.overallProgress >= 40) return "Intermediate";
      return "Beginner";
    },

    getLastAchievement() {
      // Find the most recently completed topic (100% progress)
      const completedTopics = this.topicCompletion
        .filter((topic) => topic.percentageCompleted >= 100)
        .sort((a, b) => b.pointsEarned - a.pointsEarned);

      if (completedTopics.length > 0) {
        return `Mastered ${completedTopics[0].topicName}`;
      }

      // If no topics are 100% complete, find the one with highest progress
      const inProgressTopics = this.topicCompletion
        .filter((topic) => topic.percentageCompleted > 0)
        .sort((a, b) => b.percentageCompleted - a.percentageCompleted);

      if (inProgressTopics.length > 0) {
        return `Working on ${inProgressTopics[0].topicName}`;
      }

      return "Just getting started";
    },
  },
  methods: {
    ...mapActions("math", [
      "fetchRecommendations",
      "fetchTopics",
      "fetchTopicCompletion",
      "fetchUserProblems",
    ]),

    startPractice() {
      this.$router.push("/practice");
    },

    startTopic(topicId) {
      this.$router.push({ path: "/practice", query: { topic: topicId } });
    },

    followRecommendation(rec) {
      if (rec.link) {
        this.$router.push(rec.link);
      } else {
        console.log("Following recommendation:", rec);
      }
    },

    async loadData() {
      this.isLoading = true;
      try {
        // Load all required data in parallel
        await Promise.all([
          this.fetchTopics(),
          this.fetchTopicCompletion(),
          this.fetchUserProblems(),
          this.fetchRecommendations(),
        ]);
      } catch (error) {
        console.error("Error loading dashboard data:", error);
      } finally {
        this.isLoading = false;
      }
    },
  },
  async created() {
    await this.loadData();
  },
};
</script>

<style scoped>
.dashboard-container {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-5px);
}

.btn-toolbar {
  gap: 0.5rem;
}
</style>
