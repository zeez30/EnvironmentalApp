// --- START OF FILE AirQualityReading.cs ---

using System; // Required for DateTime

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents a single historical reading of air quality parameters at a specific time and site.
    /// Used in the Data Analysis feature.
    /// </summary>
    public class AirQualityReading
    {
        /// <summary>
        /// Gets or sets the exact date and time when the reading was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the measured Nitrogen Dioxide (NO2) level, typically in µg/m³. Null if data is unavailable.
        /// </summary>
        public double? NitrogenDioxide { get; set; }

        /// <summary>
        /// Gets or sets the measured Sulphur Dioxide (SO2) level, typically in µg/m³. Null if data is unavailable.
        /// </summary>
        public double? SulphurDioxide { get; set; }

        /// <summary>
        /// Gets or sets the measured Particulate Matter (PM2.5) level, typically in µg/m³. Null if data is unavailable.
        /// </summary>
        public double? PM25 { get; set; }

        /// <summary>
        /// Gets or sets the measured Particulate Matter (PM10) level, typically in µg/m³. Null if data is unavailable.
        /// </summary>
        public double? PM10 { get; set; }

        /// <summary>
        /// Gets or sets the name or identifier of the monitoring site where the reading was taken. Added for context in data analysis.
        /// </summary>
        public string SiteName { get; set; }
    }
}
// --- END OF FILE AirQualityReading.cs ---