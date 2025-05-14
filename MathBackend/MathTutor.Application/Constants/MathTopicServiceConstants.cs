using System;

namespace MathTutor.Application.Constants
{
    /// <summary>
    /// Constants used in the MathTopicService
    /// </summary>
    public static class MathTopicServiceConstants
    {
        /// <summary>
        /// Constants related to statement normalization
        /// </summary>
        public static class StatementNormalization
        {
            /// <summary>
            /// Characters to remove when normalizing statements
            /// </summary>
            public const string WhitespaceCharacter = " ";
            
            /// <summary>
            /// Empty replacement string
            /// </summary>
            public const string EmptyReplacement = "";
            
            /// <summary>
            /// Maximum length of statement to log for debugging
            /// </summary>
            public const int MaxStatementLogLength = 20;
        }

        /// <summary>
        /// Constants related to percentage calculation
        /// </summary>
        public static class PercentageCalculation
        {
            /// <summary>
            /// Multiplier to convert decimal to percentage
            /// </summary>
            public const int PercentageMultiplier = 100;
            
            /// <summary>
            /// Default percentage value when no problems exist
            /// </summary>
            public const int DefaultPercentage = 0;
        }
    }
}
