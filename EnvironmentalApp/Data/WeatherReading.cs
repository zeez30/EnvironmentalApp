namespace EnvironmentalApp.Data
{
    public class WeatherReading
    {
        public DateTime Timestamp { get; set; }
        public double? Temperature { get; set; }
        public double? Humidity { get; set; }
        public double? WindSpeed { get; set; }
        public double? WindDirection { get; set; }
        public double Latitude { get; set; } // Added for context
        public double Longitude { get; set; } // Added for context
    }
}