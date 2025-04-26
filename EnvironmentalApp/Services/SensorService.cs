// File: EnvironmentalApp/Services/SensorService.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks; // Add for Task.Delay
using EnvironmentalApp.Data;

namespace EnvironmentalApp.Services
{
    public class SensorService
    {
        private static ObservableCollection<Sensor> _mockSensors;

        public SensorService()
        {
            if (_mockSensors == null)
            {
                _mockSensors = CreateInitialMockData();
            }
        }

        private ObservableCollection<Sensor> CreateInitialMockData()
        {
            return new ObservableCollection<Sensor>()
            {
                new Sensor // Weather Sensor - up to date firmware
                {
                    Id = 1, Name = "Temperature Sensor 1", Type = "Weather", Status = "Online",
                    Latitude = 55.9533, Longitude = -3.1883, LastReadingValue = "15.5°C", LastReadingTimestamp = DateTime.Now.AddMinutes(-5),
                    NextMaintenanceDate = DateTime.Now.AddDays(30), LastMaintenanceDate = DateTime.Now.AddMonths(-5),
                    SamplingIntervalSeconds = 600, IsEnabled = true, // Config examples
                    CurrentFirmwareVersion = "2.1.0", LatestFirmwareVersion = "2.1.0", FirmwareUpdateStatus = "Up to date" // Firmware examples
                },
                new Sensor // Air Sensor - Needs firmware update
                {
                    Id = 2, Name = "Air Quality Monitor - City Center", Type = "Air", Status = "Online",
                    Latitude = 55.9500, Longitude = -3.1900, LastReadingValue = "Good", LastReadingTimestamp = DateTime.Now.AddMinutes(-10),
                    NextMaintenanceDate = DateTime.Now.AddDays(-2), LastMaintenanceDate = DateTime.Now.AddMonths(-2), MaintenanceNotes = "Filter replacement needed.",
                    SamplingIntervalSeconds = 300, ReportingThreshold = 150.0, IsEnabled = true,
                    CurrentFirmwareVersion = "1.9.5", LatestFirmwareVersion = "2.1.0", FirmwareUpdateStatus = "Update Available"
                },
                new Sensor // Water Sensor - Offline, disabled
                {
                    Id = 3, Name = "Water Flow Sensor - River Esk", Type = "Water", Status = "Offline",
                    Latitude = 55.9200, Longitude = -3.1500, LastReadingValue = "N/A", LastReadingTimestamp = DateTime.Now.AddHours(-1),
                    NextMaintenanceDate = null, LastMaintenanceDate = DateTime.Now.AddYears(-1),
                    SamplingIntervalSeconds = 1800, IsEnabled = false, // Disabled
                    CurrentFirmwareVersion = "1.0.1", LatestFirmwareVersion = "1.0.1", FirmwareUpdateStatus = "Up to date"
                },
                 new Sensor // Weather Sensor - Different config, up to date
                {
                    Id = 4, Name = "Humidity Sensor - Botanical Garden", Type = "Weather", Status = "Online",
                    Latitude = 55.9600, Longitude = -3.1950, LastReadingValue = "78%", LastReadingTimestamp = DateTime.Now.AddMinutes(-2),
                    NextMaintenanceDate = DateTime.Now.AddDays(90), LastMaintenanceDate = DateTime.Now.AddDays(-10),
                    SamplingIntervalSeconds = 900, IsEnabled = true,
                    CurrentFirmwareVersion = "3.0.0", LatestFirmwareVersion = "3.0.0", FirmwareUpdateStatus = "Up to date"
                }
            };
        }

        public ObservableCollection<Sensor> GetMockSensors()
        {
            return _mockSensors;
        }

        public bool ScheduleMaintenance(int sensorId, DateTime nextDate, string notes)
        {
            // ... (previous implementation) ...
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                sensor.NextMaintenanceDate = nextDate;
                sensor.MaintenanceNotes = notes;
                return true;
            }
            return false;
        }

        public bool MarkMaintenanceComplete(int sensorId, DateTime completionDate)
        {
            // ... (previous implementation) ...
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                sensor.LastMaintenanceDate = completionDate;
                sensor.NextMaintenanceDate = null;
                sensor.MaintenanceNotes = "Maintenance completed on " + completionDate.ToString("yyyy-MM-dd");
                return true;
            }
            return false;
        }

        // --- New Methods for Config/Firmware ---

        public bool UpdateSensorConfiguration(int sensorId, int newInterval, double? newThreshold, bool newIsEnabled)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                sensor.SamplingIntervalSeconds = newInterval;
                sensor.ReportingThreshold = newThreshold; // Can be null
                sensor.IsEnabled = newIsEnabled;
                // Simulate potential status change based on IsEnabled
                if (!newIsEnabled && sensor.Status == "Online")
                {
                    sensor.Status = "Offline (Disabled)";
                }
                else if (newIsEnabled && sensor.Status == "Offline (Disabled)")
                {
                    sensor.Status = "Online"; // Simulate coming back online
                }
                return true;
            }
            return false;
        }

        public async Task<bool> InitiateFirmwareUpdate(int sensorId)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null && sensor.CurrentFirmwareVersion != sensor.LatestFirmwareVersion)
            {
                sensor.FirmwareUpdateStatus = "Updating...";
                // Simulate update process delay
                await Task.Delay(TimeSpan.FromSeconds(5)); // Wait 5 seconds

                // Simulate outcome (e.g., success)
                sensor.CurrentFirmwareVersion = sensor.LatestFirmwareVersion;
                sensor.FirmwareUpdateStatus = "Up to date";
                // sensor.FirmwareUpdateStatus = "Update Failed"; // Or simulate failure
                return true; // Update initiated (and completed in this simulation)
            }
            if (sensor != null)
            {
                sensor.FirmwareUpdateStatus = "No update needed"; // Status if already up-to-date
            }
            return false; // Update not needed or sensor not found
        }
    }
}