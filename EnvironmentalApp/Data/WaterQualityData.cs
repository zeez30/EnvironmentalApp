// WaterQualityData.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents water quality data readings.
    /// </summary>
    public class WaterQualityData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the water quality data record.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date of the water quality reading. (Column A)
        /// </summary>
        public DateTime Date { get; set; }        // Column A

        /// <summary>
        /// Gets or sets the time of the water quality reading. (Column B)
        /// </summary>
        public TimeSpan Time { get; set; }        // Column B (adjust if Time is stored differently)

        /// <summary>
        /// Gets or sets the concentration of Nitrate in the water. (Column C)
        /// </summary>
        public double Nitrate { get; set; }      // Column C

        /// <summary>
        /// Gets or sets the concentration of Nitrite in the water. (Column D)
        /// </summary>
        public double Nitrite { get; set; }      // Column D

        /// <summary>
        /// Gets or sets the concentration of Phosphate in the water. (Column E)
        /// </summary>
        public double Phosphate { get; set; }    // Column E

        /// <summary>
        /// Gets or sets the Electrical Conductivity (EC) of the water. (Column F)
        /// </summary>
        public double EC { get; set; }