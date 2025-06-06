<script>
import Navbar from '@/components/Home/Navbar.vue';

export default {
  name: 'App',
  components: {
    Navbar
  }
};
</script>

<template>
  <div id="app">
    <Navbar />
    <main class="main-content">
      <router-view></router-view>
    </main>
  </div>
</template>

<style>
#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1;
  padding: 1rem;
  background-color: #f8f9fa;
}

/* Responsive padding for main content */
@media (min-width: 640px) {
  .main-content {
    padding: 1.5rem;
  }
}

@media (min-width: 768px) {
  .main-content {
    padding: 2rem;
  }
}

@media (min-width: 1024px) {
  .main-content {
    padding: 2.5rem;
  }
}

/* Bootstrap overrides and custom styles */
.btn-primary {
  background-color: #0d6efd;
  border-color: #0d6efd;
}

.btn-primary:hover {
  background-color: #0b5ed7;
  border-color: #0a58ca;
}

.navbar-dark .navbar-nav .nav-link {
  color: rgba(255, 255, 255, 0.85);
}

.navbar-dark .navbar-nav .nav-link:hover {
  color: #ffffff;
}

.dropdown-menu {
  border: none;
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
}
</style>
