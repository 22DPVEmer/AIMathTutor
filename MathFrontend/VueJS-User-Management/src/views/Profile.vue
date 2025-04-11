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
                  <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
                </div>
                <div v-else>
                  <!-- Profile Header -->
                  <div class="mb-6 flex justify-between items-center">
                    <div>
                      <h3 class="text-2xl font-bold text-gray-900">Welcome, {{ user.firstName }}</h3>
                      <p class="text-gray-600">{{ user.email }}</p>
                    </div>
                    <div class="flex space-x-3">
                      <button
                        @click="showEditMode"
                        class="inline-flex items-center px-4 py-2 border border-primary-600 text-sm font-medium rounded-md text-primary-600 bg-white hover:bg-primary-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                      >
                        <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                        </svg>
                        Edit Profile
                      </button>
                      <button
                        @click="confirmDelete"
                        class="inline-flex items-center px-4 py-2 border border-red-600 text-sm font-medium rounded-md text-red-600 bg-white hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                      >
                        <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                        </svg>
                        Delete Account
                      </button>
                    </div>
                  </div>

                  <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <!-- Math Progress Widget -->
                    <MathProgressWidget :progress="progress" />

                    <!-- Profile Card (View Mode) -->
                    <div v-if="!editMode">
                      <div class="bg-white rounded-lg shadow overflow-hidden">
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">User Profile</h5>
                        </div>
                        <div class="p-4">
                          <div class="flex items-center mb-4">
                            <div class="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center text-xl font-medium text-gray-600 mr-4">
                              {{ user.firstName?.charAt(0) }}{{ user.lastName?.charAt(0) }}
                            </div>
                            <div>
                              <h5 class="text-lg font-medium text-gray-900">
                                {{ user.firstName }} {{ user.lastName }}
                              </h5>
                              <p class="text-gray-600 flex items-center">
                                <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                                </svg>
                                {{ user.email }}
                              </p>
                            </div>
                          </div>
                          <div class="border-t border-gray-200 pt-4">
                            <dl class="divide-y divide-gray-200">
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">First Name:</dt>
                                <dd class="text-sm text-gray-900 col-span-2">{{ user.firstName }}</dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">Last Name:</dt>
                                <dd class="text-sm text-gray-900 col-span-2">{{ user.lastName }}</dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">Email:</dt>
                                <dd class="text-sm text-gray-900 col-span-2">{{ user.email }}</dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">Created:</dt>
                                <dd class="text-sm text-gray-900 col-span-2">{{ formatDate(user.createdAt) }}</dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">Last Login:</dt>
                                <dd class="text-sm text-gray-900 col-span-2">{{ formatDate(user.lastLogin) }}</dd>
                              </div>
                              <div class="grid grid-cols-3 gap-4 py-3">
                                <dt class="text-sm font-medium text-gray-500">Verified:</dt>
                                <dd class="text-sm col-span-2">
                                  <span
                                    class="px-2 py-1 text-xs font-medium rounded-full"
                                    :class="user.isVerified ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'"
                                  >
                                    {{ user.isVerified ? "Yes" : "No" }}
                                  </span>
                                </dd>
                              </div>
                            </dl>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Edit Profile Form (Edit Mode) -->
                    <div v-if="editMode">
                      <div class="bg-white rounded-lg shadow overflow-hidden">
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">Edit Profile</h5>
                        </div>
                        <div class="p-4">
                          <form @submit.prevent="updateProfile" class="space-y-4">
                            <div>
                              <label for="firstName" class="block text-sm font-medium text-gray-700">First Name</label>
                              <input
                                type="text"
                                id="firstName"
                                v-model="formData.firstName"
                                required
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                            </div>
                            <div>
                              <label for="lastName" class="block text-sm font-medium text-gray-700">Last Name</label>
                              <input
                                type="text"
                                id="lastName"
                                v-model="formData.lastName"
                                required
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                            </div>
                            <div>
                              <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
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
                                <svg v-if="updating" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                </svg>
                                Save Changes
                              </button>
                            </div>
                          </form>
                        </div>
                      </div>
                    </div>

                    <!-- Change Password Card -->
                    <div v-if="editMode">
                      <div class="bg-white rounded-lg shadow overflow-hidden">
                        <div class="bg-primary-600 px-4 py-3">
                          <h5 class="text-lg font-medium text-white">Change Password</h5>
                        </div>
                        <div class="p-4">
                          <form @submit.prevent="changePassword" class="space-y-4">
                            <div>
                              <label for="currentPassword" class="block text-sm font-medium text-gray-700">Current Password</label>
                              <input
                                type="password"
                                id="currentPassword"
                                v-model="passwordData.currentPassword"
                                required
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                            </div>
                            <div>
                              <label for="newPassword" class="block text-sm font-medium text-gray-700">New Password</label>
                              <input
                                type="password"
                                id="newPassword"
                                v-model="passwordData.newPassword"
                                required
                                minlength="6"
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                            </div>
                            <div>
                              <label for="confirmPassword" class="block text-sm font-medium text-gray-700">Confirm New Password</label>
                              <input
                                type="password"
                                id="confirmPassword"
                                v-model="passwordData.confirmPassword"
                                required
                                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-primary-500 focus:ring-primary-500 sm:text-sm"
                              />
                              <p v-if="passwordMismatch" class="mt-1 text-sm text-red-600">
                                Passwords do not match
                              </p>
                            </div>
                            <div class="flex justify-end">
                              <button
                                type="submit"
                                :disabled="changingPassword || passwordMismatch"
                                class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                              >
                                <svg v-if="changingPassword" class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                </svg>
                                Update Password
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
        <div class="modal-content">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title" id="deleteModalLabel">
              Confirm Account Deletion
            </h5>
            <button
              type="button"
              class="btn-close btn-close-white"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <p>
              Are you sure you want to delete your account? This action cannot
              be undone and all your data will be permanently removed.
            </p>
            <div v-if="deleteError" class="alert alert-danger">
              {{ deleteError }}
            </div>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              data-bs-dismiss="modal"
            >
              Cancel
            </button>
            <button
              type="button"
              class="btn btn-danger"
              @click="deleteAccount"
              :disabled="deleting"
            >
              <span
                v-if="deleting"
                class="spinner-border spinner-border-sm me-1"
                role="status"
                aria-hidden="true"
              ></span>
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

