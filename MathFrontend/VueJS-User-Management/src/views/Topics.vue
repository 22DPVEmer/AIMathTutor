<template>
  <div class="topics-container">
    <div class="row">
      <!-- Sidebar -->
      <Sidebar />

      <!-- Main Content -->
      <main class="col-md-9 ms-sm-auto col-lg-9 px-md-4">
        <div
          class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom"
        >
          <h1 class="h2">Math Topics</h1>
          <div class="btn-toolbar mb-2 mb-md-0">
            <div class="btn-group me-2">
              <button
                type="button"
                class="btn btn-sm btn-outline-primary"
                @click="showAllTopics"
              >
                All Topics
              </button>
              <button
                type="button"
                class="btn btn-sm btn-outline-primary"
                @click="showInProgress"
              >
                In Progress
              </button>
              <button
                type="button"
                class="btn btn-sm btn-outline-primary"
                @click="showCompleted"
              >
                Completed
              </button>
            </div>
          </div>
        </div>

        <!-- Loading Indicator -->
        <div v-if="loading" class="text-center py-4">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
          <p class="mt-2">Loading content...</p>
        </div>

        <!-- School Classes -->
        <div v-else class="row mb-4">
          <div class="col-12">
            <div class="card">
              <div class="card-body">
                <h3 class="card-title">School Classes</h3>
                <div class="row">
                  <div
                    class="col-md-3 mb-3"
                    v-for="(schoolClass, index) in schoolClasses"
                    :key="index"
                  >
                    <div class="card h-100">
                      <div class="card-body">
                        <h5 class="card-title">{{ schoolClass.name }}</h5>
                        <p class="card-text">{{ schoolClass.description }}</p>
                        <div
                          class="d-flex justify-content-between align-items-center"
                        >
                          <button
                            class="btn btn-primary"
                            @click="viewSchoolClass(schoolClass.id)"
                          >
                            Explore
                          </button>
                          <span class="badge bg-info"
                            >{{ schoolClass.topicCount }} topics</span
                          >
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Selected School Class Topics -->
        <div v-if="selectedSchoolClass && !loading" class="row mb-4">
          <div class="col-12">
            <div class="card">
              <div
                class="card-header d-flex justify-content-between align-items-center"
              >
                <h3 class="card-title mb-0">
                  Topics for {{ selectedSchoolClass.name }}
                </h3>
                <button
                  class="btn btn-sm btn-outline-secondary"
                  @click="clearSelectedClass"
                >
                  Back to All Classes
                </button>
              </div>
              <div class="card-body">
                <!-- Parent Topics -->
                <div
                  v-for="(parentTopic, parentIndex) in parentTopics"
                  :key="parentIndex"
                  class="mb-4"
                >
                  <div
                    class="parent-topic p-3 bg-light rounded mb-2 border-start border-primary border-4"
                  >
                    <div
                      class="d-flex justify-content-between align-items-center"
                    >
                      <h4 class="mb-0">{{ parentTopic.name }}</h4>
                      <span
                        :class="
                          'badge bg-' +
                          getDifficultyClass(parentTopic.difficulty)
                        "
                      >
                        {{ parentTopic.difficulty }}
                      </span>
                    </div>
                    <p class="text-muted mt-2 mb-0">
                      {{ parentTopic.description }}
                    </p>
                  </div>

                  <!-- Subtopics -->
                  <div class="row">
                    <div
                      v-for="(subtopic, subtopicIndex) in getSubtopics(
                        parentTopic.id
                      )"
                      :key="subtopicIndex"
                      class="col-md-4 mb-3"
                    >
                      <div class="card h-100 subtopic-card">
                        <div class="card-body">
                          <div class="d-flex justify-content-between">
                            <h5 class="card-title">{{ subtopic.name }}</h5>
                            <span
                              :class="
                                'badge bg-' +
                                getDifficultyClass(subtopic.difficulty)
                              "
                            >
                              {{ subtopic.difficulty }}
                            </span>
                          </div>
                          <p class="card-text">{{ subtopic.description }}</p>

                          <div class="mt-3">
                            <div class="progress mb-2" style="height: 10px">
                              <div
                                class="progress-bar"
                                role="progressbar"
                                :style="{ width: subtopic.progress + '%' }"
                                :aria-valuenow="subtopic.progress || 0"
                                aria-valuemin="0"
                                aria-valuemax="100"
                              ></div>
                            </div>
                            <div class="d-flex justify-content-between">
                              <small
                                >Progress: {{ subtopic.progress || 0 }}%</small
                              >
                              <button
                                class="btn btn-sm btn-primary"
                                @click="startTopic(subtopic.id)"
                              >
                                Start
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
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
import { mapActions } from "vuex";
import Sidebar from "@/components/Home/Sidebar.vue";
import axios from "axios";

