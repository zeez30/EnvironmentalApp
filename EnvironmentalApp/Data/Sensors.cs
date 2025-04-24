// File: EnvironmentalApp/Data/Sensor.cs
using System;

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

        // --- New Properties for Maintenance ---
        public DateTime? NextMaintenanceDate { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public string MaintenanceNotes { get; set; } // Optional notes for the next/last task
        // --- End of New Properties ---
    }
}