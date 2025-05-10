using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
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
        private readonly ILogger<KernelProvider> _logger;

        /// <summary>
        /// Gets the Semantic Kernel instance
        /// </summary>
        public Kernel Kernel { get; }

        /// <summary>
        /// Initializes a new instance of the KernelProvider class
        /// </summary>
        /// <param name="kernel">The Semantic Kernel instance</param>
        /// <param name="logger">The logger</param>
        public KernelProvider(Kernel kernel, ILogger<KernelProvider> logger)
        {
            Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _logger.LogInformation("KernelProvider initialized with Semantic Kernel");
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
                _logger.LogDebug("Sending prompt to AI model: {Prompt}", prompt);
                
                var arguments = new KernelArguments();
                
                if (temperature.HasValue)
                {
                    arguments["Temperature"] = temperature.Value;
                }
                
                if (maxTokens.HasValue)
                {
                    arguments["MaxTokens"] = maxTokens.Value;
                }
                
                var result = await Kernel.InvokePromptAsync(prompt, arguments);
                var response = result.GetValue<string>() ?? string.Empty;
                
                _logger.LogDebug("Received response from AI model: {Response}", response);
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invoking prompt: {Message}", ex.Message);
                throw;
            }
        }
    }
}
