// --- START OF FILE FirmwareStatusToColorConverter.cs ---

using System; // Required for IValueConverter, Type, Object, CultureInfo
using System.Globalization; // Required for CultureInfo
using Microsoft.Maui.Controls; // Required for IValueConverter
using Microsoft.Maui.Graphics; // Required for Colors

namespace EnvironmentalApp.Converters
{
    /// <summary>
    /// Converts a firmware update status string (e.g., "Updating...", "Up to date", "Failed")
    /// into a corresponding <see cref="Color"/>.
    /// Used in the Sensor Management page to visually indicate the firmware status.
    /// </summary>
    public class FirmwareStatusToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a firmware status string to a specific color.
        /// </summary>
        /// <param name="value">The firmware status string (e.g., from Sensor.FirmwareUpdateStatus).</param>
        /// <param name="targetType">The type of the binding target property (expected to be Color).</param>
        /// <param name="parameter">An optional converter parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>A <see cref="Color"/> representing the status (e.g., Orange for "Updating...", Green for "Up to date"), or Black as a default.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                // Use case-insensitive comparison for robustness
                switch (status.ToLowerInvariant().Trim())
                {
                    case "updating...":
                        return Colors.Orange; // Status indicating update is in progress
                    case "update available":
                        return Colors.DodgerBlue; // Status indicating an update can be initiated
                    case "up to date":
                    case "no update needed": // Treat "no update needed" similarly to "up to date" visually
                        return Colors.Green; // Status indicating firmware is current
                    case "update failed":
                        return Colors.Red; // Status indicating the last update attempt failed
                    case "idle":
                        return Colors.Gray; // Status indicating no current activity or check performed
                    default:
                        // Default color for unknown or unhandled statuses
                        System.Diagnostics.Debug.WriteLine($"Warning: Unknown firmware status '{status}' encountered in FirmwareStatusToColorConverter.");
                        return Colors.Black;
                }
            }
            // Return default color if the value is not a string or is null
            return Colors.Black;
        }

        /// <summary>
        /// Converting a color back to a firmware status string is not supported.
        /// Throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value">The color value.</param>
        /// <param name="targetType">The type to convert to (string).</param>
        /// <param name="parameter">An optional converter parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Throws NotImplementedException.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Cannot convert Color back to Firmware Status string.");
        }
    }
}
// --- END OF FILE FirmwareStatusToColorConverter.cs ---