export default {
  name: "Topics",
  components: {
    Sidebar,
  },
  data() {
    return {
      loading: false,
      schoolClasses: [],
      topics: [],
      selectedSchoolClass: null,
      parentTopics: [],
    };
  },
  methods: {
    ...mapActions("math", ["fetchTopics"]),

    async loadSchoolClasses() {
      this.loading = true;
      try {
        const response = await axios.get(
          "http://localhost:5000/api/schoolclass"
        );
        this.schoolClasses = response.data;
      } catch (error) {
        console.error("Error loading school classes:", error);
      } finally {
        this.loading = false;
      }
    },

    async viewSchoolClass(schoolClassId) {
      this.loading = true;
      try {
        // Get the school class details
        const classResponse = await axios.get(
          `http://localhost:5000/api/schoolclass/${schoolClassId}`
        );
        this.selectedSchoolClass = classResponse.data;

        // Get all topics for this school class
        const topicsResponse = await axios.get(
          `http://localhost:5000/api/mathtopic/schoolclass/${schoolClassId}`
        );
        this.topics = topicsResponse.data;

        // Filter parent topics (those without a parentTopicId)
        this.parentTopics = this.topics.filter((topic) => !topic.parentTopicId);
      } catch (error) {
        console.error(
          `Error loading topics for school class ${schoolClassId}:`,
          error
        );
      } finally {
        this.loading = false;
      }
    },

    clearSelectedClass() {
      this.selectedSchoolClass = null;
      this.topics = [];
      this.parentTopics = [];
    },

    // Get subtopics for a specific parent topic
    getSubtopics(parentId) {
      return this.topics.filter((topic) => topic.parentTopicId === parentId);
    },

    getDifficultyClass(difficulty) {
      if (typeof difficulty === "string") {
        switch (difficulty.toLowerCase()) {
          case "easy":
            return "success";
          case "medium":
            return "warning";
          case "hard":
            return "danger";
          default:
            return "secondary";
        }
      } else {
        return "secondary";
      }
    },

    getStatusClass(status) {
      switch (status.toLowerCase()) {
        case "completed":
          return "success";
        case "in progress":
          return "primary";
        case "not started":
          return "secondary";
        default:
          return "secondary";
      }
    },

    showAllTopics() {
      // Implement filter logic
    },

    showInProgress() {
      // Implement filter logic
    },

    showCompleted() {
      // Implement filter logic
    },

    startTopic(topicId) {
      this.$router.push({ path: "/practice", query: { topic: topicId } });
    },
  },
  async created() {
    await this.loadSchoolClasses();
  },
};
</script>

<style scoped>
.topics-container {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-5px);
}

.progress {
  background-color: #e9ecef;
}

.table th {
  background-color: #f8f9fa;
}

.subtopic-card {
  transition: all 0.2s ease;
  border: 1px solid #dee2e6;
}

.subtopic-card:hover {
  border-color: #6c757d;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.parent-topic {
  background-color: #f8f9fa;
}
</style>
