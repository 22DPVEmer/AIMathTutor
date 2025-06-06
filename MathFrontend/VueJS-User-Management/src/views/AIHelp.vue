<template>
  <div class="ai-help-container">
    <!-- Sidebar -->
    <Sidebar />

    <!-- Main Content -->
    <main class="w-full pt-14 sm:pt-20 md:pt-3 px-3 px-md-4">
        <div
          class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom"
        >
          <h1 class="h2">AI Math Assistant</h1>
        </div>

        <!-- Chat Interface -->
        <div class="row">
          <div class="col-12">
            <div class="card">
              <div class="card-body">
                <div class="chat-container">
                  <!-- Chat Messages -->
                  <div class="chat-messages" ref="chatMessages">
                    <div
                      v-for="(message, index) in messages"
                      :key="index"
                      :class="['message', message.type]"
                    >
                      <div class="message-content">
                        <div
                          v-if="message.type === 'ai'"
                          class="message-header"
                        >
                          <i class="bi bi-robot me-2"></i>AI Assistant
                        </div>
                        <div v-else class="message-header">
                          <i class="bi bi-person me-2"></i>You
                        </div>
                        <div
                          class="message-text"
                          v-html="message.content"
                        ></div>
                      </div>
                    </div>
                  </div>

                  <!-- Input Area -->
                  <div class="chat-input">
                    <div class="input-group">
                      <input
                        type="text"
                        class="form-control"
                        v-model="userInput"
                        placeholder="Ask a math question..."
                        @keyup.enter="sendMessage"
                        :disabled="loading"
                      />
                      <button
                        class="btn btn-primary"
                        @click="sendMessage"
                        :disabled="loading || !userInput"
                      >
                        <span v-if="loading">
                          <span
                            class="spinner-border spinner-border-sm"
                            role="status"
                            aria-hidden="true"
                          ></span>
                          Sending...
                        </span>
                        <span v-else>Send</span>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="row mt-4">
          <div class="col-12">
            <h3>Quick Actions</h3>
            <div class="row">
              <div
                class="col-md-4 mb-3"
                v-for="(action, index) in quickActions"
                :key="index"
              >
                <div class="card">
                  <div class="card-body">
                    <h5 class="card-title">{{ action.title }}</h5>
                    <p class="card-text">{{ action.description }}</p>
                    <button
                      class="btn btn-outline-primary"
                      @click="useQuickAction(action)"
                    >
                      Use
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
  </div>
</template>

<script>
import { mapActions } from "vuex";
import Sidebar from "@/components/Home/Sidebar.vue";

export default {
  name: "AIHelp",
  components: {
    Sidebar,
  },
  data() {
    return {
      userInput: "",
      loading: false,
      messages: [
        {
          type: "ai",
          content:
            "Hello! I'm your AI Math Assistant. How can I help you today?",
        },
      ],
      quickActions: [
        {
          title: "Explain a Concept",
          description: "Get a detailed explanation of any math concept",
        },
        {
          title: "Solve a Problem",
          description: "Get step-by-step solution to a math problem",
        },
        {
          title: "Practice Problems",
          description: "Get personalized practice problems",
        },
      ],
    };
  },
  methods: {
    ...mapActions("math", ["getAIResponse"]),
    async sendMessage() {
      if (!this.userInput.trim() || this.loading) return;

      const userMessage = this.userInput;
      this.messages.push({
        type: "user",
        content: userMessage,
      });
      this.userInput = "";
      this.loading = true;

      try {
        const response = await this.getAIResponse(userMessage);
        this.messages.push({
          type: "ai",
          content: response,
        });
      } catch (error) {
        this.messages.push({
          type: "ai",
          content: "Sorry, I encountered an error. Please try again.",
        });
      } finally {
        this.loading = false;
        this.$nextTick(() => {
          this.scrollToBottom();
        });
      }
    },
    useQuickAction(action) {
      console.log("Using quick action:", action);
    },
    scrollToBottom() {
      const container = this.$refs.chatMessages;
      container.scrollTop = container.scrollHeight;
    },
  },
  mounted() {
    this.scrollToBottom();
  },
};
</script>

<style scoped>
.ai-help-container {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.chat-container {
  display: flex;
  flex-direction: column;
  height: 500px;
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
  background-color: #f8f9fa;
  border-radius: 0.25rem;
  margin-bottom: 1rem;
}

.message {
  margin-bottom: 1rem;
  max-width: 80%;
}

.message.user {
  margin-left: auto;
}

.message-content {
  padding: 0.75rem;
  border-radius: 0.5rem;
}

.message.ai .message-content {
  background-color: #e9ecef;
}

.message.user .message-content {
  background-color: #007bff;
  color: white;
}

.message-header {
  font-size: 0.8rem;
  margin-bottom: 0.5rem;
  opacity: 0.8;
}

.chat-input {
  padding: 1rem;
  background-color: white;
  border-radius: 0.25rem;
  box-shadow: 0 -2px 4px rgba(0, 0, 0, 0.1);
}

.card {
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-5px);
}
</style>
