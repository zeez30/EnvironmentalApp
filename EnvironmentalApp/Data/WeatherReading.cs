// --- START OF FILE WeatherReading.cs ---

using System; // Required for DateTime

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents a single historical reading of weather parameters at a specific time and location.
    /// Used in the Data Analysis feature.
    /// </summary>
    public class WeatherReading
    {
        /// <summary>
        /// Gets or sets the exact date and time when the reading was taken (often in UTC).
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the measured temperature, typically in degrees Celsius (°C). Null if data is unavailable.
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// Gets or sets the measured relative humidity, typically as a percentage (%). Null if data is unavailable.
        /// </summary>
        public double? Humidity { get; set; }

        /// <summary>
        /// Gets or sets the measured wind speed, typically in meters per second (m/s). Null if data is unavailable.
        /// </summary>
        public double? WindSpeed { get; set; }

        /// <summary>
        /// Gets or sets the measured wind direction, typically in degrees (°), where 0°/360° is North, 90° is East, etc. Null if data is unavailable.
        /// </summary>
        public double? WindDirection { get; set; }

        /// <summary>
        /// Gets or sets the geographical latitude where the weather reading was taken. Added for context.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the geographical longitude where the weather reading was taken. Added for context.
        /// </summary>
        public double Longitude { get; set; }
    }
}
// --- END OF FILE WeatherReading.cs ---