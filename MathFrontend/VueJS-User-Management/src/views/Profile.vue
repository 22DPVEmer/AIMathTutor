<template>
  <div class="min-h-screen bg-gray-50">
    <section class="p-4 sm:p-6 lg:p-8">
      <div class="max-w-7xl mx-auto">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200">
          <div class="flex flex-col md:flex-row">
            <Sidebar />
            <div class="flex-1">
              <div class="p-4 sm:p-6 lg:p-8">
                <div v-if="loading" class="flex justify-center">
                  <div
                    class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"
                  ></div>
                </div>
                <div v-else>
                  <!-- Profile Header -->
                  <div class="mb-6 flex justify-between items-center">
                    <div>
                      <h3 class="text-2xl font-bold text-gray-900">
                        Welcome, {{ user.firstName }}
                      </h3>
                      <p class="text-gray-600">{{ user.email }}</p>
                    </div>
                    <div class="flex space-x-3">
                      <button
                        @click="showEditMode"
                        class="inline-flex items-center px-4 py-2 border border-primary-600 text-sm font-medium rounded-md text-primary-600 bg-white hover:bg-primary-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                      >
                        <svg
                          class="w-4 h-4 mr-2"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                          />
                        </svg>
                        Edit Profile
                      </button>
                      <button
                        @click="confirmDelete"
                        class="inline-flex items-center px-4 py-2 border border-red-600 text-sm font-medium rounded-md text-red-600 bg-white hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                      >
                        <svg
                          class="w-4 h-4 mr-2"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                          />
                        </svg>
                        Delete Account
                      </button>
                    </div>
                  </div>

                  <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <!-- Math Progress Widget -->
                    <div>
                      <div
                        class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden"
                      >
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">
                            Math Learning Progress
                          </h5>
                        </div>
                        <div class="p-4">
                          <div class="flex items-center mb-4">
                            <div
                              class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center text-xl font-medium text-primary-600 mr-4"
                            >
                              <svg
                                class="w-8 h-8"
                                fill="none"
                                stroke="currentColor"
                                viewBox="0 0 24 24"
                              >
                                <path
                                  stroke-linecap="round"
                                  stroke-linejoin="round"
                                  stroke-width="2"
                                  d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                                />
                              </svg>
                            </div>
                            <div>
                              <h3 class="text-2xl font-bold text-gray-900">
                                {{ progress }}%
                              </h3>
                              <p class="text-gray-600">Overall Completion</p>
                            </div>
                          </div>

                          <!-- Progress Bar -->
                          <div class="h-2 bg-gray-200 rounded-full mb-6">
                            <div
                              class="h-2 rounded-full bg-green-500"
                              :style="{ width: progress + '%' }"
                            ></div>
                          </div>

                          <!-- Stats -->
                          <div class="grid grid-cols-2 gap-4 mb-4">
                            <div class="bg-gray-50 p-3 rounded-lg text-center">
                              <h5 class="text-xl font-semibold text-gray-900">
                                {{ completedProblems }}
                              </h5>
                              <p class="text-sm text-gray-600">
                                Problems Solved
                              </p>
                            </div>
                            <div class="bg-gray-50 p-3 rounded-lg text-center">
                              <h5 class="text-xl font-semibold text-gray-900">
                                {{ skillLevel }}
                              </h5>
                              <p class="text-sm text-gray-600">Current Skill</p>
                            </div>
                          </div>
                        </div>

                        <div
                          class="bg-gray-50 px-4 py-3 border-t border-gray-200"
                        >
                          <div class="flex justify-between items-center">
                            <span class="text-sm text-gray-600"
                              >Last achievement: {{ lastAchievement }}</span
                            >
                            <button
                              class="inline-flex items-center px-3 py-1 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                              @click="$router.push('/topics')"
                            >
                              View All Topics
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Topic Progress Section -->
                    <div>
                      <div
                        class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden"
                      >
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">
                            Topic Progress
                          </h5>
                        </div>
                        <div class="p-4">
                          <div class="grid grid-cols-2 gap-4">
                            <!-- Filter out topics with 0 total points and map remaining ones -->
                            <div
                              v-for="topic in activeTopics"
                              :key="topic.topicId"
                              @click="navigateToTopic(topic)"
                              class="relative bg-white rounded-lg shadow-sm p-4 flex flex-col items-center justify-center hover:shadow-md transition-shadow border border-gray-100 topic-card cursor-pointer"
                            >
                              <!-- Circular Progress -->
                              <div class="relative w-20 h-20">
                                <svg class="w-full h-full" viewBox="0 0 36 36">
                                  <path
                                    d="M18 2.0845
                                      a 15.9155 15.9155 0 0 1 0 31.831
                                      a 15.9155 15.9155 0 0 1 0 -31.831"
                                    fill="none"
                                    stroke="#E5E7EB"
                                    stroke-width="3"
                                  />
                                  <path
                                    d="M18 2.0845
                                      a 15.9155 15.9155 0 0 1 0 31.831
                                      a 15.9155 15.9155 0 0 1 0 -31.831"
                                    fill="none"
                                    :stroke="
                                      getProgressColor(
                                        topic.percentageCompleted
                                      )
                                    "
                                    stroke-width="3"
                                    :stroke-dasharray="`${topic.percentageCompleted}, 100`"
                                  />
                                </svg>
                                <div
                                  class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-center"
                                >
                                  <span class="text-lg font-bold text-gray-700"
                                    >{{ topic.percentageCompleted }}%</span
                                  >
                                </div>
                              </div>

                              <!-- Topic Name and Points -->
                              <h4
                                class="mt-2 text-sm font-medium text-gray-900 text-center hover:text-primary-600 transition-colors"
                              >
                                {{ topic.topicName }}
                              </h4>
                              <p class="mt-1 text-xs text-gray-500">
                                {{ topic.pointsEarned }}/{{
                                  topic.totalPointsPossible
                                }}
                                points
                              </p>

                              <!-- Status Badge -->
                              <span
                                :class="
                                  getStatusClass(topic.percentageCompleted)
                                "
                                class="absolute top-2 right-2 px-2 py-1 text-xs font-medium rounded-full"
                              >
                                {{ getStatusText(topic.percentageCompleted) }}
                              </span>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Profile Card (View Mode) -->
                    <div v-if="!editMode">
                      <div
                        class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden"
                      >
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">
                            User Profile
                          </h5>
                        </div>
                        <div class="p-4">
                          <div class="flex items-center mb-4">
                            <div
                              class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center text-xl font-medium text-primary-600 mr-4"
                            >
                              {{ user.firstName?.charAt(0)
                              }}{{ user.lastName?.charAt(0) }}
                            </div>
                            <div>
                              <h5 class="text-lg font-medium text-gray-900">
                                {{ user.firstName }} {{ user.lastName }}
                              </h5>
                              <p class="text-gray-600 flex items-center">
                                <svg
                                  class="w-4 h-4 mr-2"
                                  fill="none"
                                  stroke="currentColor"
                                  viewBox="0 0 24 24"
                                >
                                  <path
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    stroke-width="2"
                                    d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                                  />
                                </svg>
                                {{ user.email }}
                              </p>
                            </div>
                          </div>
                          <div class="border-t border-gray-200 pt-4">
                            <dl class="divide-y divide-gray-200">
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">
                                  First Name:
                                </dt>
                                <dd class="text-sm text-gray-900 col-span-2">
                                  {{ user.firstName }}
                                </dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">
                                  Last Name:
                                </dt>
                                <dd class="text-sm text-gray-900 col-span-2">
                                  {{ user.lastName }}
                                </dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">
                                  Email:
                                </dt>
                                <dd class="text-sm text-gray-900 col-span-2">
                                  {{ user.email }}
                                </dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">
                                  Created:
                                </dt>
                                <dd class="text-sm text-gray-900 col-span-2">
                                  {{ formatDate(user.createdAt) }}
                                </dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">
                                  Last Login:
                                </dt>
                                <dd class="text-sm text-gray-900 col-span-2">
                                  {{ formatDate(user.lastLogin) }}
                                </dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">
                                  Verified:
                                </dt>
                                <dd class="text-sm col-span-2">
                                  <span
                                    class="px-2 py-1 text-xs font-medium rounded-full"
                                    :class="
                                      user.isVerified
                                        ? 'bg-green-100 text-green-800'
                                        : 'bg-blue-100 text-blue-800'
                                    "
                                  >
                                    {{ user.isVerified ? "Yes" : "No" }}
                                  </span>
                                </dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">
                                  Roles:
                                </dt>
                                <dd class="text-sm col-span-2">
                                  <div class="flex flex-wrap gap-2">
                                    <span
                                      v-for="role in user.roles"
                                      :key="role"
                                      class="px-2 py-1 text-xs font-medium rounded-full bg-primary-100 text-primary-800"
                                    >
                                      {{ role }}
                                    </span>
                                    <span
                                      v-if="!user.roles?.length"
                                      class="text-gray-500"
                                    >
                                      No roles assigned
                                    </span>
                                  </div>
                                </dd>
                              </div>
                            </dl>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Edit Profile Form (Edit Mode) -->
                    <div v-if="editMode">
                      <div
                        class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden"
                      >
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">
                            Edit Profile
                          </h5>
                        </div>
                        <div class="p-4">
                          <form
                            @submit.prevent="updateProfile"
                            class="space-y-4"
                          >
                            <div>
                              <label
                                for="firstName"
                                class="block text-sm font-medium text-gray-700"
                                >First Name</label
                              >
                              <input
                                type="text"
                                id="firstName"
                                v-model="formData.firstName"
                                required
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                            </div>
                            <div>
                              <label
                                for="lastName"
                                class="block text-sm font-medium text-gray-700"
                                >Last Name</label
                              >
                              <input
                                type="text"
                                id="lastName"
                                v-model="formData.lastName"
                                required
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                            </div>
                            <div>
                              <label
                                for="email"
                                class="block text-sm font-medium text-gray-700"
                                >Email</label
                              >
                              <input
                                type="email"
                                id="email"
                                v-model="formData.email"
                                required
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                            </div>
                            <div class="flex justify-end space-x-3">
                              <button
                                type="button"
                                @click="cancelEdit"
                                class="px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                              >
                                Cancel
                              </button>
                              <button
                                type="submit"
                                :disabled="updating"
                                class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                              >
                                <svg
                                  v-if="updating"
                                  class="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                                  fill="none"
                                  viewBox="0 0 24 24"
                                >
                                  <circle
                                    class="opacity-25"
                                    cx="12"
                                    cy="12"
                                    r="10"
                                    stroke="currentColor"
                                    stroke-width="4"
                                  ></circle>
                                  <path
                                    class="opacity-75"
                                    fill="currentColor"
                                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                                  ></path>
                                </svg>
                                Save Changes
                              </button>
                            </div>
                          </form>
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
    </section>

    <!-- Delete Confirmation Modal -->
    <div
      class="modal fade"
      id="deleteModal"
      tabindex="-1"
      aria-labelledby="deleteModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog">
        <div class="modal-content rounded-lg shadow-lg border border-gray-200">
          <div class="modal-header bg-red-600 text-white px-4 py-3">
            <h5 class="modal-title text-lg font-medium" id="deleteModalLabel">
              Confirm Account Deletion
            </h5>
            <button
              type="button"
              class="text-white opacity-80 hover:opacity-100 focus:outline-none"
              data-bs-dismiss="modal"
              aria-label="Close"
            >
              <svg
                class="w-5 h-5"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>
          <div class="modal-body p-4">
            <p class="text-gray-700 mb-4">
              Are you sure you want to delete your account? This action cannot
              be undone and all your data will be permanently removed.
            </p>
            <div
              v-if="deleteError"
              class="bg-red-50 text-red-700 p-3 rounded-md border border-red-200"
            >
              {{ deleteError }}
            </div>
          </div>
          <div
            class="modal-footer bg-gray-50 px-4 py-3 border-t border-gray-200 flex justify-end space-x-3"
          >
            <button
              type="button"
              class="px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
              data-bs-dismiss="modal"
            >
              Cancel
            </button>
            <button
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 disabled:opacity-50"
              @click="deleteAccount"
              :disabled="deleting"
            >
              <svg
                v-if="deleting"
                class="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  class="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  stroke-width="4"
                ></circle>
                <path
                  class="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                ></path>
              </svg>
              Delete Account
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Navbar from "@/components/Home/Navbar.vue";
import { mapState, mapGetters } from "vuex";
import { useToast } from "vue-toastification";