import MathProgressWidget from "@/components/Home/MathProgressWidget.vue";
import { mapState, mapGetters } from "vuex";
import { useStore } from "@/store";

interface UserData {
  id?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  createdAt?: string;
  lastLogin?: string;
  isVerified?: boolean;
  [key: string]: any;
}

export default defineComponent({
  name: "Profile",
  components: {
    Navbar,
    MathProgressWidget,
  },
  data() {
    return {
      progress: 65, // Example math learning progress percentage
      editMode: false,
      formData: {
        firstName: "",
        lastName: "",
        email: "",
        id: "",
      },
      passwordData: {
        currentPassword: "",
        newPassword: "",
        confirmPassword: "",
      },
      updating: false,
      changingPassword: false,
      deleteModal: null as Modal | null,
      deleting: false,
      deleteError: "",
    };
  },
  computed: {
    ...mapGetters("user", ["userData"]),
    ...mapState("user", ["loading"]),
    user(): UserData {
      return this.userData;
    },
    passwordMismatch(): boolean {
      return (
        this.passwordData.newPassword !== this.passwordData.confirmPassword &&
        this.passwordData.confirmPassword !== ""
      );
    },
  },
  methods: {
    async loadProfile() {
      try {
        await this.$store.dispatch("user/getUserProfile");
      } catch (error) {
        console.error("Error loading profile:", error);
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
      this.passwordData = {
        currentPassword: "",
        newPassword: "",
        confirmPassword: "",
      };
    },
    async updateProfile() {
      this.updating = true;
      try {
        const response = await this.$store.dispatch(
          "user/updateUserProfile",
          this.formData
        );
        if (response.success) {
          this.$toast.success("Profile updated successfully");
          this.editMode = false;
        } else {
          this.$toast.error(response.message || "Failed to update profile");
        }
      } catch (error) {
        this.$toast.error("An error occurred while updating your profile");
        console.error("Profile update error:", error);
      } finally {
        this.updating = false;
      }
    },
    async changePassword() {
      if (this.passwordMismatch) return;

      this.changingPassword = true;
      try {
        const response = await this.$store.dispatch("user/changePassword", {
          userId: this.user.id,
          currentPassword: this.passwordData.currentPassword,
          newPassword: this.passwordData.newPassword,
        });

        if (response.success) {
          this.$toast.success("Password changed successfully");
          this.passwordData = {
            currentPassword: "",
            newPassword: "",
            confirmPassword: "",
          };
          this.editMode = false;
        } else {
          this.$toast.error(response.message || "Failed to change password");
        }
      } catch (error) {
        this.$toast.error("An error occurred while changing your password");
        console.error("Change password error:", error);
      } finally {
        this.changingPassword = false;
      }
    },
    confirmDelete() {
      this.deleteError = "";
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
          this.$toast.success("Account deleted successfully");
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
  },
  mounted() {
    this.loadProfile();
  },
});
</script>

<style scoped>
.profile-avatar {
  background-color: #f8f9fa;
  color: #0d6efd;
}
</style>
