<template>
  <nav class="bg-primary-500 shadow-lg">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <div class="flex">
          <!-- Logo/Brand -->
          <router-link to="/" class="flex items-center px-2 text-white font-semibold text-xl hover:text-white">
            AI Math Tutor
          </router-link>

          <!-- Left side navigation items -->
          <div class="hidden sm:ml-6 sm:flex sm:space-x-4">
            <router-link 
              to="/practice"
              class="flex items-center text-white px-3 py-2 rounded-md text-sm font-medium transition-all duration-200 hover:bg-primary-600"
            >
              Practice
            </router-link>
            <router-link 
              to="/topics"
              class="flex items-center text-white px-3 py-2 rounded-md text-sm font-medium transition-all duration-200 hover:bg-primary-600"
            >
              Topics
            </router-link>
            <router-link 
              to="/math-problems"
              class="flex items-center text-white px-3 py-2 rounded-md text-sm font-medium transition-all duration-200 hover:bg-primary-600"
            >
              Problem Generator
            </router-link>
            <router-link 
              to="/my-problems"
              class="flex items-center text-white px-3 py-2 rounded-md text-sm font-medium transition-all duration-200 hover:bg-primary-600"
            >
              My Problems
            </router-link>
          </div>
        </div>

        <!-- Right side navigation items -->
        <div class="hidden sm:ml-6 sm:flex sm:items-center">
          <!-- Show these items when user is NOT authenticated -->
          <template v-if="!isAuthenticated">
            <router-link 
              to="/login"
              class="text-white px-3 py-2 rounded-md text-sm font-medium transition-all duration-200 hover:bg-primary-600"
            >
              Login
            </router-link>
            <router-link 
              to="/register"
              class="text-white px-3 py-2 rounded-md text-sm font-medium ml-4 transition-all duration-200 hover:bg-primary-600"
            >
              Register
            </router-link>
          </template>

          <!-- Show these items when user is authenticated -->
          <template v-else>
            <div class="ml-3 relative">
              <button
                @click="toggleDropdown"
                class="flex items-center text-white px-3 py-2 rounded-md text-sm font-medium focus:outline-none transition-all duration-200 hover:bg-primary-600"
                type="button"
              >
                <span>{{ userData?.firstName || "User" }}</span>
                <svg class="ml-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </button>
              
              <!-- Dropdown menu -->
              <div
                v-show="dropdownOpen"
                class="origin-top-right absolute right-0 mt-2 w-48 rounded-md shadow-lg bg-white ring-1 ring-black ring-opacity-5 divide-y divide-gray-100"
              >
                <div class="py-1">
                  <router-link
                    to="/profile"
                    class="group flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-primary-50 transition-all duration-200"
                  >
                    <svg class="mr-3 h-5 w-5 text-gray-400 group-hover:text-primary-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                    Profile
                  </router-link>
                </div>
                <div class="py-1">
                  <a
                    href="#"
                    @click.prevent="handleLogout"
                    class="group flex items-center px-4 py-2 text-sm text-gray-700 hover:bg-primary-50 transition-all duration-200"
                  >
                    <svg class="mr-3 h-5 w-5 text-gray-400 group-hover:text-primary-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                    </svg>
                    Logout
                  </a>
                </div>
              </div>
            </div>
          </template>
        </div>

        <!-- Mobile menu button -->
        <div class="flex items-center sm:hidden">
          <button
            @click="isMobileMenuOpen = !isMobileMenuOpen"
            class="inline-flex items-center justify-center p-2 rounded-md text-white hover:bg-primary-600 focus:outline-none transition-all duration-200"
          >
            <svg 
              class="h-6 w-6" 
              :class="{'hidden': isMobileMenuOpen, 'block': !isMobileMenuOpen }"
              fill="none" 
              stroke="currentColor" 
              viewBox="0 0 24 24"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
            <svg 
              class="h-6 w-6" 
              :class="{'block': isMobileMenuOpen, 'hidden': !isMobileMenuOpen }"
              fill="none" 
              stroke="currentColor" 
              viewBox="0 0 24 24"
            >
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Mobile menu -->
    <div 
      class="sm:hidden"
      :class="{'block': isMobileMenuOpen, 'hidden': !isMobileMenuOpen}"
    >
      <div class="px-2 pt-2 pb-3 space-y-1">
        <router-link 
          to="/practice"
          class="text-white block px-3 py-2 rounded-md text-base font-medium hover:bg-primary-600 transition-all duration-200"
        >
          Practice
        </router-link>
        <router-link 
          to="/topics"
          class="text-white block px-3 py-2 rounded-md text-base font-medium hover:bg-primary-600 transition-all duration-200"
        >
          Topics
        </router-link>
        <router-link 
          to="/math-problems"
          class="text-white block px-3 py-2 rounded-md text-base font-medium hover:bg-primary-600 transition-all duration-200"
        >
          Problem Generator
        </router-link>
        <router-link 
          to="/my-problems"
          class="text-white block px-3 py-2 rounded-md text-base font-medium hover:bg-primary-600 transition-all duration-200"
        >
          My Problems
        </router-link>
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
    const isMobileMenuOpen = ref(false);

    const toggleDropdown = () => {
      dropdownOpen.value = !dropdownOpen.value;
    };

    // Close dropdown when clicking outside
    const handleClickOutside = (event) => {
      if (dropdownOpen.value && !event.target.closest(".relative")) {
        dropdownOpen.value = false;
      }
    };

    // Add and remove event listeners
    if (typeof window !== "undefined") {
      window.addEventListener("click", handleClickOutside);
    }

    return {
      dropdownOpen,
      isMobileMenuOpen,
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
  beforeUnmount() {
    // Clean up event listener
    if (typeof window !== "undefined") {
      window.removeEventListener("click", this.handleClickOutside);
    }
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
