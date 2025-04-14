import { api } from './user';

interface GenerateMathProblemRequest {
  topic: string;
  difficulty: string;
  topicId?: number;
}

interface GeneratedMathProblemResponse {
  statement: string;
  solution: string;
  explanation: string;
}

interface EvaluateMathAnswerRequest {
  problem: string;
  userAnswer: string;
}

interface EvaluateMathAnswerResponse {
  isCorrect: boolean;
  feedback: string;
}

export const generateMathProblem = async (request: GenerateMathProblemRequest): Promise<GeneratedMathProblemResponse> => {
  try {
    const response = await api.post('/MathProblem/generate', request);
    return response.data;
  } catch (error) {
    console.error('Error generating math problem:', error);
    throw error;
  }
};

export const evaluateMathAnswer = async (request: EvaluateMathAnswerRequest): Promise<EvaluateMathAnswerResponse> => {
  try {
    const response = await api.post('/MathProblem/evaluate-direct', request);
    return response.data;
  } catch (error) {
    console.error('Error evaluating math answer:', error);
    throw error;
  }
}; 