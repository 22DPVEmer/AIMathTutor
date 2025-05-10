using System.Text.Json;

namespace MathTutor.Application.Interfaces
{
    /// <summary>
    /// Service for handling JSON serialization and deserialization
    /// </summary>
    public interface IJsonService
    {
        /// <summary>
        /// Gets the JSON serializer options
        /// </summary>
        JsonSerializerOptions JsonOptions { get; }

        /// <summary>
        /// Serializes an object to a JSON string
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="value">The object to serialize</param>
        /// <returns>The JSON string</returns>
        string Serialize<T>(T value);

        /// <summary>
        /// Deserializes a JSON string to an object
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize to</typeparam>
        /// <param name="json">The JSON string</param>
        /// <returns>The deserialized object</returns>
        T Deserialize<T>(string json);

        /// <summary>
        /// Checks if a string is valid JSON
        /// </summary>
        /// <param name="json">The string to check</param>
        /// <returns>True if the string is valid JSON, false otherwise</returns>
        bool IsValidJson(string json);

        /// <summary>
        /// Checks if a JSON string has all the required properties
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <param name="propertyNames">The names of the required properties</param>
        /// <returns>True if the JSON string has all the required properties, false otherwise</returns>
        bool HasRequiredProperties(string json, params string[] propertyNames);

        /// <summary>
        /// Creates a structured JSON response from a potentially incomplete JSON string
        /// </summary>
        /// <param name="json">The JSON string</param>
        /// <returns>A structured JSON response</returns>
        string CreateStructuredResponse(string json);

        /// <summary>
        /// Escapes a string for use in JSON
        /// </summary>
        /// <param name="str">The string to escape</param>
        /// <returns>The escaped string</returns>
        string EscapeJsonString(string str);
    }
}
