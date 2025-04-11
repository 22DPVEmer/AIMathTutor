<template>
  <div class="subject-selector">
    <h2 class="text-2xl font-bold mb-6">Math Topics</h2>
    
    <!-- Subject Navigation Tabs -->
    <div class="subject-tabs mb-6">
      <div class="flex border-b">
        <button 
          v-for="(subject, index) in subjects" 
          :key="index"
          @click="selectSubject(subject)"
          :class="[
            'px-4 py-2 text-center', 
            selectedSubject.id === subject.id 
              ? 'border-b-2 border-blue-500 text-blue-600 font-medium' 
              : 'text-gray-600 hover:text-blue-500'
          ]"
        >
          {{ subject.name }}
        </button>
      </div>
    </div>
    
    <!-- Topic Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div 
        v-for="(topic, index) in filteredTopics" 
        :key="index"
        @click="selectTopic(topic)"
        class="topic-card border rounded-lg p-4 cursor-pointer hover:border-blue-500 transition-colors"
        :class="{'border-blue-500 bg-blue-50': selectedTopic.id === topic.id}"
      >
        <div class="flex justify-between items-start">
          <h3 class="font-bold text-lg">{{ topic.name }}</h3>
          <span 
            class="difficulty-badge px-2 py-1 rounded-full text-xs" 
            :class="getDifficultyClass(topic.difficulty)"
          >
            {{ getDifficultyLabel(topic.difficulty) }}
          </span>
        </div>
        <p class="text-gray-600 text-sm mt-2">{{ topic.description }}</p>
        <div class="flex items-center mt-4">
          <div class="flex-grow">
            <div class="h-2 bg-gray-200 rounded-full">
              <div 
                class="h-2 bg-green-500 rounded-full" 
                :style="`width: ${topic.progress || 0}%`"
              ></div>
            </div>
          </div>
          <span class="text-xs text-gray-600 ml-2">{{ topic.progress || 0 }}%</span>
        </div>
      </div>
    </div>
    
    <!-- Difficulty Selector -->
    <div class="difficulty-selector mt-8 p-4 border rounded-lg" v-if="selectedTopic.id">
      <h3 class="font-bold mb-4">Select difficulty for {{ selectedTopic.name }}</h3>
      
      <div class="flex flex-wrap gap-3">
        <button 
          v-for="difficulty in difficulties" 
          :key="difficulty.value"
          @click="selectDifficulty(difficulty.value)"
          class="px-4 py-2 rounded-md transition-colors"
          :class="[
            selectedDifficulty === difficulty.value 
              ? 'bg-blue-600 text-white' 
              : 'bg-gray-100 hover:bg-gray-200'
          ]"
        >
          {{ difficulty.label }}
        </button>
      </div>
      
      <button 
        @click="generateProblem"
        class="mt-6 bg-green-600 text-white px-6 py-3 rounded-md hover:bg-green-700 transition-colors w-full"
        :disabled="!canGenerateProblem"
      >
        Generate Problem
      </button>
    </div>
  </div>
</template>

<script>
import { ref, computed } from 'vue';
import axios from 'axios';

