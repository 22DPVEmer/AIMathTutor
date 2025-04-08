<template>
  <div class="dashboard-container">
    <div class="row">
      <!-- Sidebar -->
      <Sidebar />

      <!-- Main Content -->
      <main class="col-md-9 ms-sm-auto col-lg-9 px-md-4">
        <div
          class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom"
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
          <ProgressWidget :progress="overallProgress" />
          <div class="col-12 col-md-4 mb-4">
            <div class="card bg-light p-3 rounded shadow-sm">
              <p class="text-muted">Topics Mastered</p>
              <h4>{{ masteredTopics }}/{{ totalTopics }}</h4>
            </div>
          </div>
          <div class="col-12 col-md-4 mb-4">
            <div class="card bg-light p-3 rounded shadow-sm">
              <p class="text-muted">Problems Solved</p>
              <h4>{{ problemsSolved }}</h4>
            </div>
          </div>
        </div>

        <!-- Quick Access -->
        <div class="row mb-4">
          <div class="col-12">
            <h3 class="mb-3">Quick Access</h3>
            <div class="row">
              <div
                class="col-md-4 mb-3"
                v-for="(topic, index) in recentTopics"
                :key="index"
              >
                <div class="card h-100">
                  <div class="card-body">
                    <h5 class="card-title">{{ topic.name }}</h5>
                    <p class="card-text">{{ topic.description }}</p>
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

        <!-- AI Recommendations -->
        <div class="row">
          <div class="col-12">
            <h3 class="mb-3">AI Recommendations</h3>
            <div class="card">
              <div class="card-body">
                <div v-if="loading" class="text-center">
                  <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                  </div>
                </div>
                <div v-else-if="recommendations.length > 0">
                  <div
                    v-for="(rec, index) in recommendations"
                    :key="index"
                    class="mb-3"
                  >
                    <h5>{{ rec.title }}</h5>
                    <p>{{ rec.description }}</p>
                    <button
                      class="btn btn-outline-primary"
                      @click="followRecommendation(rec)"
                    >
                      Get Started
                    </button>
                  </div>
                </div>
                <div v-else>
                  <p>No recommendations available at the moment.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
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
      overallProgress: 0,
      masteredTopics: 0,
      totalTopics: 10,
      problemsSolved: 0,
      recentTopics: [
        {
          id: 1,
          name: "Algebra Basics",
          description: "Master the fundamentals of algebraic expressions",
        },
        {
          id: 2,
          name: "Geometry",
          description: "Explore shapes, angles, and spatial relationships",
        },
        {
          id: 3,
          name: "Calculus",
          description: "Dive into limits, derivatives, and integrals",
        },
      ],
    };
  },
  computed: {
    ...mapState("user", ["userData"]),
    ...mapState("math", ["loading", "recommendations"]),
  },
  methods: {
    ...mapActions("math", ["fetchRecommendations"]),
    startPractice() {
      this.$router.push("/practice");
    },
    startTopic(topicId) {
      this.$router.push({ path: "/practice", query: { topic: topicId } });
    },
    followRecommendation(rec) {
      // Implement recommendation action
      console.log("Following recommendation:", rec);
    },
  },
  async created() {
    await this.fetchRecommendations();
    // In a real app, these would come from the backend
    this.overallProgress = 65;
    this.masteredTopics = 4;
    this.problemsSolved = 127;
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
