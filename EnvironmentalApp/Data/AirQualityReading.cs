namespace EnvironmentalApp.Data
{
    public class AirQualityReading
    {
        public DateTime Timestamp { get; set; }
        public double? NitrogenDioxide { get; set; }
        public double? SulphurDioxide { get; set; }
        public double? PM25 { get; set; }
        public double? PM10 { get; set; }
        public string SiteName { get; set; } // Added for context
    }
}