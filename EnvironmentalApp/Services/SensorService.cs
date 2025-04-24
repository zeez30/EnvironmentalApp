// File: EnvironmentalApp/Services/SensorService.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using EnvironmentalApp.Data;

namespace EnvironmentalApp.Services
{
    public class SensorService
    {
        // Keep the mock data in memory for demonstration
        private static ObservableCollection<Sensor> _mockSensors;

        public SensorService()
        {
            // Initialize mock data only if it hasn't been already
            if (_mockSensors == null)
            {
                _mockSensors = CreateInitialMockData();
            }
        }

        private ObservableCollection<Sensor> CreateInitialMockData()
        {
            // Create some sample sensor data including maintenance info
            return new ObservableCollection<Sensor>()
            {
                new Sensor
                {
                    Id = 1,
                    Name = "Temperature Sensor 1",
                    Type = "Weather",
                    Status = "Online",
                    Latitude = 55.9533,
                    Longitude = -3.1883,
                    LastReadingValue = "15.5°C",
                    LastReadingTimestamp = DateTime.Now.AddMinutes(-5),
                    NextMaintenanceDate = DateTime.Now.AddDays(30), // Scheduled in 30 days
                    LastMaintenanceDate = DateTime.Now.AddMonths(-5)
                },
                new Sensor
                {
                    Id = 2,
                    Name = "Air Quality Monitor - City Center",
                    Type = "Air",
                    Status = "Online",
                    Latitude = 55.9500,
                    Longitude = -3.1900,
                    LastReadingValue = "Good",
                    LastReadingTimestamp = DateTime.Now.AddMinutes(-10),
                    NextMaintenanceDate = DateTime.Now.AddDays(-2), // Overdue!
                    LastMaintenanceDate = DateTime.Now.AddMonths(-2),
                    MaintenanceNotes = "Filter replacement needed."
                },
                new Sensor
                {
                    Id = 3,
                    Name = "Water Flow Sensor - River Esk",
                    Type = "Water",
                    Status = "Offline", // Sensor is offline, maintenance might be needed
                    Latitude = 55.9200,
                    Longitude = -3.1500,
                    LastReadingValue = "N/A",
                    LastReadingTimestamp = DateTime.Now.AddHours(-1),
                    NextMaintenanceDate = null, // Not scheduled
                    LastMaintenanceDate = DateTime.Now.AddYears(-1)
                },
                new Sensor
                {
                    Id = 4,
                    Name = "Humidity Sensor - Botanical Garden",
                    Type = "Weather",
                    Status = "Online",
                    Latitude = 55.9600,
                    Longitude = -3.1950,
                    LastReadingValue = "78%",
                    LastReadingTimestamp = DateTime.Now.AddMinutes(-2),
                    NextMaintenanceDate = DateTime.Now.AddDays(90), // Scheduled in 90 days
                    LastMaintenanceDate = DateTime.Now.AddDays(-10) // Recently maintained
                }
            };
        }

        public ObservableCollection<Sensor> GetMockSensors()
        {
            // Return the current state of the mock data
            return _mockSensors;
        }

        // Method to simulate scheduling maintenance
        public bool ScheduleMaintenance(int sensorId, DateTime nextDate, string notes)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                sensor.NextMaintenanceDate = nextDate;
                sensor.MaintenanceNotes = notes;
                // In a real app, you'd save this to a database.
                // For ObservableCollection, the UI might update automatically if bound correctly,
                // but sometimes explicit notification is needed depending on how updates happen.
                // We might need to trigger a property change notification if Sensor implemented INotifyPropertyChanged.
                // For simplicity here, we assume the collection view will refresh or we reload data.
                return true;
            }
            return false;
        }

        // Method to simulate completing maintenance
        public bool MarkMaintenanceComplete(int sensorId, DateTime completionDate)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                sensor.LastMaintenanceDate = completionDate;
                sensor.NextMaintenanceDate = null; // Clear scheduled date, or calculate next based on interval
                sensor.MaintenanceNotes = "Maintenance completed on " + completionDate.ToString("yyyy-MM-dd");
                // Again, persistence and notification handling would be needed in a real app.
                return true;
            }
            return false;
        }
    }
}