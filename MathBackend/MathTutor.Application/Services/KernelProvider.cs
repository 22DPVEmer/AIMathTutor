using Microsoft.SemanticKernel;
using MathTutor.Application.Constants;
using MathTutor.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Implementation of IKernelProvider that provides access to the Semantic Kernel
    /// </summary>
    public class KernelProvider : IKernelProvider
    {
        /// <summary>
        /// Gets the Semantic Kernel instance
        /// </summary>
        public Kernel Kernel { get; }

        /// <summary>
        /// Initializes a new instance of the KernelProvider class
        /// </summary>
        /// <param name="kernel">The Semantic Kernel instance</param>
        public KernelProvider(Kernel kernel)
        {
            Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        /// <summary>
        /// Invokes a prompt with the Semantic Kernel
        /// </summary>
        /// <param name="prompt">The prompt to send</param>
        /// <param name="temperature">Optional temperature parameter</param>
        /// <param name="maxTokens">Optional max tokens parameter</param>
        /// <returns>The response from the AI model</returns>
        public async Task<string> InvokePromptAsync(string prompt, double? temperature = null, int? maxTokens = null)
        {
            try
            {
                var arguments = new KernelArguments();

                if (temperature.HasValue)
                {
                    arguments[KernelProviderConstants.TemperatureParameter] = temperature.Value;
                }

                if (maxTokens.HasValue)
                {
                    arguments[KernelProviderConstants.MaxTokensParameter] = maxTokens.Value;
                }

                var result = await Kernel.InvokePromptAsync(prompt, arguments);
                var response = result.GetValue<string>() ?? string.Empty;

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
