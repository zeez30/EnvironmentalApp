//MetaData.cs
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents metadata about environmental data measurements.
    /// </summary>
    public class MetaData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the metadata record.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the category of the environmental measurement. (Column A)
        /// </summary>
        public string Category { get; set; } // Column A

        /// <summary>
        /// Gets or sets the quantity being measured (e.g., "Nitrogen Dioxide"). (Column B)
        /// </summary>
        public string Quantity { get; set; } // Column B

        /// <summary>
        /// Gets or sets the symbol representing the quantity (e.g., "NO2"). (Column C)
        /// </summary>
        public string Symbol { get; set; }    // Column C

        /// <summary>
        /// Gets or sets the unit of measurement (e.g., "ppm"). (Column D)
        /// </summary>
        public string Unit { get; set; }      // Column D

        /// <summary>
        /// Gets or sets a more detailed description of the unit. (Column E)
        /// </summary>
        public string UnitDescription { get; set; } // Column E

        /// <summary>
        /// Gets or sets the frequency at which the measurement is taken. (Column F)
        /// </summary>
        public string MeasurementFrequency { get; set; } // Column F

        /// <summary>
        /// Gets or sets the safe level or threshold for the measurement, if applicable. (Column G)
        /// </summary>
        public double? SafeLevel { get; set; }    // Column G (Nullable double)

        /// <summary>
        /// Gets or sets a reference or source for this metadata. (Column H)
        /// </summary>
        public string Reference { get; set; }    // Column H

        /// <summary>
        /// Gets or sets the type or identifier of the sensor used for the measurement. (Column I)
        /// </summary>
        public string Sensor { get; set; }      // Column I

        /// <summary>
        /// Gets or sets a URL providing more information about the measurement or sensor. (Column J)
        /// </summary>
        public string URL { get; set; }          // Column J
    }
}