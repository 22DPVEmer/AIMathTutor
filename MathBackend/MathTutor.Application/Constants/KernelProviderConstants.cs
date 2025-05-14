namespace MathTutor.Application.Constants
{
    public static class KernelProviderConstants
    {
        // Parameter names
        public const string TemperatureParameter = "Temperature";
        public const string MaxTokensParameter = "MaxTokens";

        // Log messages
        public const string InitializationMessage = "KernelProvider initialized with Semantic Kernel";
        public const string SendingPromptMessage = "Sending prompt to AI model: {Prompt}";
        public const string ReceivedResponseMessage = "Received response from AI model: {Response}";
        public const string InvocationErrorMessage = "Error invoking prompt: {Message}";
    }
}
