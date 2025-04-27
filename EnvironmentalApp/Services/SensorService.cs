// --- START OF FILE SensorService.cs ---

// File: EnvironmentalApp/Services/SensorService.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks; // Required for Task.Delay in async methods
using EnvironmentalApp.Data; // Access to the Sensor data model

namespace EnvironmentalApp.Services
{
    /// <summary>
    /// Provides services for managing sensor data.
    /// In this version, it uses a static collection of mock sensor data for demonstration.
    /// Handles operations like retrieving sensors, scheduling maintenance, updating configuration, and initiating firmware updates.
    /// Relevant to Operations Manager (maintenance) and Administrator (config/firmware) roles.
    /// </summary>
    public class SensorService
    {
        // Static collection to hold mock sensor data, ensuring it persists across service instances.
        private static ObservableCollection<Sensor> _mockSensors;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorService"/> class.
        /// Creates the initial mock sensor data if it hasn't been created yet.
        /// </summary>
        public SensorService()
        {
            // Initialize mock data only once
            if (_mockSensors == null)
            {
                _mockSensors = CreateInitialMockData();
            }
        }

        /// <summary>
        /// Creates and returns the initial set of mock sensor data.
        /// Includes sensors with varying statuses, maintenance schedules, configurations, and firmware states
        /// to demonstrate different scenarios in the Sensor Management page.
        /// </summary>
        /// <returns>An ObservableCollection of <see cref="Sensor"/> objects.</returns>
        private ObservableCollection<Sensor> CreateInitialMockData()
        {
            return new ObservableCollection<Sensor>()
            {
                new Sensor // Example 1: Weather Sensor, firmware up-to-date
                {
                    Id = 1, Name = "Temperature Sensor Alpha", Type = "Weather", Status = "Online",
                    Latitude = 55.9533, Longitude = -3.1883, LastReadingValue = "15.5°C", LastReadingTimestamp = DateTime.Now.AddMinutes(-5),
                    NextMaintenanceDate = DateTime.Now.AddDays(30), LastMaintenanceDate = DateTime.Now.AddMonths(-5), MaintenanceNotes = "Routine check scheduled.",
                    SamplingIntervalSeconds = 600, ReportingThreshold = 25.0, IsEnabled = true, // Config details
                    CurrentFirmwareVersion = "2.1.0", LatestFirmwareVersion = "2.1.0", FirmwareUpdateStatus = "Up to date" // Firmware details
                },
                new Sensor // Example 2: Air Quality Sensor, needs firmware update, maintenance overdue
                {
                    Id = 2, Name = "Air Quality Monitor - City Center", Type = "Air", Status = "Online",
                    Latitude = 55.9500, Longitude = -3.1900, LastReadingValue = "AQI: 45 (Good)", LastReadingTimestamp = DateTime.Now.AddMinutes(-10),
                    NextMaintenanceDate = DateTime.Now.AddDays(-2), // Overdue
                    LastMaintenanceDate = DateTime.Now.AddMonths(-2), MaintenanceNotes = "Filter replacement needed urgently.",
                    SamplingIntervalSeconds = 300, ReportingThreshold = 150.0, IsEnabled = true,
                    CurrentFirmwareVersion = "1.9.5", LatestFirmwareVersion = "2.1.0", FirmwareUpdateStatus = "Update Available" // Needs update
                },
                new Sensor // Example 3: Water Sensor, offline, disabled, no maintenance scheduled
                {
                    Id = 3, Name = "Water Flow Sensor - River Esk", Type = "Water", Status = "Offline",
                    Latitude = 55.9200, Longitude = -3.1500, LastReadingValue = "N/A", LastReadingTimestamp = DateTime.Now.AddHours(-1),
                    NextMaintenanceDate = null, // Not scheduled
                    LastMaintenanceDate = DateTime.Now.AddYears(-1), MaintenanceNotes = "Sensor offline, investigate connection.",
                    SamplingIntervalSeconds = 1800, ReportingThreshold = null, IsEnabled = false, // Disabled
                    CurrentFirmwareVersion = "1.0.1", LatestFirmwareVersion = "1.0.1", FirmwareUpdateStatus = "Up to date"
                },
                 new Sensor // Example 4: Weather Sensor, different config, up-to-date firmware
                {
                    Id = 4, Name = "Humidity Sensor - Botanical Garden", Type = "Weather", Status = "Online",
                    Latitude = 55.9600, Longitude = -3.1950, LastReadingValue = "78%", LastReadingTimestamp = DateTime.Now.AddMinutes(-2),
                    NextMaintenanceDate = DateTime.Now.AddDays(90), LastMaintenanceDate = DateTime.Now.AddDays(-10), MaintenanceNotes = "",
                    SamplingIntervalSeconds = 900, IsEnabled = true, ReportingThreshold = 90.0,
                    CurrentFirmwareVersion = "3.0.0", LatestFirmwareVersion = "3.0.0", FirmwareUpdateStatus = "Up to date"
                }
            };
        }

