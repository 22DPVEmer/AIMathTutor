<template>
  <!-- Mobile/Tablet Collapsible Sidebar (hidden on desktop) -->
  <div class="md:hidden">
    <button
      @click="toggleSidebar"
      class="sidebar-btn fixed top-2 left-2 sm:top-4 sm:left-4 z-50 p-2 sm:p-3 bg-primary-600 text-white rounded-md shadow-lg"
      :class="{ 'bg-primary-700': sidebarOpen }"
    >
      <svg
        class="w-5 h-5 sm:w-6 sm:h-6"
        :class="{ 'rotate-90': sidebarOpen }"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M4 6h16M4 12h16M4 18h16"
        />
      </svg>
    </button>

    <!-- Sidebar Overlay -->
    <div
      v-if="sidebarOpen"
      @click="closeSidebar"
      @touchstart="handleTouchStart"
      @touchmove="handleTouchMove"
      @touchend="handleTouchEnd"
      class="md:hidden fixed inset-0 bg-black bg-opacity-50 z-40"
    ></div>

    <!-- Slide-out Sidebar -->
    <nav
      ref="sidebar"
      class="md:hidden fixed top-0 left-0 h-full w-56 sm:w-64 bg-white shadow-lg transform transition-transform duration-300 ease-in-out z-50"
      :class="{ 'translate-x-0': sidebarOpen, '-translate-x-full': !sidebarOpen }"
      @touchstart="handleSidebarTouchStart"
      @touchmove="handleSidebarTouchMove"
      @touchend="handleSidebarTouchEnd"
    >
      <div class="p-3 sm:p-4">
        <div class="flex items-center justify-between mb-4 sm:mb-6">
          <h2 class="text-base sm:text-lg font-bold text-gray-900">Menu</h2>
          <button
            @click="closeSidebar"
            class="p-1 text-gray-500 hover:text-gray-700"
          >
            <svg class="w-4 h-4 sm:w-5 sm:h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <ul class="space-y-1 sm:space-y-2">
          <li>
            <router-link
              to="/topics"
              @click="closeSidebar"
              class="flex items-center p-2 sm:p-3 text-gray-700 rounded-md hover:bg-primary-50 hover:text-primary-600 transition-colors text-sm sm:text-base"
              :class="{ 'bg-primary-100 text-primary-700': $route.path === '/topics' }"
            >
              <svg class="w-4 h-4 sm:w-5 sm:h-5 mr-2 sm:mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.746 0 3.332.477 4.5 1.253v13C19.832 18.477 18.246 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
              </svg>
              Topics
            </router-link>
          </li>
          <li>
            <router-link
              to="/math-problems"
              @click="closeSidebar"
              class="flex items-center p-2 sm:p-3 text-gray-700 rounded-md hover:bg-primary-50 hover:text-primary-600 transition-colors text-sm sm:text-base"
              :class="{ 'bg-primary-100 text-primary-700': $route.path === '/math-problems' }"
            >
              <svg class="w-4 h-4 sm:w-5 sm:h-5 mr-2 sm:mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
              </svg>
              Problem Generator
            </router-link>
          </li>
          <li>
            <router-link
              to="/my-problems"
              @click="closeSidebar"
              class="flex items-center p-2 sm:p-3 text-gray-700 rounded-md hover:bg-primary-50 hover:text-primary-600 transition-colors text-sm sm:text-base"
              :class="{ 'bg-primary-100 text-primary-700': $route.path === '/my-problems' }"
            >
              <svg class="w-4 h-4 sm:w-5 sm:h-5 mr-2 sm:mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              My Problems
            </router-link>
          </li>
          <li>
            <router-link
              to="/profile"
              @click="closeSidebar"
              class="flex items-center p-2 sm:p-3 text-gray-700 rounded-md hover:bg-primary-50 hover:text-primary-600 transition-colors text-sm sm:text-base"
              :class="{ 'bg-primary-100 text-primary-700': $route.path === '/profile' }"
            >
              <svg class="w-4 h-4 sm:w-5 sm:h-5 mr-2 sm:mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
              Profile
            </router-link>
          </li>
        </ul>
      </div>
    </nav>
  </div>
