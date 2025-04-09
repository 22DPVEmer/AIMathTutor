<template>
  <div>
    <section class="p-3 p-md-4 p-xl-5">
      <div class="container-fluid">
        <div class="card border-light-subtle shadow-sm">
          <div class="row g-0">
            <Sidebar />
            <div class="col-12 col-md-9">
              <div class="card-body p-3 p-md-4 p-xl-5">
                <div v-if="loading" class="d-flex justify-content-center">
                  <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                  </div>
                </div>
                <div v-else>
                  <!-- Profile Header -->
                  <div
                    class="mb-4 d-flex justify-content-between align-items-center"
                  >
                    <div>
                      <h3 class="mb-0">Welcome, {{ user.firstName }}</h3>
                      <p class="text-secondary mb-0">{{ user.email }}</p>
                    </div>
                    <div>
                      <button
                        @click="showEditMode"
                        class="btn btn-outline-primary me-2"
                      >
                        <i class="bi bi-pencil-square me-1"></i> Edit Profile
                      </button>
                      <button
                        @click="confirmDelete"
                        class="btn btn-outline-danger"
                      >
                        <i class="bi bi-trash me-1"></i> Delete Account
                      </button>
                    </div>
                  </div>

                  <div class="row">
                    <!-- Math Progress Widget -->
                    <MathProgressWidget :progress="progress" />

                    <!-- Profile Card (View Mode) -->
                    <div class="col-md-6" v-if="!editMode">
                      <div class="card mb-4 shadow-sm border-0">
                        <div class="card-header bg-primary text-white">
                          <h5 class="card-title mb-0">User Profile</h5>
                        </div>
                        <div class="card-body">
                          <div class="d-flex mb-3">
                            <div
                              class="profile-avatar me-3 bg-light rounded-circle d-flex justify-content-center align-items-center"
                              style="width: 64px; height: 64px; font-size: 24px"
                            >
                              {{ user.firstName?.charAt(0)
                              }}{{ user.lastName?.charAt(0) }}
                            </div>
                            <div>
                              <h5 class="mb-1">
                                {{ user.firstName }} {{ user.lastName }}
                              </h5>
                              <p class="mb-0 text-muted">
                                <i class="bi bi-envelope me-1"></i
                                >{{ user.email }}
                              </p>
                            </div>
                          </div>
                          <hr />
                          <div class="row mb-2">
                            <div class="col-sm-4 text-muted">First Name:</div>
                            <div class="col-sm-8">{{ user.firstName }}</div>
                          </div>
                          <div class="row mb-2">
                            <div class="col-sm-4 text-muted">Last Name:</div>
                            <div class="col-sm-8">{{ user.lastName }}</div>
                          </div>
                          <div class="row mb-2">
                            <div class="col-sm-4 text-muted">Email:</div>
                            <div class="col-sm-8">{{ user.email }}</div>
                          </div>
                          <div class="row mb-2">
                            <div class="col-sm-4 text-muted">Created:</div>
                            <div class="col-sm-8">
                              {{ formatDate(user.createdAt) }}
                            </div>
                          </div>
                          <div class="row mb-2">
                            <div class="col-sm-4 text-muted">Last Login:</div>
                            <div class="col-sm-8">
                              {{ formatDate(user.lastLogin) }}
                            </div>
                          </div>
                          <div class="row mb-2">
                            <div class="col-sm-4 text-muted">Verified:</div>
                            <div class="col-sm-8">
                              <span
                                class="badge"
                                :class="
                                  user.isVerified ? 'bg-success' : 'bg-warning'
                                "
                              >
                                {{ user.isVerified ? "Yes" : "No" }}
                              </span>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Edit Profile Form (Edit Mode) -->
                    <div class="col-md-6" v-if="editMode">
                      <div class="card mb-4 shadow-sm border-0">
                        <div class="card-header bg-primary text-white">
                          <h5 class="card-title mb-0">Edit Profile</h5>
                        </div>
                        <div class="card-body">
                          <form @submit.prevent="updateProfile">
                            <div class="mb-3">
                              <label for="firstName" class="form-label"
                                >First Name</label
                              >
                              <input
                                type="text"
                                class="form-control"
                                id="firstName"
                                v-model="formData.firstName"
                                required
                              />
                            </div>
                            <div class="mb-3">
                              <label for="lastName" class="form-label"
                                >Last Name</label
                              >
                              <input
                                type="text"
                                class="form-control"
                                id="lastName"
                                v-model="formData.lastName"
                                required
                              />
                            </div>
                            <div class="mb-3">
                              <label for="email" class="form-label"
                                >Email</label
                              >
                              <input
                                type="email"
                                class="form-control"
                                id="email"
                                v-model="formData.email"
                                required
                              />
                            </div>
                            <div class="d-flex justify-content-end gap-2">
                              <button
                                type="button"
                                @click="cancelEdit"
                                class="btn btn-outline-secondary"
                              >
                                Cancel
                              </button>
                              <button
                                type="submit"
                                class="btn btn-primary"
                                :disabled="updating"
                              >
                                <span
                                  v-if="updating"
                                  class="spinner-border spinner-border-sm me-1"
                                  role="status"
                                  aria-hidden="true"
                                ></span>
                                Save Changes
                              </button>
                            </div>
                          </form>
                        </div>
                      </div>
                    </div>

                    <!-- Change Password Card -->
                    <div class="col-md-6" v-if="editMode">
                      <div class="card mb-4 shadow-sm border-0">
                        <div class="card-header bg-primary text-white">
                          <h5 class="card-title mb-0">Change Password</h5>
                        </div>
                        <div class="card-body">
                          <form @submit.prevent="changePassword">
                            <div class="mb-3">
                              <label for="currentPassword" class="form-label"
                                >Current Password</label
                              >
                              <input
                                type="password"
                                class="form-control"
                                id="currentPassword"
                                v-model="passwordData.currentPassword"
                                required
                              />
                            </div>
                            <div class="mb-3">
                              <label for="newPassword" class="form-label"
                                >New Password</label
                              >
                              <input
                                type="password"
                                class="form-control"
                                id="newPassword"
                                v-model="passwordData.newPassword"
                                required
                                minlength="6"
                              />
                            </div>
                            <div class="mb-3">
                              <label for="confirmPassword" class="form-label"
                                >Confirm New Password</label
                              >
                              <input
                                type="password"
                                class="form-control"
                                id="confirmPassword"
                                v-model="passwordData.confirmPassword"
                                required
                              />
                              <div
                                class="form-text text-danger"
                                v-if="passwordMismatch"
                              >
                                Passwords do not match
                              </div>
                            </div>
                            <div class="d-flex justify-content-end">
                              <button
                                type="submit"
                                class="btn btn-primary"
                                :disabled="changingPassword || passwordMismatch"
                              >
                                <span
                                  v-if="changingPassword"
                                  class="spinner-border spinner-border-sm me-1"
                                  role="status"
                                  aria-hidden="true"
                                ></span>
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
import { Modal } from "bootstrap";
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
