<template>
  <div class="topics-view p-6">
    <h1 class="text-3xl font-bold mb-6">Math Topics</h1>
    <p class="mb-6 text-gray-600">
      Explore math topics by grade level. Click on a subtopic to practice
      problems.
    </p>

    <!-- Router view for nested routes -->
    <router-view></router-view>
  </div>
</template>

<script>
export default {
  name: "TopicsView",
};
</script>

<style scoped>
.topics-view {
  max-width: 1200px;
  margin: 0 auto;
}
</style>