export default {
  name: 'SubjectSelector',
  
  emits: ['topic-selected', 'generate-problem'],
  
  setup(props, { emit }) {
    // Sample data - would be fetched from API in real implementation
    const subjects = ref([
      { id: 1, name: 'Algebra' },
      { id: 2, name: 'Geometry' },
      { id: 3, name: 'Calculus' },
      { id: 4, name: 'Statistics' },
      { id: 5, name: 'Number Theory' }
    ]);
    
    const topics = ref([
      { id: 1, subjectId: 1, name: 'Linear Equations', description: 'Solve equations in the form ax + b = c', difficulty: 1, progress: 75 },
      { id: 2, subjectId: 1, name: 'Quadratic Equations', description: 'Solve equations in the form ax² + bx + c = 0', difficulty: 2, progress: 40 },
      { id: 3, subjectId: 1, name: 'Systems of Equations', description: 'Solve multiple equations with multiple variables', difficulty: 2, progress: 20 },
      { id: 4, subjectId: 1, name: 'Inequalities', description: 'Solve and graph inequalities', difficulty: 2, progress: 0 },
      { id: 5, subjectId: 2, name: 'Angles', description: 'Understand and calculate angles', difficulty: 1, progress: 90 },
      { id: 6, subjectId: 2, name: 'Triangles', description: 'Properties and calculations with triangles', difficulty: 1, progress: 65 },
      { id: 7, subjectId: 2, name: 'Circles', description: 'Understand properties of circles', difficulty: 2, progress: 30 },
      { id: 8, subjectId: 3, name: 'Limits', description: 'Understand and calculate limits', difficulty: 2, progress: 10 },
      { id: 9, subjectId: 3, name: 'Derivatives', description: 'Calculate and apply derivatives', difficulty: 3, progress: 5 },
      { id: 10, subjectId: 4, name: 'Probability', description: 'Calculate probabilities of events', difficulty: 2, progress: 50 },
      { id: 11, subjectId: 5, name: 'Prime Numbers', description: 'Understanding and working with prime numbers', difficulty: 1, progress: 80 },
    ]);
    
    const difficulties = [
      { value: 'easy', label: 'Easy' },
      { value: 'medium', label: 'Medium' },
      { value: 'hard', label: 'Hard' }
    ];
    
    const selectedSubject = ref({ id: 1, name: 'Algebra' });
    const selectedTopic = ref({});
    const selectedDifficulty = ref('');
    
    const filteredTopics = computed(() => {
      return topics.value.filter(topic => topic.subjectId === selectedSubject.value.id);
    });
    
    const canGenerateProblem = computed(() => {
      return selectedTopic.value.id && selectedDifficulty.value;
    });
    
    function selectSubject(subject) {
      selectedSubject.value = subject;
      selectedTopic.value = {};
      selectedDifficulty.value = '';
    }
    
    function selectTopic(topic) {
      selectedTopic.value = topic;
      selectedDifficulty.value = '';
      emit('topic-selected', topic);
    }
    
    function selectDifficulty(difficulty) {
      selectedDifficulty.value = difficulty;
    }
    
    function getDifficultyClass(level) {
      switch(level) {
        case 1: return 'bg-green-100 text-green-800';
        case 2: return 'bg-yellow-100 text-yellow-800';
        case 3: return 'bg-red-100 text-red-800';
        default: return 'bg-gray-100 text-gray-800';
      }
    }
    
    function getDifficultyLabel(level) {
      switch(level) {
        case 1: return 'Easy';
        case 2: return 'Medium';
        case 3: return 'Hard';
        default: return 'Unknown';
      }
    }
    
    function generateProblem() {
      if (!canGenerateProblem.value) return;
      
      emit('generate-problem', {
        topic: selectedTopic.value.name,
        difficulty: selectedDifficulty.value,
        topicId: selectedTopic.value.id
      });
    }
    
    // In real implementation, we would fetch subjects and topics from API
    async function fetchSubjects() {
      try {
        const response = await axios.get('/api/Subject');
        subjects.value = response.data;
        if (subjects.value.length > 0) {
          selectedSubject.value = subjects.value[0];
        }
      } catch (error) {
        console.error('Error fetching subjects:', error);
      }
    }
    
    async function fetchTopics(subjectId) {
      try {
        const response = await axios.get(`/api/Subject/${subjectId}/topics`);
        topics.value = response.data;
      } catch (error) {
        console.error('Error fetching topics:', error);
      }
    }
    
    // Uncomment to fetch from API in real implementation
    // onMounted(() => {
    //   fetchSubjects();
    // });
    
    return {
      subjects,
      topics,
      filteredTopics,
      difficulties,
      selectedSubject,
      selectedTopic,
      selectedDifficulty,
      canGenerateProblem,
      selectSubject,
      selectTopic,
      selectDifficulty,
      getDifficultyClass,
      getDifficultyLabel,
      generateProblem
    };
  }
};
</script>

<style scoped>
.subject-selector {
  max-width: 1000px;
  margin: 0 auto;
}

.topic-card {
  transition: all 0.2s ease;
}

.topic-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
}
</style> 