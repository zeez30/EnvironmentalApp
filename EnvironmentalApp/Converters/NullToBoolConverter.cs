// --- START OF FILE NullToBoolConverter.cs ---

using System; // Required for IValueConverter, Type, Object, CultureInfo, Convert
using System.Globalization; // Required for CultureInfo
using Microsoft.Maui.Controls; // Required for IValueConverter

namespace EnvironmentalApp.Converters
{
    /// <summary>
    /// Converts a null or non-null object value to a boolean.
    /// By default, null converts to false and non-null converts to true.
    /// An optional boolean converter parameter can invert this logic (e.g., parameter=true means null converts to true).
    /// Useful for controlling UI element properties like IsEnabled based on whether an object property is set.
    /// Example: Enable "Mark Maintenance Complete" button only if NextMaintenanceDate is NOT null. (Use parameter=false or no parameter).
    /// Example: Show "Not Scheduled" text only if NextMaintenanceDate IS null. (Use parameter=true).
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Converts an object value to a boolean based on whether it is null.
        /// </summary>
        /// <param name="value">The object value to check for nullity.</param>
        /// <param name="targetType">The type of the binding target property (expected to be boolean).</param>
        /// <param name="parameter">Optional parameter. If 'true' or 'True', null converts to true. Otherwise, null converts to false (default).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>A boolean value based on the nullity of the input and the optional parameter.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Determine the inversion behavior based on the parameter
            bool invert = false; // Default: null -> false, non-null -> true
            if (parameter != null)
            {
                // Try parsing parameter as bool, case-insensitive
                if (bool.TryParse(parameter.ToString(), out bool parameterBool))
                {
                    invert = parameterBool; // If parameter is explicitly true, invert the logic
                }
                // Handle common string representations if bool.TryParse fails
                else if (parameter.ToString().Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    invert = true;
                }
                else if (parameter.ToString().Trim().Equals("invert", StringComparison.OrdinalIgnoreCase)) // Allow "invert" as synonym for true
                {
                    invert = true;
                }
            }


            // Perform the conversion based on inversion flag
            if (invert)
            {
                // Inverted logic: null -> true, non-null -> false
                return value == null;
            }
            else
            {
                // Default logic: null -> false, non-null -> true
                return value != null;
            }
        }

        /// <summary>
        /// Converting a boolean back to null/non-null based on the original value is not typically supported.
        /// Throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <param name="targetType">The type to convert to (object).</param>
        /// <param name="parameter">An optional converter parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Throws NotImplementedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Cannot convert boolean back based on nullity.");
        }
    }
}
// --- END OF FILE NullToBoolConverter.cs ---