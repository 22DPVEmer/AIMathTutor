<template>
  <div class="topics-container">
    <div class="row">
      <!-- Sidebar -->
      <Sidebar />

      <!-- Main Content -->
      <main class="col-md-9 ms-sm-auto col-lg-9 px-md-4">
        <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
          <h1 class="h2">Math Topics</h1>
          <div class="btn-toolbar mb-2 mb-md-0">
            <div class="btn-group me-2">
              <button type="button" class="btn btn-sm btn-outline-primary" @click="showAllTopics">
                All Topics
              </button>
              <button type="button" class="btn btn-sm btn-outline-primary" @click="showInProgress">
                In Progress
              </button>
              <button type="button" class="btn btn-sm btn-outline-primary" @click="showCompleted">
                Completed
              </button>
            </div>
          </div>
        </div>

        <!-- Topic Categories -->
        <div class="row mb-4">
          <div class="col-12">
            <div class="card">
              <div class="card-body">
                <h3 class="card-title">Categories</h3>
                <div class="row">
                  <div class="col-md-3 mb-3" v-for="(category, index) in categories" :key="index">
                    <div class="card h-100">
                      <div class="card-body">
                        <h5 class="card-title">{{ category.name }}</h5>
                        <p class="card-text">{{ category.description }}</p>
                        <button class="btn btn-primary" @click="viewCategory(category.id)">
                          Explore
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Topics List -->
        <div class="row">
          <div class="col-12">
            <div class="card">
              <div class="card-body">
                <h3 class="card-title">All Topics</h3>
                <div class="table-responsive">
                  <table class="table table-hover">
                    <thead>
                      <tr>
                        <th>Topic</th>
                        <th>Difficulty</th>
                        <th>Progress</th>
                        <th>Status</th>
                        <th>Action</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(topic, index) in topics" :key="index">
                        <td>{{ topic.name }}</td>
                        <td>
                          <span :class="'badge bg-' + getDifficultyClass(topic.difficulty)">
                            {{ topic.difficulty }}
                          </span>
                        </td>
                        <td>
                          <div class="progress" style="height: 20px;">
                            <div class="progress-bar" role="progressbar" 
                                 :style="{ width: topic.progress + '%' }"
                                 :aria-valuenow="topic.progress" 
                                 aria-valuemin="0" 
                                 aria-valuemax="100">
                              {{ topic.progress }}%
                            </div>
                          </div>
                        </td>
                        <td>
                          <span :class="'badge bg-' + getStatusClass(topic.status)">
                            {{ topic.status }}
                          </span>
                        </td>
                        <td>
                          <button class="btn btn-sm btn-primary" @click="startTopic(topic.id)">
                            Start
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
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
import { mapActions } from 'vuex';
import Sidebar from '@/components/Home/Sidebar.vue';

export default {
  name: 'Topics',
  components: {
    Sidebar
  },
  data() {
    return {
      categories: [
        {
          id: 1,
          name: 'Algebra',
          description: 'Master algebraic expressions and equations'
        },
        {
          id: 2,
          name: 'Geometry',
          description: 'Explore shapes and spatial relationships'
        },
        {
          id: 3,
          name: 'Calculus',
          description: 'Learn about limits, derivatives, and integrals'
        },
        {
          id: 4,
          name: 'Statistics',
          description: 'Understand data analysis and probability'
        }
      ],
      topics: [
        {
          id: 1,
          name: 'Linear Equations',
          difficulty: 'Easy',
          progress: 75,
          status: 'In Progress'
        },
        {
          id: 2,
          name: 'Quadratic Equations',
          difficulty: 'Medium',
          progress: 30,
          status: 'In Progress'
        },
        {
          id: 3,
          name: 'Trigonometry',
          difficulty: 'Hard',
          progress: 0,
          status: 'Not Started'
        }
      ]
    };
  },
  methods: {
    ...mapActions('math', ['fetchTopics']),
    getDifficultyClass(difficulty) {
      switch (difficulty.toLowerCase()) {
        case 'easy':
          return 'success';
        case 'medium':
          return 'warning';
        case 'hard':
          return 'danger';
        default:
          return 'secondary';
      }
    },
    getStatusClass(status) {
      switch (status.toLowerCase()) {
        case 'completed':
          return 'success';
        case 'in progress':
          return 'primary';
        case 'not started':
          return 'secondary';
        default:
          return 'secondary';
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
    viewCategory(categoryId) {
      // Implement category view
      console.log('Viewing category:', categoryId);
    },
    startTopic(topicId) {
      this.$router.push({ path: '/practice', query: { topic: topicId } });
    }
  },
  async created() {
    await this.fetchTopics();
  }
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
</style> 