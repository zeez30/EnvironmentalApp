namespace EnvironmentalApp.Data
{
    public class WaterQualityReading
    {
        public DateTime Timestamp { get; set; }
        public double? Nitrate { get; set; }
        public double? Nitrite { get; set; }
        public double? Phosphate { get; set; }
        public double? EC { get; set; }
        public string SiteName { get; set; } // Added for context
    }
}