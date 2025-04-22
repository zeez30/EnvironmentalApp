// WeatherData.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents weather data readings.
    /// </summary>
    public class WeatherData
    {
        /// <summary>
        /// Gets or sets the unique identifier for the weather data record.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the weather reading. (Column A - Time)
        /// </summary>
        public DateTime Time { get; set; }          // Column A (Time)

        /// <summary>
        /// Gets or sets the temperature reading. (Column B - temperature_relative_hum)
        /// </summary>
        public double Temperature { get; set; }      // Column B (temperature_relative_hum)

        /// <summary>
        /// Gets or sets the relative humidity reading. (Column C - relative_hum)
        /// </summary>
        public double RelativeHumidity { get; set; } // Column C (relative_hum)

        /// <summary>
        /// Gets or sets the wind speed reading. (Column D - wind_speed)
        /// </summary>
        public double WindSpeed { get; set; }        // Column D (wind_speed)

        /// <summary>
        /// Gets or sets the wind direction reading at 10 meters. (Column E - wind_direction_10m)
        /// </summary>
        public double WindDirection { get; set; }    // Column E (wind_direction_10m)
    }
}