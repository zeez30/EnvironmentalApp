using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentalApp.Data
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Air, Water, Weather
        public string Status { get; set; } // Online, Offline, Malfunctioning
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LastReadingValue { get; set; }
        public DateTime LastReadingTimestamp { get; set; }
        public bool IsAnomalous { get; set; }
        public string AnomalyReason { get; set; }
    }
}
