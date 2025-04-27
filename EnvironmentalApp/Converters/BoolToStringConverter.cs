// --- START OF FILE BoolToStringConverter.cs ---

using System; // Required for IValueConverter, Type, Object, CultureInfo
using System.Globalization; // Required for CultureInfo
using Microsoft.Maui.Controls; // Required for IValueConverter

namespace EnvironmentalApp.Converters
{
    /// <summary>
    /// Converts a boolean value to a custom string representation (e.g., True -> "Yes", False -> "No").
    /// Useful for displaying boolean properties in a more user-friendly format in the UI.
    /// Allows customization of the True and False string values via properties.
    /// </summary>
    public class BoolToStringConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the string representation for a True value. Defaults to "True".
        /// </summary>
        public string TrueString { get; set; } = "True";

        /// <summary>
        /// Gets or sets the string representation for a False value. Defaults to "False".
        /// </summary>
        public string FalseString { get; set; } = "False";

        /// <summary>
        /// Converts a boolean value to its corresponding string representation (TrueString or FalseString).
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The type of the binding target property (expected to be string).</param>
        /// <param name="parameter">An optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>The TrueString if value is true, FalseString if value is false, or an empty string if the input is not a boolean.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueString : FalseString;
            }
            // Return a default value if the input is not a boolean
            return string.Empty; // Or potentially FalseString or null depending on desired behavior
        }

        /// <summary>
        /// Converts a string representation back to a boolean value.
        /// Returns true if the string matches the TrueString (case-insensitive).
        /// </summary>
        /// <param name="value">The string value to convert back.</param>
        /// <param name="targetType">The type of the binding target property (expected to be boolean).</param>
        /// <param name="parameter">An optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>True if the input string equals TrueString (case-insensitive), false otherwise.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Perform case-insensitive comparison
                return stringValue.Equals(TrueString, StringComparison.OrdinalIgnoreCase);
            }
            // Default conversion back if input is not a string
            return false;
        }
    }
}
// --- END OF FILE BoolToStringConverter.cs ---