// --- START OF FILE ComparisonConverter.cs ---

using System; // Required for IValueConverter, Type, Object, CultureInfo, Enum
using System.Globalization; // Required for CultureInfo
using Microsoft.Maui.Controls; // Required for IValueConverter

namespace EnvironmentalApp.Converters
{
    /// <summary>
    /// Defines the types of comparison operations supported by the <see cref="ComparisonConverter"/>.
    /// </summary>
    public enum ComparisonOperation
    {
        /// <summary>Checks if the bound value equals the converter parameter.</summary>
        Equals,
        /// <summary>Checks if the bound value does not equal the converter parameter.</summary>
        NotEquals
        // Future: Add LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual if needed
    }

    /// <summary>
    /// Performs a comparison between the bound value and the converter parameter based on the specified <see cref="Operation"/>.
    /// Returns a boolean result. Useful for controlling UI element states (e.g., IsEnabled) based on data comparisons.
    /// Example: Enable a button only if CurrentFirmwareVersion is not equal to LatestFirmwareVersion.
    /// </summary>
    public class ComparisonConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the comparison operation to perform (e.g., Equals, NotEquals). Defaults to NotEquals.
        /// </summary>
        public ComparisonOperation Operation { get; set; } = ComparisonOperation.NotEquals; // Default operation

        /// <summary>
        /// Converts the bound value by comparing it to the converter parameter using the specified operation.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property (expected to be boolean).</param>
        /// <param name="parameter">The converter parameter to compare against the bound value.</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>True or false based on the result of the comparison operation.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Compare 'value' (from binding) with 'parameter' (from ConverterParameter in XAML)
            switch (Operation)
            {
                case ComparisonOperation.Equals:
                    // Use object.Equals for safe comparison, handles nulls appropriately
                    return object.Equals(value, parameter);

                case ComparisonOperation.NotEquals:
                default: // Default to NotEquals, common for enabling buttons when values differ
                    return !object.Equals(value, parameter);

                    // Future: Implement other operations if added to the enum
                    // case ComparisonOperation.GreaterThan:
                    //     // Requires IComparable check and careful null handling
                    //     if (value is IComparable compValue && parameter is IComparable compParam) {
                    //         try { return compValue.CompareTo(compParam) > 0; } catch { return false; }
                    //     }
                    //     return false; // Cannot compare
            }
        }

        /// <summary>
        /// Converting back from a boolean comparison result to the original value is not typically meaningful or supported.
        /// Throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Throws NotImplementedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Conversion back is generally not needed or possible for comparison results
            throw new NotImplementedException("ComparisonConverter cannot convert back.");
        }
    }
}
// --- END OF FILE ComparisonConverter.cs ---