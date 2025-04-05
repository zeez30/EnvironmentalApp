// Sensor.cs
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string OperationalStatus { get; set; }
        public DateTime? LastMaintenanceDate { get; set; } // Nullable DateTime
        public string Description { get; set; }
        public string SensorType { get; set; }
    }
}