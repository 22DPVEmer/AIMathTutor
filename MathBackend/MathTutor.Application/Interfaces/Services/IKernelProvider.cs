using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace MathTutor.Application.Interfaces
{
    /// <summary>
    /// Provides access to the Semantic Kernel instance
    /// </summary>
    public interface IKernelProvider
    {
        /// <summary>
        /// Gets the Semantic Kernel instance
        /// </summary>
        Kernel Kernel { get; }

        /// <summary>
        /// Invokes a prompt with the Semantic Kernel
        /// </summary>
        /// <param name="prompt">The prompt to send</param>
        /// <param name="temperature">Optional temperature parameter</param>
        /// <param name="maxTokens">Optional max tokens parameter</param>
        /// <returns>The response from the AI model</returns>
        Task<string> InvokePromptAsync(string prompt, double? temperature = null, int? maxTokens = null);
    }
}
