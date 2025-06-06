<template>
  <div class="min-h-screen bg-gray-50">
    <Sidebar />
    <section class="pt-14 sm:pt-20 md:pt-2 p-2 sm:p-4 md:p-6 lg:p-8">
      <div class="max-w-7xl mx-auto">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200">
          <div class="p-3 sm:p-4 md:p-6 lg:p-8">
                <div v-if="loading" class="flex justify-center">
                  <div
                    class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"
                  ></div>
                </div>
                <div v-else>
                  <!-- Main Content Layout -->
                  <div class="flex flex-col xl:flex-row gap-4 md:gap-6">
                    <!-- Left Section: Progress and Topics -->
                    <div class="flex-1 space-y-4 md:space-y-6">
                      <!-- Math Progress Widget -->
                      <div
                        class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden"
                      >
                        <div class="bg-primary-600 px-3 py-2 sm:px-4 sm:py-3">
                          <h5 class="text-base sm:text-lg font-medium text-white">
                            Math Learning Progress
                          </h5>
                        </div>
                        <div class="p-3 sm:p-4">
                          <div class="flex items-center mb-3 sm:mb-4">
                            <div
                              class="w-12 h-12 sm:w-16 sm:h-16 bg-gray-100 rounded-full flex items-center justify-center text-lg sm:text-xl font-medium text-primary-600 mr-3 sm:mr-4"
                            >
                              <svg
                                class="w-6 h-6 sm:w-8 sm:h-8"
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
                              <h3 class="text-xl sm:text-2xl font-bold text-gray-900">
                                {{ progress }}%
                              </h3>
                              <p class="text-sm sm:text-base text-gray-600">Overall Completion</p>
                            </div>
                          </div>

                          <!-- Progress Bar -->
                          <div class="h-2 bg-gray-200 rounded-full mb-4 sm:mb-6">
                            <div
                              class="h-2 rounded-full bg-green-500"
                              :style="{ width: progress + '%' }"
                            ></div>
                          </div>

                          <!-- Stats -->
                          <div class="grid grid-cols-2 gap-2 sm:gap-4 mb-3 sm:mb-4">
                            <div class="bg-gray-50 p-2 sm:p-3 rounded-lg text-center">
                              <h5 class="text-lg sm:text-xl font-semibold text-gray-900">
                                {{ completedProblems }}
                              </h5>
                              <p class="text-xs sm:text-sm text-gray-600">
                                Problems Solved
                              </p>
                            </div>
                            <div class="bg-gray-50 p-2 sm:p-3 rounded-lg text-center">
                              <h5 class="text-lg sm:text-xl font-semibold text-gray-900">
                                {{ skillLevel }}
                              </h5>
                              <p class="text-xs sm:text-sm text-gray-600">Current Skill</p>
                            </div>
                          </div>
                        </div>

                        <div
                          class="bg-gray-50 px-3 py-2 sm:px-4 sm:py-3 border-t border-gray-200"
                        >
                          <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-2 sm:gap-0">
                            <span class="text-xs sm:text-sm text-gray-600"
                              >Last activity: {{ lastAchievement }}</span
                            >
                            <button
                              class="inline-flex items-center px-2 py-1 sm:px-3 sm:py-1 border border-transparent text-xs sm:text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                              @click="$router.push('/topics')"
                            >
                              View All Topics
                            </button>
                          </div>
                        </div>
                      </div>

                      <!-- Topic Progress Section -->
                      <div
                        class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden"
                      >
                        <div class="bg-primary-600 px-3 py-2 sm:px-4 sm:py-3">
                          <h5 class="text-base sm:text-lg font-medium text-white">
                            Topic Progress
                          </h5>
                        </div>
                        <div class="p-2 sm:p-4">
                          <div class="grid grid-cols-2 gap-2 sm:gap-4">
                            <!-- Filter out topics with 0 total points and map remaining ones -->
                            <div
                              v-for="topic in activeTopics"
                              :key="topic.topicId"
                              @click="navigateToTopic(topic)"
                              class="relative bg-white rounded-lg shadow-sm p-2 sm:p-4 flex flex-col items-center justify-center hover:shadow-md transition-shadow border border-gray-100 topic-card cursor-pointer"
                            >
                              <!-- Circular Progress -->
                              <div class="relative w-12 h-12 sm:w-16 sm:h-16 md:w-20 md:h-20">
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
                                  <span class="text-xs sm:text-sm md:text-lg font-bold text-gray-700"
                                    >{{ topic.percentageCompleted }}%</span
                                  >
                                </div>
                              </div>

                              <!-- Topic Name and Points -->
                              <h4
                                class="mt-1 sm:mt-2 text-xs sm:text-sm font-medium text-gray-900 text-center hover:text-primary-600 transition-colors leading-tight"
                              >
                                {{ topic.topicName }}
                              </h4>
                              <p class="mt-0.5 sm:mt-1 text-xs text-gray-500">
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
                                class="absolute top-1 right-1 sm:top-2 sm:right-2 px-1 py-0.5 sm:px-2 sm:py-1 text-xs font-medium rounded-full"
                              >
                                {{ getStatusText(topic.percentageCompleted) }}
                              </span>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Right Section: User Profile -->
                    <div class="xl:w-1/3">
                      <div
                        class="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden"
                      >
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">
                            Profile Information
                          </h5>
                        </div>
                        <div class="p-4">
                          <!-- Profile Header -->
                          <div class="mb-6">
                            <!-- Mobile Layout -->
                            <div class="md:hidden">
                              <div class="text-center mb-4">
                                <div
                                  class="w-20 h-20 bg-gray-100 rounded-full flex items-center justify-center text-2xl font-medium text-primary-600 mx-auto mb-3"
                                >
                                  {{ user.firstName?.charAt(0)
                                  }}{{ user.lastName?.charAt(0) }}
                                </div>
                                <h3 class="text-xl font-bold text-gray-900">
                                  {{ user.firstName }} {{ user.lastName }}
                                </h3>
                                <p class="text-gray-600 text-sm">{{ user.email }}</p>
                              </div>
                              <div class="flex flex-col space-y-2">
                                <button
                                  @click="showEditMode"
                                  class="w-full inline-flex items-center justify-center px-4 py-3 border border-primary-600 text-sm font-medium rounded-md text-primary-600 bg-white hover:bg-primary-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
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
                                  class="w-full inline-flex items-center justify-center px-4 py-3 border border-red-600 text-sm font-medium rounded-md text-red-600 bg-white hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
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

                            <!-- Desktop Layout -->
                            <div class="hidden md:block">
                              <div class="flex items-center space-x-4 mb-4">
                                <div
                                  class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center text-xl font-medium text-primary-600"
                                >
                                  {{ user.firstName?.charAt(0)
                                  }}{{ user.lastName?.charAt(0) }}
                                </div>
                                <div>
                                  <h3 class="text-xl font-bold text-gray-900">
                                    {{ user.firstName }} {{ user.lastName }}
                                  </h3>
                                  <p class="text-gray-600">{{ user.email }}</p>
                                </div>
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
                          </div>

                          <!-- Profile Details -->
                          <div class="space-y-4">
                            <div class="border-t border-gray-200 pt-4">
                              <h6
                                class="text-sm font-medium text-gray-900 mb-2"
                              >
                                Account Details
                              </h6>
                              <div class="space-y-2">
                                <div class="flex justify-between">
                                  <span class="text-sm text-gray-500"
                                    >Member Since</span
                                  >
                                  <span class="text-sm text-gray-900">{{
                                    formatDate(user.createdAt)
                                  }}</span>
                                </div>
                                <div class="flex justify-between">
                                  <span class="text-sm text-gray-500"
                                    >Last Login</span
                                  >
                                  <span class="text-sm text-gray-900">{{
                                    formatDate(user.lastLogin)
                                  }}</span>
                                </div>
                                <div class="flex justify-between">
                                  <span class="text-sm text-gray-500"
                                    >Account Status</span
                                  >
                                  <span class="text-sm text-gray-900">{{
                                    user.isVerified ? "Verified" : "Pending"
                                  }}</span>
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
      <div class="modal-dialog modal-dialog-centered mx-3 md:mx-auto">
        <div class="modal-content rounded-lg shadow-lg border border-gray-200 max-w-md mx-auto">
          <div class="modal-header bg-red-600 text-white px-4 py-3">
            <h5 class="modal-title text-lg font-medium" id="deleteModalLabel">
              Confirm Account Deletion
            </h5>
            <button
              type="button"
              class="text-white opacity-80 hover:opacity-100 focus:outline-none ml-auto"
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
            <p class="text-gray-700 mb-4 text-sm md:text-base">
              Are you sure you want to delete your account? This action cannot
              be undone and all your data will be permanently removed.
            </p>
            <div
              v-if="deleteError"
              class="bg-red-50 text-red-700 p-3 rounded-md border border-red-200 text-sm"
            >
              {{ deleteError }}
            </div>
          </div>
          <div
            class="modal-footer bg-gray-50 px-4 py-3 border-t border-gray-200"
          >
            <!-- Mobile Layout -->
            <div class="flex flex-col space-y-2 md:hidden">
              <button
                type="button"
                class="w-full px-4 py-3 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                data-bs-dismiss="modal"
              >
                Cancel
              </button>
              <button
                type="button"
                class="w-full inline-flex items-center justify-center px-4 py-3 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 disabled:opacity-50"
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

            <!-- Desktop Layout -->
            <div class="hidden md:flex justify-end space-x-3">
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
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Navbar from "@/components/Home/Navbar.vue";
import Sidebar from "@/components/Home/Sidebar.vue";
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
    Sidebar,
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