interface UserData {
  id?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  createdAt?: string;
  lastLogin?: string;
  isVerified?: boolean;
  roles?: string[];
  [key: string]: any;
}

interface TopicData {
  topicId: string;
  topicName: string;
  pointsEarned: number;
  totalPointsPossible: number;
  percentageCompleted: number;
}

export default defineComponent({
  name: "Profile",
  components: {
    Navbar,
  },
  setup() {
    const toast = useToast();
    return { toast };
  },
  data() {
    return {
      editMode: false,
      formData: {
        firstName: "",
        lastName: "",
        email: "",
        id: "",
      },
      updating: false,
      deleteModal: null as any,
      deleting: false,
      deleteError: "",
    };
  },
  computed: {
    ...mapGetters("user", ["userData"]),
    ...mapState("user", ["loading"]),
    ...mapState("math", ["topicCompletion"]),
    ...mapGetters("math", [
      "getOverallProgress",
      "getMasteredTopics",
      "getTotalTopics",
      "getProblemsSolved",
    ]),

    user(): UserData {
      return this.userData;
    },

    progress(): number {
      return this.getOverallProgress;
    },

    completedProblems(): number {
      return this.getProblemsSolved;
    },

    activeTopics(): TopicData[] {
      // Filter out topics with 0 total points
      return this.topicCompletion.filter(
        (topic: TopicData) => topic.totalPointsPossible > 0
      );
    },

    skillLevel(): string {
      // Determine skill level based on overall progress
      if (this.progress >= 80) return "Advanced";
      if (this.progress >= 40) return "Intermediate";
      return "Beginner";
    },

    lastAchievement(): string {
      // Find the most recently completed topic (100% progress)
      const completedTopics = this.topicCompletion
        .filter((topic: TopicData) => topic.percentageCompleted >= 100)
        .sort((a: TopicData, b: TopicData) => b.pointsEarned - a.pointsEarned);

      if (completedTopics.length > 0) {
        return `Mastered ${completedTopics[0].topicName}`;
      }

      // If no topics are 100% complete, find the one with highest progress
      const inProgressTopics = this.topicCompletion
        .filter((topic: TopicData) => topic.percentageCompleted > 0)
        .sort(
          (a: TopicData, b: TopicData) =>
            b.percentageCompleted - a.percentageCompleted
        );

      if (inProgressTopics.length > 0) {
        return `Working on ${inProgressTopics[0].topicName}`;
      }

      return "Just getting started";
    },
  },
  methods: {
    getProgressColor(percentage: number): string {
      if (percentage >= 80) return "#10B981"; // Green for high progress
      if (percentage >= 40) return "#10B981"; // Green for medium progress (changed from yellow)
      return "#10B981"; // Green for low progress
    },

    getStatusClass(percentage: number): string {
      if (percentage >= 100) return "bg-green-100 text-green-800";
      if (percentage >= 50) return "bg-blue-100 text-blue-800";
      if (percentage >= 25) return "bg-blue-100 text-blue-800";
      return "bg-green-100 text-green-800";
    },

    getStatusText(percentage: number): string {
      if (percentage >= 100) return "Mastered";
      if (percentage >= 50) return "In Progress";
      if (percentage >= 25) return "Started";
      return "New";
    },

    async loadProfile() {
      try {
        await this.$store.dispatch("user/getUserProfile");
      } catch (error) {
        console.error("Error loading profile:", error);
      }
    },

    async loadMathData() {
      try {
        // Load all required math data in parallel
        await Promise.all([
          this.$store.dispatch("math/fetchTopics"),
          this.$store.dispatch("math/fetchTopicCompletion"),
          this.$store.dispatch("math/fetchUserProblems"),
        ]);
      } catch (error) {
        console.error("Error loading math data:", error);
      }
    },
    formatDate(dateString: string | undefined): string {
      if (!dateString) return "N/A";
      return new Date(dateString).toLocaleString();
    },
    showEditMode() {
      this.formData = {
        firstName: this.user.firstName || "",
        lastName: this.user.lastName || "",
        email: this.user.email || "",
        id: this.user.id || "",
      };
      this.editMode = true;
    },
    cancelEdit() {
      this.editMode = false;
    },
    async updateProfile() {
      this.updating = true;
      try {
        const response = await this.$store.dispatch(
          "user/updateUserProfile",
          this.formData
        );

        if (response.success) {
          // Update the store with new user data
          this.$store.commit("user/SET_USER_DATA", response.data);

          this.toast.success("Profile updated successfully");
          this.editMode = false;
        } else {
          this.toast.error(response.message || "Failed to update profile");
        }
      } catch (error: any) {
        this.toast.error(
          error.message || "An error occurred while updating your profile"
        );
        console.error("Profile update error:", error);
      } finally {
        this.updating = false;
      }
    },
    confirmDelete() {
      this.deleteError = "";
      // Using any to avoid Modal type issues
      const Modal = (window as any).Modal;
      this.deleteModal = new Modal(
        document.getElementById("deleteModal") as HTMLElement
      );
      this.deleteModal?.show();
    },
    async deleteAccount() {
      this.deleting = true;
      this.deleteError = "";

      try {
        const response = await this.$store.dispatch(
          "user/deleteAccount",
          this.user.id
        );
        if (response.success) {
          this.deleteModal?.hide();
          this.toast.success("Account deleted successfully");
          // Log out and redirect to login page
          await this.$store.dispatch("user/logout");
          this.$router.push("/login");
        } else {
          this.deleteError = response.message || "Failed to delete account";
        }
      } catch (error: any) {
        this.deleteError =
          error.message || "An error occurred while deleting your account";
        console.error("Delete account error:", error);
      } finally {
        this.deleting = false;
      }
    },

    navigateToTopic(topic: TopicData) {
      // Get the topic ID from the topicId string
      const topicId = topic.topicId;

      // Navigate directly to the topic problems view
      this.$router.push(`/topics/${topicId}/problems`);

      // Log for debugging
      console.log(`Navigating to topic problems for topic ID: ${topicId}`);
    },
  },
  async mounted() {
    await this.loadProfile();
    await this.loadMathData();
  },
});
</script>

<style scoped>
.profile-avatar {
  background-color: #f8f9fa;
  color: #0d6efd;
}

/* Progress circle animation */
@keyframes progress-fill {
  0% {
    stroke-dasharray: 0, 100;
  }
}

svg path:nth-child(2) {
  animation: progress-fill 1.5s ease-in-out forwards;
}

/* Card hover effects */
.topic-card {
  transition: all 0.3s ease;
}

.topic-card:hover {
  transform: translateY(-5px);
  box-shadow:
    0 10px 15px -3px rgba(0, 0, 0, 0.1),
    0 4px 6px -2px rgba(0, 0, 0, 0.05);
  border-color: #3b82f6; /* primary-500 color */
}
</style>
