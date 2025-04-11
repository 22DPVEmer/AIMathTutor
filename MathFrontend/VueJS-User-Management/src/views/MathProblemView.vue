<template>
  <div class="math-problem-view p-6">
    <h1 class="text-3xl font-bold mb-6">Math Problem Generator</h1>
    <p class="mb-6 text-gray-600">
      Generate personalized math problems on any topic with different difficulty levels. 
      Test your knowledge and get instant feedback!
    </p>
    
    <div class="tabs mb-8">
      <button 
        @click="activeTab = 'topic'"
        :class="[
          'px-6 py-3 mr-2 rounded-t-lg',
          activeTab === 'topic' ? 'bg-blue-600 text-white' : 'bg-gray-200'
        ]"
      >
        Browse By Topic
      </button>
      <button 
        @click="activeTab = 'custom'"
        :class="[
          'px-6 py-3 rounded-t-lg',
          activeTab === 'custom' ? 'bg-blue-600 text-white' : 'bg-gray-200'
        ]"
      >
        Custom Problem
      </button>
    </div>
    
    <div v-if="activeTab === 'topic'" class="mb-8">
      <SubjectSelector @generate-problem="handleGenerateFromTopic" />
    </div>
    
    <div v-else-if="activeTab === 'custom'" class="mb-8">
      <MathProblemGenerator ref="problemGenerator" />
    </div>
    
    <div v-if="generatedProblem" class="bg-white shadow-md rounded-lg p-6 mt-8">
      <h2 class="text-2xl font-bold mb-4">Generated Problem</h2>
      <div class="mb-6">
        <h3 class="font-semibold mb-2">Topic: {{ currentTopic }}</h3>
        <h4 class="font-semibold mb-2">Problem Statement:</h4>
        <div class="p-4 bg-gray-50 rounded-md" v-html="formattedStatement"></div>
      </div>
      
      <!-- Problem solving interface would be similar to MathProblemGenerator component -->
    </div>
  </div>
</template>

<script>
import MathProblemGenerator from '@/components/MathProblems/MathProblemGenerator.vue';
import SubjectSelector from '@/components/MathProblems/SubjectSelector.vue';
import axios from 'axios';
import { ref, computed } from 'vue';

export default {
  name: 'MathProblemView',
  components: {
    MathProblemGenerator,
    SubjectSelector
  },
  
  setup() {
    const activeTab = ref('topic');
    const currentTopic = ref('');
    const generatedProblem = ref(null);
    const problemGenerator = ref(null);
    
    const formattedStatement = computed(() => {
      return generatedProblem.value?.statement.replace(/\n/g, '<br>');
    });
    
    async function handleGenerateFromTopic(request) {
      try {
        currentTopic.value = request.topic;
        
        const response = await axios.post('/api/MathProblem/generate', {
          topic: request.topic,
          difficulty: request.difficulty,
          topicId: request.topicId
        });
        
        generatedProblem.value = response.data;
      } catch (error) {
        console.error('Error generating problem:', error);
        alert('Failed to generate problem. Please try again.');
      }
    }
    
    return {
      activeTab,
      currentTopic,
      generatedProblem,
      problemGenerator,
      formattedStatement,
      handleGenerateFromTopic
    };
  }
};
</script>

<style scoped>
.math-problem-view {
  max-width: 1200px;
  margin: 0 auto;
}
</style> 