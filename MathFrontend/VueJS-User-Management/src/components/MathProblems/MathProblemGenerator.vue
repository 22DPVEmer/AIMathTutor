<template>
  <div class="math-problem-generator">
    <h2 class="text-2xl font-bold mb-4">Math Problem Generator</h2>
    
    <div class="bg-white shadow-md rounded-lg p-6 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
        <div>
          <label for="topic" class="block mb-2 font-medium">Topic</label>
          <input
            id="topic"
            v-model="formData.topic"
            class="w-full p-2 border rounded-md"
            placeholder="e.g. Algebra, Calculus, Geometry"
          />
        </div>
        <div>
          <label for="difficulty" class="block mb-2 font-medium">Difficulty</label>
          <select 
            id="difficulty" 
            v-model="formData.difficulty"
            class="w-full p-2 border rounded-md"
          >
            <option value="Easy">Easy</option>
            <option value="Medium">Medium</option>
            <option value="Hard">Hard</option>
          </select>
        </div>
      </div>
      

      
      <button 
        @click="generateProblem" 
        class="bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700"
        :disabled="isLoading"
      >
        <span v-if="isLoading">Generating...</span>
        <span v-else>Generate Problem</span>
      </button>
    </div>
    
    <div v-if="generatedProblem" class="bg-white shadow-md rounded-lg p-6">
      <h3 class="text-xl font-bold mb-4">Generated Problem</h3>
      
      <div class="mb-6">
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
        
        <button 
          v-if="evaluation"
          @click="saveAttempt" 
          class="bg-blue-500 text-white py-2 px-4 rounded-md hover:bg-blue-600"
          :disabled="isSaving || attemptSaved"
        >
          <span v-if="isSaving">Saving...</span>
          <span v-else-if="attemptSaved">Saved!</span>
          <span v-else>Save to Database</span>
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
import { ref, computed } from 'vue';
import { generateMathProblem, evaluateMathAnswer, saveUserMathProblem } from '@/api/math';

export default {
  name: 'MathProblemGenerator',
  
  setup() {
    const formData = ref({
      topic: '',
      difficulty: 'Medium',
      saveToDatabase: false
    });
    
    const isLoading = ref(false);
    const isChecking = ref(false);
    const isSaving = ref(false);
    const attemptSaved = ref(false);
    const generatedProblem = ref(null);
    const userAnswer = ref('');
    const evaluation = ref(null);
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
    
    async function generateProblem() {
      if (!formData.value.topic) {
        alert('Please enter a topic');
        return;
      }
      
      isLoading.value = true;
      evaluation.value = null;
      userAnswer.value = '';
      solutionVisible.value = false;
      attemptSaved.value = false;
      
      try {
        generatedProblem.value = await generateMathProblem({
          topic: formData.value.topic,
          difficulty: formData.value.difficulty,
          saveToDatabase: formData.value.saveToDatabase
        });
      } catch (error) {
        console.error('Error generating problem:', error);
        alert('Failed to generate problem. Please try again.');
      } finally {
        isLoading.value = false;
      }
    }
    
    async function checkAnswer() {
      if (!userAnswer.value) {
        alert('Please enter your answer');
        return;
      }
      
      isChecking.value = true;
      attemptSaved.value = false;
      
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
    
    async function saveAttempt() {
      if (!evaluation.value) {
        alert('You need to check your answer before saving');
        return;
      }
      
      isSaving.value = true;
      
      try {
        const result = await saveUserMathProblem({
          statement: generatedProblem.value.statement,
          solution: generatedProblem.value.solution,
          explanation: generatedProblem.value.explanation,
          userAnswer: userAnswer.value,
          isCorrect: evaluation.value.isCorrect,
          difficulty: formData.value.difficulty,
          topic: formData.value.topic
        });
        
        if (result) {
          attemptSaved.value = true;
        } else {
          alert('Failed to save the problem. Please try again.');
        }
      } catch (error) {
        console.error('Error saving problem:', error);
        alert('Failed to save the problem. Please try again.');
      } finally {
        isSaving.value = false;
      }
    }
    
    return {
      formData,
      isLoading,
      isChecking,
      isSaving,
      attemptSaved,
      generatedProblem,
      userAnswer,
      evaluation,
      solutionVisible,
      formattedStatement,
      formattedSolution,
      formattedExplanation,
      generateProblem,
      checkAnswer,
      showSolution,
      saveAttempt
    };
  }
};
</script>

<style scoped>
.math-problem-generator {
  max-width: 800px;
  margin: 0 auto;
}
</style> 