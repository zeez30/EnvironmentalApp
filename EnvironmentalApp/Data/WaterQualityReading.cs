namespace EnvironmentalApp.Data
{
    public class WaterQualityReading
    {
        public DateTime Timestamp { get; set; }
        public double? Nitrate { get; set; }
        public double? Nitrite { get; set; }
        public double? Phosphate { get; set; }
        public double? EC { get; set; } // Note: This might be daily, handle accordingly
        public string SiteName { get; set; } // Added for context
    }
}