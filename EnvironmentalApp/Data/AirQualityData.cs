//AirQualityData.cs
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents air quality data readings.
    /// </summary>
    public class AirQualityData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the air quality data record.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the air quality reading.
        /// </summary>
        public DateTime DateTime { get; set; } // Combine Date and Time into one DateTime

        /// <summary>
        /// Gets or sets the concentration of Nitrogen Dioxide in the air.
        /// </summary>
        public double NitrogenDioxide { get; set; }

        /// <summary>
        /// Gets or sets the concentration of Sulphur Dioxide in the air.
        /// </summary>
        public double SulphurDioxide { get; set; }

        /// <summary>
        /// Gets or sets the concentration of particulate matter with a diameter of 2.5 micrometers or less (PM2.5).
        /// </summary>
        public double PM2_5 { get; set; }

        /// <summary>
        /// Gets or sets the concentration of particulate matter with a diameter of 10 micrometers or less (PM10).
        /// </summary>
        public double PM10 { get; set; }
    }
}