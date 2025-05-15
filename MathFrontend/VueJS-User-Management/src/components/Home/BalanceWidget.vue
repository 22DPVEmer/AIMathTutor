<template>
  <div class="col-12 col-md-4 mb-4">
    <div class="progress-widget bg-light p-3 rounded shadow-sm">
      <p class="text-muted">Learning Progress</p>
      <h4>{{ progress }}%</h4>
      <div class="progress mt-2">
        <div
          class="progress-bar"
          role="progressbar"
          :style="{ width: progress + '%' }"
          :aria-valuenow="progress"
          aria-valuemin="0"
          aria-valuemax="100"
        ></div>
      </div>
    </div>
    <div class="mt-3 d-grid gap-2 d-md-block">
      <button type="button" class="btn btn-primary" @click="viewProgress">
        View Details
      </button>
    </div>
  </div>
  <div class="col-md-6 mb-4">
    <div class="card shadow-sm border-0">
      <div class="card-header bg-primary text-white">
        <h5 class="card-title mb-0">Your Balance</h5>
      </div>
      <div class="card-body">
        <div class="d-flex align-items-center mb-3">
          <div class="icon-wrapper bg-light rounded-circle p-3 me-3">
            <i class="bi bi-graph-up text-primary fs-4"></i>
          </div>
          <div>
            <h3 class="mb-0">{{ formattedBalance }}</h3>
            <p class="text-muted mb-0">Current Points</p>
          </div>
        </div>
        <div class="progress mb-3" style="height: 10px">
          <div
            class="progress-bar bg-success"
            role="progressbar"
            :style="{ width: progressWidth + '%' }"
            :aria-valuenow="Math.min(balance / 100, 100)"
            aria-valuemin="0"
            aria-valuemax="100"
          ></div>
        </div>
        <div class="d-flex justify-content-between small">
          <span>Level {{ currentLevel }}</span>
          <span>{{ pointsToNextLevel }} points to next level</span>
        </div>
      </div>
      <div class="card-footer bg-light">
        <div class="d-flex justify-content-between align-items-center">
          <span>Last update: {{ lastUpdated }}</span>
          <button class="btn btn-sm btn-outline-primary">
            <i class="bi bi-arrow-repeat me-1"></i> Refresh
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "BalanceWidget",
  props: {
    balance: {
      type: Number,
      required: true,
    },
    progress: {
      type: Number,
      default: 0,
    },
  },
  methods: {
    viewProgress() {
      console.log("View progress clicked");
    },
  },
  computed: {
    formattedBalance(): string {
      return new Intl.NumberFormat().format(this.balance);
    },
    currentLevel(): number {
      return Math.floor(this.balance / 10000) + 1;
    },
    pointsToNextLevel(): number {
      return 10000 - (this.balance % 10000);
    },
    progressWidth(): number {
      return (this.balance % 10000) / 100;
    },
    lastUpdated(): string {
      return new Date().toLocaleString();
    },
  },
});
</script>

<style scoped>
.progress-widget {
  transition: transform 0.2s;
}

.progress-widget:hover {
  transform: translateY(-5px);
}

.icon-wrapper {
  width: 60px;
  height: 60px;
  display: flex;
  justify-content: center;
  align-items: center;
}
</style>
