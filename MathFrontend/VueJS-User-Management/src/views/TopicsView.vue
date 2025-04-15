<template>
  <div class="topics-view p-6">
    <h1 class="text-3xl font-bold mb-6">Math Topics</h1>
    <p class="mb-6 text-gray-600">
      Browse through different math topics, track your progress, and practice problems in various difficulty levels.
    </p>
    
    <SubjectSelector @generate-problem="handleGenerateFromTopic" />
    
    <div v-if="generatedProblem" class="bg-white shadow-md rounded-lg p-6 mt-8">
      <h2 class="text-2xl font-bold mb-4">Generated Problem</h2>
      <div class="mb-6">
        <h3 class="font-semibold mb-2">Topic: {{ currentTopic }}</h3>
        <h4 class="font-semibold mb-2">Problem Statement:</h4>
        <div class="p-4 bg-gray-50 rounded-md" v-html="formattedStatement"></div>
      </div>
      
      <div class="mb-4">
        <label for="answer" class="block mb-2 font-medium">Your Answer:</label>
        <input
          id="answer"
          v-model="userAnswer"
          class="w-full p-2 border rounded-md"
          placeholder="Enter your answer here"
        />
      </div>
      
      <div class="flex flex-wrap gap-2 mb-4">
        <button 
          @click="checkAnswer" 
          class="bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700"
          :disabled="!userAnswer || isChecking"
        >
          <span v-if="isChecking">Checking...</span>
          <span v-else>Check Answer</span>
        </button>
        
        <button 
          @click="showSolution" 
          class="bg-yellow-500 text-white py-2 px-4 rounded-md hover:bg-yellow-600"
        >
          Show Solution
        </button>
      </div>
      
      <div v-if="evaluation" class="mt-6 p-4 rounded-md" :class="evaluation.isCorrect ? 'bg-green-50' : 'bg-red-50'">
        <h4 class="font-semibold mb-2">Feedback:</h4>
        <p>{{ evaluation.feedback }}</p>
      </div>
      
      <div v-if="solutionVisible" class="mt-6">
        <h4 class="font-semibold mb-2">Solution:</h4>
        <div class="p-4 bg-gray-50 rounded-md" v-html="formattedSolution"></div>
        
        <h4 class="font-semibold mt-4 mb-2">Explanation:</h4>
        <div class="p-4 bg-gray-50 rounded-md" v-html="formattedExplanation"></div>
      </div>
    </div>
  </div>
</template>

<script>
import SubjectSelector from '@/components/MathProblems/SubjectSelector.vue';
import axios from 'axios';
import { ref, computed } from 'vue';
import { evaluateMathAnswer } from '@/api/math';

export default {
  name: 'TopicsView',
  components: {
    SubjectSelector
  },
  
  setup() {
    const currentTopic = ref('');
    const generatedProblem = ref(null);
    const userAnswer = ref('');
    const evaluation = ref(null);
    const isChecking = ref(false);
    const solutionVisible = ref(false);
    
    const formattedStatement = computed(() => {
      return generatedProblem.value?.statement.replace(/\n/g, '<br>');
    });
    
    const formattedSolution = computed(() => {
      return generatedProblem.value?.solution.replace(/\n/g, '<br>');
    });
    
    const formattedExplanation = computed(() => {
      return generatedProblem.value?.explanation.replace(/\n/g, '<br>');
    });
    
    async function handleGenerateFromTopic(request) {
      try {
        currentTopic.value = request.topic;
        userAnswer.value = '';
        evaluation.value = null;
        solutionVisible.value = false;
        
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
    
    async function checkAnswer() {
      if (!userAnswer.value) {
        alert('Please enter your answer');
        return;
      }
      
      isChecking.value = true;
      
      try {
        evaluation.value = await evaluateMathAnswer({
          problem: generatedProblem.value.statement,
          userAnswer: userAnswer.value
        });
        
        // Automatically show solution if answer is incorrect
        if (!evaluation.value.isCorrect) {
          solutionVisible.value = true;
        }
      } catch (error) {
        console.error('Error checking answer:', error);
        alert('Failed to check answer. Please try again.');
      } finally {
        isChecking.value = false;
      }
    }
    
    function showSolution() {
      solutionVisible.value = true;
    }
    
    return {
      currentTopic,
      generatedProblem,
      userAnswer,
      evaluation,
      isChecking,
      solutionVisible,
      formattedStatement,
      formattedSolution,
      formattedExplanation,
      handleGenerateFromTopic,
      checkAnswer,
      showSolution
    };
  }
};
</script>

<style scoped>
.topics-view {
  max-width: 1200px;
  margin: 0 auto;
}
</style> 