        /// <summary>
        /// Retrieves the collection of mock sensors.
        /// </summary>
        /// <returns>The <see cref="ObservableCollection{Sensor}"/> containing the current mock sensor data.</returns>
        public ObservableCollection<Sensor> GetMockSensors()
        {
            return _mockSensors;
        }

        // --- Maintenance Methods ---

        /// <summary>
        /// Schedules maintenance for a specific sensor by updating its next maintenance date and notes.
        /// Directly supports the "Schedule maintenance" user story for Operations Managers.
        /// </summary>
        /// <param name="sensorId">The unique ID of the sensor to schedule maintenance for.</param>
        /// <param name="nextDate">The date for the next scheduled maintenance.</param>
        /// <param name="notes">Optional notes related to the maintenance task.</param>
        /// <returns>True if the sensor was found and updated successfully, false otherwise.</returns>
        public bool ScheduleMaintenance(int sensorId, DateTime nextDate, string notes)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                // Update properties - INotifyPropertyChanged on Sensor will update the UI
                sensor.NextMaintenanceDate = nextDate;
                sensor.MaintenanceNotes = notes;
                // Optionally update status if relevant (e.g., "Maintenance Scheduled")
                // sensor.Status = "Maintenance Scheduled";
                return true;
            }
            return false; // Sensor not found
        }

        /// <summary>
        /// Marks maintenance as complete for a specific sensor.
        /// Updates the last maintenance date, clears the next maintenance date and notes.
        /// Supports the "ensure timely checks" aspect for Operations Managers by recording completion.
        /// </summary>
        /// <param name="sensorId">The unique ID of the sensor whose maintenance is complete.</param>
        /// <param name="completionDate">The date and time when the maintenance was completed.</param>
        /// <returns>True if the sensor was found and updated successfully, false otherwise.</returns>
        public bool MarkMaintenanceComplete(int sensorId, DateTime completionDate)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                // Update properties - INotifyPropertyChanged takes care of UI updates
                sensor.LastMaintenanceDate = completionDate;
                sensor.NextMaintenanceDate = null; // Clear scheduled date
                sensor.MaintenanceNotes = $"Maintenance completed on {completionDate:yyyy-MM-dd}"; // Update notes
                // Optionally reset status if it was changed for scheduling
                // if(sensor.Status == "Maintenance Scheduled") sensor.Status = "Online"; // Or original status
                return true;
            }
            return false; // Sensor not found
        }

        // --- Configuration and Firmware Methods (Admin Focus) ---

        /// <summary>
        /// Updates the configuration settings for a specific sensor.
        /// Modifies properties like sampling interval, reporting threshold, and enabled status.
        /// Directly supports the "Update sensor configurations" user story for Administrators.
        /// </summary>
        /// <param name="sensorId">The unique ID of the sensor to configure.</param>
        /// <param name="newInterval">The new sampling interval in seconds.</param>
        /// <param name="newThreshold">The new reporting threshold (nullable).</param>
        /// <param name="newIsEnabled">The new enabled status (true or false).</param>
        /// <returns>True if the sensor was found and updated successfully, false otherwise.</returns>
        public bool UpdateSensorConfiguration(int sensorId, int newInterval, double? newThreshold, bool newIsEnabled)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor != null)
            {
                // Update configuration properties - UI updates via INotifyPropertyChanged
                sensor.SamplingIntervalSeconds = newInterval;
                sensor.ReportingThreshold = newThreshold;
                sensor.IsEnabled = newIsEnabled;

                // Simulate a status change if the sensor is disabled/enabled
                if (!newIsEnabled && sensor.Status.StartsWith("Online")) // Check prefix to handle "Online" or "Online (Low Battery)" etc.
                {
                    sensor.Status = "Offline (Disabled)"; // Set specific status
                }
                else if (newIsEnabled && sensor.Status == "Offline (Disabled)")
                {
                    // Simulate sensor coming back online - in reality, this might take time
                    sensor.Status = "Online";
                }
                // Add more sophisticated status logic if needed (e.g., based on actual communication checks)

                return true;
            }
            return false; // Sensor not found
        }

        /// <summary>
        /// Initiates a firmware update process for a specific sensor asynchronously.
        /// This method simulates the update process: sets status to "Updating...", waits, then sets
        /// the current version to the latest and status to "Up to date" (or "Failed" potentially).
        /// Directly supports the "Update sensor... firmware" user story for Administrators.
        /// The simulation uses Task.Delay to mimic a time-consuming operation.
        /// </summary>
        /// <param name="sensorId">The unique ID of the sensor to update.</param>
        /// <returns>A task that resolves to true if the update process was successfully initiated (sensor found and update needed), false otherwise.</returns>
        public async Task<bool> InitiateFirmwareUpdate(int sensorId)
        {
            var sensor = _mockSensors.FirstOrDefault(s => s.Id == sensorId);

            // Check if sensor exists and if an update is actually needed
            if (sensor != null && sensor.CurrentFirmwareVersion != sensor.LatestFirmwareVersion)
            {
                try
                {
                    // Update status to indicate process start - UI updates via INotifyPropertyChanged
                    sensor.FirmwareUpdateStatus = "Updating...";

                    // Simulate the time taken for the firmware update process
                    await Task.Delay(TimeSpan.FromSeconds(5)); // Simulate a 5-second update duration

                    // Simulate successful completion of the update
                    sensor.CurrentFirmwareVersion = sensor.LatestFirmwareVersion; // Update the version
                    sensor.FirmwareUpdateStatus = "Up to date"; // Update the status

                    // Optionally, simulate failure scenario:
                    // sensor.FirmwareUpdateStatus = "Update Failed";
                    // await DisplayAlert("Simulated Failure", "The firmware update failed.", "OK"); // (Needs access to UI thread or messaging center)

                    return true; // Indicate that the update process was initiated and (in this simulation) completed.
                }
                catch (Exception ex)
                {
                    // Handle potential exceptions during the simulated update
                    System.Diagnostics.Debug.WriteLine($"Error during simulated firmware update for sensor {sensorId}: {ex.Message}");
                    sensor.FirmwareUpdateStatus = "Update Failed"; // Set status to failed on error
                    return false; // Indicate failure
                }
            }
            else if (sensor != null)
            {
                // Sensor exists but is already up-to-date
                sensor.FirmwareUpdateStatus = "No update needed"; // Update status for clarity
                return false; // Update not initiated because it wasn't needed
            }
            else
            {
                return false; // Sensor not found, update cannot be initiated
            }
        }
    }
}
// --- END OF FILE SensorService.cs ---