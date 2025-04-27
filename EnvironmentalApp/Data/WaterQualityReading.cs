// --- START OF FILE WaterQualityReading.cs ---

using System; // Required for DateTime

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents a single historical reading of water quality parameters at a specific time and site.
    /// Used in the Data Analysis feature.
    /// </summary>
    public class WaterQualityReading
    {
        /// <summary>
        /// Gets or sets the exact date and time when the reading was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the measured Nitrate level, typically in mg/l. Null if data is unavailable.
        /// </summary>
        public double? Nitrate { get; set; }

        /// <summary>
        /// Gets or sets the measured Nitrite level, typically in mg/l. Null if data is unavailable.
        /// </summary>
        public double? Nitrite { get; set; }

        /// <summary>
        /// Gets or sets the measured Phosphate level, typically in mg/l. Null if data is unavailable.
        /// </summary>
        public double? Phosphate { get; set; }

        /// <summary>
        /// Gets or sets the measured level of E. coli (or potentially Electrical Conductivity, depending on context/unit),
        /// typically in cfu/100ml for E. coli. Null if data is unavailable.
        /// Note: Original file context suggests E. coli based on unit cfu/100ml.
        /// </summary>
        public double? EC { get; set; } // Assuming E. coli based on unit cfu/100ml in sample data

        /// <summary>
        /// Gets or sets the name or identifier of the monitoring site where the reading was taken. Added for context in data analysis.
        /// </summary>
        public string SiteName { get; set; }
    }
}
// --- END OF FILE WaterQualityReading.cs ---