using Microsoft.SemanticKernel;
using MathTutor.Application.Constants;
using MathTutor.Application.Interfaces;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<KernelProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the KernelProvider class
        /// </summary>
        /// <param name="kernel">The Semantic Kernel instance</param>
        /// <param name="logger">The logger instance</param>
        public KernelProvider(Kernel kernel, ILogger<KernelProvider> logger)
        {
            Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _logger.LogDebug("Invoking prompt with temperature: {Temperature}, maxTokens: {MaxTokens}", temperature, maxTokens);

                var arguments = new KernelArguments();

                if (temperature.HasValue)
                {
                    arguments[KernelProviderConstants.TemperatureParameter] = temperature.Value;
                }

                if (maxTokens.HasValue)
                {
                    arguments[KernelProviderConstants.MaxTokensParameter] = maxTokens.Value;
                }

                _logger.LogDebug("Calling Kernel.InvokePromptAsync...");
                var result = await Kernel.InvokePromptAsync(prompt, arguments);
                var response = result.GetValue<string>() ?? string.Empty;

                _logger.LogDebug("Kernel response received, length: {Length}", response.Length);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking prompt with Semantic Kernel. Prompt length: {PromptLength}", prompt?.Length ?? 0);
                throw;
            }
        }
    }
}
