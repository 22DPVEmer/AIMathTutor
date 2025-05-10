using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MathTutor.Application.Services
{
    /// <summary>
    /// Service for handling JSON serialization and deserialization
    /// </summary>
    public class JsonService : IJsonService
    {
        private readonly ILogger<JsonService> _logger;

        /// <summary>
        /// Gets the JSON serializer options
        /// </summary>
        public JsonSerializerOptions JsonOptions { get; }

        /// <summary>
        /// Initializes a new instance of the JsonService class
        /// </summary>
        /// <param name="logger">The logger</param>
        public JsonService(ILogger<JsonService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            JsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
        }

        /// <summary>
        /// Serializes an object to a JSON string
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="value">The object to serialize</param>
        /// <returns>The JSON string</returns>
        public string Serialize<T>(T value)
        {
            try
            {
                return JsonSerializer.Serialize(value, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serializing object to JSON");
                throw;
            }
        }

        /// <summary>
        /// Deserializes a JSON string to an object
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize to</typeparam>
        /// <param name="json">The JSON string</param>
        /// <returns>The deserialized object</returns>
        public T Deserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing JSON to object");
                throw;
            }
        }

        /// <summary>
        /// Checks if a string is valid JSON
        /// </summary>
        /// <param name="json">The string to check</param>
        /// <returns>True if the string is valid JSON, false otherwise</returns>
        public bool IsValidJson(string json)
        {
            try
            {
                JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a JSON string has all the required properties
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <param name="propertyNames">The names of the required properties</param>
        /// <returns>True if the JSON string has all the required properties, false otherwise</returns>
        public bool HasRequiredProperties(string json, params string[] propertyNames)
        {
            try
            {
                var jsonObject = JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);
                return propertyNames.All(prop => jsonObject.TryGetProperty(prop, out _));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a structured JSON response from a potentially incomplete JSON string
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>A structured JSON response</returns>
        public string CreateStructuredResponse(string json)
        {
            try
            {
                var jsonObject = JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);

                string statement = string.Empty;
                string solution = string.Empty;
                string explanation = string.Empty;

                if (jsonObject.TryGetProperty("statement", out var statementElement))
                {
                    statement = statementElement.GetString() ?? string.Empty;
                }

                if (jsonObject.TryGetProperty("solution", out var solutionElement))
                {
                    solution = solutionElement.GetString() ?? string.Empty;
                }

                if (jsonObject.TryGetProperty("explanation", out var explanationElement))
                {
                    explanation = explanationElement.GetString() ?? string.Empty;
                }

                return $@"{{
                    ""statement"": ""{EscapeJsonString(statement)}"",
                    ""solution"": ""{EscapeJsonString(solution)}"",
                    ""explanation"": ""{EscapeJsonString(explanation)}""
                }}";
            }
            catch
            {
                return $@"{{
                    ""statement"": ""Invalid problem format"",
                    ""solution"": ""N/A"",
                    ""explanation"": ""The system could not generate a valid problem.""
                }}";
            }
        }

        /// <summary>
        /// Escapes a string for use in JSON
        /// </summary>
        /// <param name="str">The string to escape</param>
        /// <returns>The escaped string</returns>
        public string EscapeJsonString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            // Use the built-in JSON serializer to properly escape the string
            // This is more reliable than manual regex replacement
            return JsonSerializer.Serialize(str).Trim('"');
        }
    }
}
