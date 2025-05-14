<template>
  <div class="col-12 col-md-6 mb-4">
    <div class="card shadow-sm border-0">
      <div class="card-header bg-primary text-white">
        <h5 class="card-title mb-0">Math Learning Progress</h5>
      </div>
      <div class="card-body">
        <div class="d-flex align-items-center mb-3">
          <div class="icon-wrapper bg-light rounded-circle p-3 me-3">
            <i class="bi bi-graph-up text-primary fs-4"></i>
          </div>
          <div>
            <h3 class="mb-0">{{ progress }}%</h3>
            <p class="text-muted mb-0">Overall Completion</p>
          </div>
        </div>
        <div class="progress mb-3" style="height: 10px">
          <div
            class="progress-bar bg-success"
            role="progressbar"
            :style="{ width: progress + '%' }"
            :aria-valuenow="progress"
            aria-valuemin="0"
            aria-valuemax="100"
          ></div>
        </div>
        <!-- Topic Progress List -->
        <div v-if="topicProgress && topicProgress.length > 0" class="mt-3">
          <h6 class="mb-2">Topic Progress</h6>
          <div class="topic-progress-list">
            <div
              v-for="(topic, index) in topicProgress"
              :key="index"
              class="topic-item mb-2"
            >
              <div class="d-flex justify-content-between mb-1">
                <span
                  class="small fw-medium topic-name-link"
                  @click="navigateToTopic(topic)"
                  >{{ topic.topicName }}</span
                >
                <span class="small text-muted"
                  >{{ topic.pointsEarned }}/{{
                    topic.totalPointsPossible
                  }}
                  points</span
                >
              </div>
              <div class="progress" style="height: 8px">
                <div
                  class="progress-bar"
                  :class="{ 'bg-success': topic.percentageCompleted >= 100 }"
                  role="progressbar"
                  :style="{ width: topic.percentageCompleted + '%' }"
                  :aria-valuenow="topic.percentageCompleted"
                  aria-valuemin="0"
                  aria-valuemax="100"
                ></div>
              </div>
            </div>
          </div>
        </div>
        <div v-else class="mt-3 text-center py-2">
          <p class="text-muted mb-0">No topics started yet</p>
        </div>

        <div class="mt-3">
          <div class="row g-3">
            <div class="col-6">
              <div class="p-3 border rounded bg-light text-center">
                <h5 class="mb-1">{{ completedProblems }}</h5>
                <p class="small text-muted mb-0">Problems Solved</p>
              </div>
            </div>
            <div class="col-6">
              <div class="p-3 border rounded bg-light text-center">
                <h5 class="mb-1">{{ skillLevel }}</h5>
                <p class="small text-muted mb-0">Current Skill</p>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="card-footer bg-light">
        <div class="d-flex justify-content-between align-items-center">
          <span>Last activity: {{ lastAchievement }}</span>
          <button class="btn btn-sm btn-primary" @click="viewTopics">
            <i class="bi bi-grid-3x3-gap-fill me-1"></i> View All Topics
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "MathProgressWidget",
  props: {
    progress: {
      type: Number,
      required: true,
    },
    completedProblems: {
      type: Number,
      default: 0,
    },
    skillLevel: {
      type: String,
      default: "Beginner",
    },
    lastAchievement: {
      type: String,
      default: "None yet",
    },
    topicProgress: {
      type: Array,
      default: () => [],
    },
  },
  data() {
    return {};
  },
  methods: {
    viewTopics() {
      console.log("View topics clicked");
      this.$router.push("/topics");
    },
    navigateToTopic(topic: any) {
      const topicId = topic.topicId;
      this.$router.push(`/topics/${topicId}/problems`);
      console.log(`Navigating to topic problems for topic ID: ${topicId}`);
    },
  },
  computed: {},
});
</script>

<style scoped>
.icon-wrapper {
  width: 60px;
  height: 60px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.topic-name-link {
  cursor: pointer;
  color: #0d6efd;
  transition: color 0.2s ease;
}

.topic-name-link:hover {
  color: #0a58ca;
  text-decoration: underline;
}
</style>
