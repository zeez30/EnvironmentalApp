// WeatherData.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    public class WeatherData
    {
        [Key]
        public int Id { get; set; }

        public DateTime Time { get; set; }         // Column A (Time)
        public double Temperature { get; set; }    // Column B (temperature_relative_hum)
        public double RelativeHumidity { get; set; } // Column C (relative_hum)
        public double WindSpeed { get; set; }       // Column D (wind_speed)
        public double WindDirection { get; set; }   // Column E (wind_direction_10m)
    }
}