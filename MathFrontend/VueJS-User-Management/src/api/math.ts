import { api } from "./user";

interface GenerateMathProblemRequest {
  topic: string;
  difficulty: string;
  topicId?: number;
  saveToDatabase?: boolean;
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

interface SaveProblemAttemptRequest {
  statement: string;
  solution: string;
  explanation: string;
  userAnswer: string;
  isCorrect: boolean;
  difficulty: string;
  topic: string;
  topicId?: number;
}

interface UserMathProblemModel {
  id: number;
  statement: string;
  solution: string;
  explanation: string;
  topicName: string;
  difficulty: string;
  createdAt: string;
  isCorrect: boolean;
  userAnswer: string;
  userId: string;
  userName: string;
  topicId?: number;
}

interface MathTopicModel {
  id: number;
  name: string;
  description: string;
  difficulty: string;
  categoryId: number;
  categoryName: string;
  problemCount: number;
}

export const generateMathProblem = async (
  request: GenerateMathProblemRequest
): Promise<GeneratedMathProblemResponse> => {
  try {
    const response = await api.post("/MathProblem/generate", request);
    return response.data;
  } catch (error) {
    console.error("Error generating math problem:", error);
    throw error;
  }
};

export const evaluateMathAnswer = async (
  request: EvaluateMathAnswerRequest
): Promise<EvaluateMathAnswerResponse> => {
  try {
    const response = await api.post("/MathProblem/evaluate-direct", request);
    return response.data;
  } catch (error) {
    console.error("Error evaluating math answer:", error);
    throw error;
  }
};

export const saveProblemAttempt = async (
  request: SaveProblemAttemptRequest
): Promise<boolean> => {
  try {
    const response = await api.post("/MathProblem/save-attempt", request);
    return response.data;
  } catch (error) {
    console.error("Error saving problem attempt:", error);
    throw error;
  }
};

export const saveUserMathProblem = async (
  request: SaveProblemAttemptRequest
): Promise<UserMathProblemModel> => {
  try {
    const response = await api.post("/UserMathProblem/save-generated", request);
    return response.data;
  } catch (error) {
    console.error("Error saving user math problem:", error);
    throw error;
  }
};

export const getUserMathProblems = async (): Promise<
  UserMathProblemModel[]
> => {
  try {
    const response = await api.get("/UserMathProblem");
    return response.data;
  } catch (error) {
    console.error("Error getting user math problems:", error);
    throw error;
  }
};

export const getUserMathProblemsByUserId = async (
  userId: string
): Promise<UserMathProblemModel[]> => {
  try {
    const response = await api.get(`/UserMathProblem/user/${userId}`);
    return response.data;
  } catch (error) {
    console.error(
      `Error getting user math problems for user ${userId}:`,
      error
    );
    throw error;
  }
};

export const deleteUserMathProblem = async (id: number): Promise<boolean> => {
  try {
    await api.delete(`/UserMathProblem/${id}`);
    return true;
  } catch (error) {
    console.error(`Error deleting user math problem ${id}:`, error);
    throw error;
  }
};

export const getCurrentUserMathProblems = async (): Promise<
  UserMathProblemModel[]
> => {
  try {
    const response = await api.get("/UserMathProblem/my-problems");
    if (response.data && Array.isArray(response.data)) {
      return response.data;
    } else {
      console.error("Unexpected response format:", response.data);
      return [];
    }
  } catch (error) {
    console.error("Error getting current user math problems:", error);
    return [];
  }
};

export const retryUserMathProblem = async (
  id: number,
  userAnswer: string
): Promise<any> => {
  try {
    const response = await api.post(`/UserMathProblem/retry/${id}`, {
      userAnswer,
    });
    return response.data;
  } catch (error) {
    console.error(`Error retrying user math problem ${id}:`, error);
    throw error;
  }
};

export const updateUserMathProblem = async (
  id: number,
  problemData: Partial<UserMathProblemModel>
): Promise<UserMathProblemModel> => {
  try {
    const response = await api.put(`/UserMathProblem/${id}`, problemData);
    return response.data;
  } catch (error) {
    console.error(`Error updating user math problem ${id}:`, error);
    throw error;
  }
};

export const getAllTopics = async (): Promise<MathTopicModel[]> => {
  try {
    const response = await api.get("/MathTopic");
    return response.data;
  } catch (error) {
    console.error("Error getting math topics:", error);
    throw error;
  }
};
