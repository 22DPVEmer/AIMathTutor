<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-primary">
    <div class="container-fluid">
      <router-link class="navbar-brand" to="/">AI Math Tutor</router-link>

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <!-- Left side navigation items -->
        <ul class="navbar-nav me-auto">
          <li class="nav-item">
            <router-link class="nav-link" to="/practice">Practice</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/topics">Topics</router-link>
          </li>
          <li class="nav-item">
            <router-link class="nav-link" to="/math-problems">Math Generator</router-link>
          </li>
        </ul>

        <!-- Right side navigation items -->
        <ul class="navbar-nav">
          <!-- Show these items when user is NOT authenticated -->
          <template v-if="!isAuthenticated">
            <li class="nav-item">
              <router-link class="nav-link" to="/login">Login</router-link>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/register"
                >Register</router-link
              >
            </li>
          </template>

          <!-- Show these items when user is authenticated -->
          <template v-else>
            <li class="nav-item relative">
              <button
                @click="toggleDropdown"
                class="nav-link flex items-center focus:outline-none"
                type="button"
              >
                <span>{{ userData?.firstName || "User" }}</span>
              </button>
              <div
                v-show="dropdownOpen"
                class="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-50"
              >
                <router-link
                  to="/profile"
                  class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors duration-200"
                >
                  <i class="bi bi-person mr-2"></i>Profile
                </router-link>
                <div class="border-t border-gray-100"></div>
                <a
                  href="#"
                  @click.prevent="handleLogout"
                  class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors duration-200"
                >
                  <i class="bi bi-box-arrow-right mr-2"></i>Logout
                </a>
              </div>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </nav>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import { ref } from "vue";

export default {
  name: "Navbar",
  setup() {
    const dropdownOpen = ref(false);

    const toggleDropdown = () => {
      dropdownOpen.value = !dropdownOpen.value;
    };

    // Close dropdown when clicking outside
    const handleClickOutside = (event) => {
      if (dropdownOpen.value && !event.target.closest(".nav-item.relative")) {
        dropdownOpen.value = false;
      }
    };

    // Add and remove event listeners
    if (typeof window !== "undefined") {
      window.addEventListener("click", handleClickOutside);
    }

    return {
      dropdownOpen,
      toggleDropdown,
    };
  },
  computed: {
    ...mapGetters("user", ["isAuthenticated", "userData"]),
  },
  methods: {
    ...mapActions("user", ["logout"]),
    async handleLogout() {
      try {
        await this.logout();
        this.$router.push("/login");
      } catch (error) {
        console.error("Logout error:", error);
      }
    },
  },
};
</script>

<style scoped>
.navbar {
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.nav-link {
  padding: 0.5rem 1rem;
  transition: color 0.2s ease-in-out;
}

.nav-link:hover {
  color: rgba(255, 255, 255, 0.9) !important;
}

.absolute {
  position: absolute;
  z-index: 1050; /* Ensure dropdown appears above other elements */
  background-color: white; /* Ensure dropdown has a visible background */
  border: 1px solid rgba(0, 0, 0, 0.1); /* Add a subtle border */
  border-radius: 0.25rem; /* Rounded corners */
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); /* Add a shadow for depth */
}

.relative {
  position: relative;
}

.block {
  display: block;
}

.px-4 {
  padding-left: 1rem;
  padding-right: 1rem;
}

.py-2 {
  padding-top: 0.5rem;
  padding-bottom: 0.5rem;
}

.text-sm {
  font-size: 0.875rem;
}

.text-gray-700 {
  color: #4a5568;
}

.hover\:bg-gray-100:hover {
  background-color: #f7fafc;
}

.transition-colors {
  transition-property: color, background-color, border-color,
    text-decoration-color, fill, stroke;
}

.duration-200 {
  transition-duration: 200ms;
}

.z-50 {
  z-index: 50;
}
</style>