</template>

<script>
import { ref, onMounted, onUnmounted } from 'vue'

export default {
  name: 'Sidebar',
  setup() {
    const sidebarOpen = ref(false)
    const touchStartX = ref(0)
    const touchStartY = ref(0)
    const touchCurrentX = ref(0)
    const touchCurrentY = ref(0)
    const isDragging = ref(false)

    const toggleSidebar = () => {
      sidebarOpen.value = !sidebarOpen.value
    }

    const closeSidebar = () => {
      sidebarOpen.value = false
    }

    // Touch event handlers for swipe gestures
    const handleTouchStart = (e) => {
      touchStartX.value = e.touches[0].clientX
      touchStartY.value = e.touches[0].clientY
      isDragging.value = false
    }

    const handleTouchMove = (e) => {
      if (!touchStartX.value) return

      touchCurrentX.value = e.touches[0].clientX
      touchCurrentY.value = e.touches[0].clientY

      const deltaX = touchCurrentX.value - touchStartX.value
      const deltaY = Math.abs(touchCurrentY.value - touchStartY.value)

      // Only consider horizontal swipes (deltaY should be small)
      if (deltaY < 50) {
        isDragging.value = true

        // Swipe right from left edge to open sidebar
        if (touchStartX.value < 50 && deltaX > 50) {
          sidebarOpen.value = true
        }
        // Swipe left to close sidebar when it's open
        else if (sidebarOpen.value && deltaX < -50) {
          sidebarOpen.value = false
        }
      }
    }

    const handleTouchEnd = () => {
      touchStartX.value = 0
      touchStartY.value = 0
      touchCurrentX.value = 0
      touchCurrentY.value = 0
      isDragging.value = false
    }

    // Sidebar-specific touch handlers (prevent closing when touching inside sidebar)
    const handleSidebarTouchStart = (e) => {
      e.stopPropagation()
    }

    const handleSidebarTouchMove = (e) => {
      e.stopPropagation()
    }

    const handleSidebarTouchEnd = (e) => {
      e.stopPropagation()
    }

    // Global touch event listeners for swipe gestures
    onMounted(() => {
      document.addEventListener('touchstart', handleTouchStart, { passive: true })
      document.addEventListener('touchmove', handleTouchMove, { passive: true })
      document.addEventListener('touchend', handleTouchEnd, { passive: true })
    })

    onUnmounted(() => {
      document.removeEventListener('touchstart', handleTouchStart)
      document.removeEventListener('touchmove', handleTouchMove)
      document.removeEventListener('touchend', handleTouchEnd)
    })

    return {
      sidebarOpen,
      toggleSidebar,
      closeSidebar,
      handleTouchStart,
      handleTouchMove,
      handleTouchEnd,
      handleSidebarTouchStart,
      handleSidebarTouchMove,
      handleSidebarTouchEnd
    }
  }
}
</script>

<style scoped>
/* Sidebar button animation */
.sidebar-btn svg {
  transition: transform 0.3s ease;
}

.sidebar-btn:hover {
  transform: scale(1.05);
}

/* Smooth transitions for sidebar */
.transform {
  transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

/* Ensure sidebar is above other content */
nav {
  z-index: 50;
}

/* Mobile-friendly touch targets */
@media (max-width: 768px) {
  .sidebar-btn {
    padding: 8px;
    min-height: 40px;
    min-width: 40px;
  }
}

/* Very small screens (iPhone SE) - only for mobile sidebar */
@media (max-width: 375px) {
  .md\:hidden .sidebar-btn {
    padding: 6px;
    min-height: 36px;
    min-width: 36px;
    top: 8px;
    left: 8px;
  }

  .md\:hidden nav {
    width: 240px; /* Slightly smaller sidebar on very small screens */
  }
}

/* Prevent text selection during swipe gestures */
.no-select {
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
}
</style>
