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

interface GuidanceRequest {
  problem: string;
  solution: string;
  userAnswer: string;
  question: string;
}

interface GuidanceResponse {
  guidance: string;
}

interface SaveProblemAttemptRequest {
  name?: string;
  statement: string;
  solution: string;
  explanation: string;
  userAnswer: string;
  isCorrect: boolean;
  difficulty: string;
  topic: string;
  topicId?: number;
}

interface EvaluateAndSaveRequest {
  problem: string;
  userAnswer: string;
  solution?: string;
  explanation?: string;
  name?: string;
  difficulty?: string;
  topic?: string;
  topicId?: number;
}

interface EvaluateAndSaveResponse {
  success: boolean;
  isCorrect: boolean;
  feedback: string;
  hasExistingCorrectAttempt?: boolean;
  problems?: any[];
  attempts?: any[];
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
  schoolClassId: number;
  schoolClassName: string;
  problemCount: number;
  parentTopicId?: number;
  parentTopicName?: string;
  gradeLevel?: string;
  subtopics?: MathTopicModel[];
  // Completion data
  totalPointsPossible?: number;
  pointsEarned?: number;
  percentageCompleted?: number;
}

interface TopicCompletionDto {
  topicId: number;
  topicName: string;
  totalPointsPossible: number;
  pointsEarned: number;
  percentageCompleted: number;
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

export const getGuidance = async (
  request: GuidanceRequest
): Promise<GuidanceResponse> => {
  try {
    const response = await api.post("/MathProblem/get-guidance", request);
    return response.data;
  } catch (error) {
    throw error;
  }
};

export const saveProblemAttempt = async (
  request: SaveProblemAttemptRequest
): Promise<boolean> => {
  try {
    // Ensure difficulty is a string
    const formattedRequest = {
      ...request,
      difficulty:
        typeof request.difficulty === "string"
          ? request.difficulty
          : String(request.difficulty),
    };

    // Send the request as the attemptDto field
    const response = await api.post(
      "/MathProblem/save-attempt",
      formattedRequest
    );
    return response.data;
  } catch (error) {
    console.error("Error saving problem attempt:", error);
    throw error;
  }
};

export const evaluateAndSave = async (
  request: EvaluateAndSaveRequest
): Promise<EvaluateAndSaveResponse> => {
  try {
    // Ensure difficulty is a string if provided
    const formattedRequest = {
      ...request,
      difficulty:
        request.difficulty && typeof request.difficulty === "string"
          ? request.difficulty
          : request.difficulty
            ? String(request.difficulty)
            : undefined,
    };

    // Call the combined endpoint
    const response = await api.post(
      "/MathProblem/evaluate-and-save",
      formattedRequest
    );
    return response.data;
  } catch (error) {
    console.error("Error evaluating and saving math problem:", error);
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

export const getTopicCompletion = async (): Promise<TopicCompletionDto[]> => {
  try {
    const response = await api.get("/MathTopic/user-progress");
    return response.data;
  } catch (error) {
    console.error("Error getting topic completion data:", error);
    return [];
  }
};

export const publishUserMathProblem = async (id: number): Promise<any> => {
  try {
    // Get the name from localStorage if available
    const savedName = localStorage.getItem(`userMathProblem_${id}_name`);

    // If we have a saved name, pass it as a query parameter
    const url = savedName
      ? `UserMathProblem/publish/${id}?name=${encodeURIComponent(savedName)}`
      : `UserMathProblem/publish/${id}`;

    const response = await api.post(url);
    return response.data;
  } catch (error) {
    console.error(`Error publishing user math problem ${id}:`, error);
    throw error;
  }
};

export const updateMathProblem = async (
  id: number,
  problemData: {
    name?: string;
    statement?: string;
    solution?: string;
    explanation?: string;
    difficulty?: string | number;
    topicId?: number;
    pointValue?: number;
  }
): Promise<any> => {
  try {
    const response = await api.put(`/MathProblem/${id}`, problemData);
    return response.data;
  } catch (error) {
    console.error(`Error updating math problem ${id}:`, error);
    throw error;
  }
};

export const deleteMathProblem = async (id: number): Promise<boolean> => {
  try {
    await api.delete(`/MathProblem/${id}`);
    return true;
  } catch (error) {
    console.error(`Error deleting math problem ${id}:`, error);
    throw error;
  }
};
