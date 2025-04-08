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
        <div class="d-flex justify-content-between small">
          <span>Level {{ currentLevel }}</span>
          <span>{{ pointsToNextLevel }} points to next level</span>
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
          <span>Last achievement: {{ lastAchievement }}</span>
          <button class="btn btn-sm btn-primary" @click="viewLearningPath">
            <i class="bi bi-mortarboard-fill me-1"></i> View Learning Path
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
  },
  data() {
    return {
      completedProblems: 147,
      skillLevel: "Intermediate",
      lastAchievement: "Mastered Quadratic Equations",
    };
  },
  methods: {
    viewLearningPath() {
      // Navigate to learning path view
      console.log("View learning path clicked");
      // this.$router.push('/learning-path');
    },
  },
  computed: {
    currentLevel(): number {
      return Math.floor(this.progress / 20) + 1;
    },
    pointsToNextLevel(): number {
      // Calculate points needed to reach next level (100 points per level)
      return 100 - (this.progress % 20) * 5;
    },
  },
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
</style>
