// --- START OF FILE Sensors.cs ---

// File: EnvironmentalApp/Data/Sensor.cs
using System;
using System.ComponentModel; // Required for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Required for CallerMemberName

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents a single environmental sensor device within the monitoring network.
    /// Contains properties for identification, location, status, readings, maintenance schedule,
    /// configuration settings, and firmware information.
    /// Implements INotifyPropertyChanged to support dynamic updates in the UI (e.g., in SensorManagementPage).
    /// </summary>
    public class Sensor : INotifyPropertyChanged
    {
        // Backing fields for properties
        private int _id;
        private string _name;
        private string _type;
        private string _status;
        private double _latitude;
        private double _longitude;
        private string _lastReadingValue;
        private DateTime _lastReadingTimestamp;
        private DateTime? _nextMaintenanceDate;
        private DateTime? _lastMaintenanceDate;
        private string _maintenanceNotes;
        private int _samplingIntervalSeconds = 3600; // Default to 1 hour
        private double? _reportingThreshold = null;
        private bool _isEnabled = true;
        private string _currentFirmwareVersion = "1.0.0";
        private string _latestFirmwareVersion = "1.0.0"; // Assume same initially
        private string _firmwareUpdateStatus = "Idle";

        /// <summary>
        /// Gets or sets the unique identifier for the sensor.
        /// </summary>
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Gets or sets the descriptive name of the sensor (e.g., "Air Quality Monitor - City Center").
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Gets or sets the type of sensor (e.g., "Air", "Water", "Weather").
        /// </summary>
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        /// <summary>
        /// Gets or sets the current operational status of the sensor (e.g., "Online", "Offline", "Maintenance Scheduled", "Offline (Disabled)").
        /// </summary>
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Gets or sets the geographical latitude of the sensor's location.
        /// </summary>
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        /// <summary>
        /// Gets or sets the geographical longitude of the sensor's location.
        /// </summary>
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        /// <summary>
        /// Gets or sets a string representation of the last reading received from the sensor (e.g., "15.5°C", "AQI: 45").
        /// </summary>
        public string LastReadingValue
        {
            get => _lastReadingValue;
            set => SetProperty(ref _lastReadingValue, value);
        }

        /// <summary>
        /// Gets or sets the timestamp of the last reading received from the sensor.
        /// </summary>
        public DateTime LastReadingTimestamp
        {
            get => _lastReadingTimestamp;
            set => SetProperty(ref _lastReadingTimestamp, value);
        }

        // --- Maintenance Properties (Relevant to Operations Manager) ---

        /// <summary>
        /// Gets or sets the date for the next scheduled maintenance. Null if no maintenance is currently scheduled.
        /// Used for tracking upcoming maintenance tasks ("Schedule maintenance").
        /// </summary>
        public DateTime? NextMaintenanceDate
        {
            get => _nextMaintenanceDate;
            set => SetProperty(ref _nextMaintenanceDate, value);
        }

        /// <summary>
        /// Gets or sets the date when the last maintenance was performed. Null if no maintenance has been recorded.
        /// Used for historical tracking ("ensure timely checks").
        /// </summary>
        public DateTime? LastMaintenanceDate
        {
            get => _lastMaintenanceDate;
            set => SetProperty(ref _lastMaintenanceDate, value);
        }

        /// <summary>
        /// Gets or sets any relevant notes regarding the scheduled or last completed maintenance.
        /// </summary>
        public string MaintenanceNotes
        {
            get => _maintenanceNotes;
            set => SetProperty(ref _maintenanceNotes, value);
        }

        // --- Configuration Properties (Relevant to Administrator) ---

        /// <summary>
        /// Gets or sets the interval, in seconds, at which the sensor takes readings.
        /// Part of "Update sensor configurations".
        /// </summary>
        public int SamplingIntervalSeconds
        {
            get => _samplingIntervalSeconds;
            set => SetProperty(ref _samplingIntervalSeconds, value);
        }

        /// <summary>
        /// Gets or sets an optional reporting threshold value. Readings exceeding this value might trigger alerts.
        /// Example configuration property. Null if no threshold is set.
        /// Part of "Update sensor configurations".
        /// </summary>
        public double? ReportingThreshold
        {
            get => _reportingThreshold;
            set => SetProperty(ref _reportingThreshold, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sensor is currently enabled and actively reporting data.
        /// Part of "Update sensor configurations".
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        // --- Firmware Properties (Relevant to Administrator) ---

        /// <summary>
        /// Gets or sets the currently installed firmware version on the sensor.
        /// Compared against <see cref="LatestFirmwareVersion"/> to determine if an update is needed.
        /// </summary>
        public string CurrentFirmwareVersion
        {
            get => _currentFirmwareVersion;
            set => SetProperty(ref _currentFirmwareVersion, value);
        }

        /// <summary>
        /// Gets or sets the latest available firmware version for this sensor type.
        /// In a real system, this might be fetched from a central server.
        /// </summary>
        public string LatestFirmwareVersion
        {
            get => _latestFirmwareVersion;
            set => SetProperty(ref _latestFirmwareVersion, value);
        }

        /// <summary>
        /// Gets or sets the current status of any ongoing or pending firmware update (e.g., "Idle", "Update Available", "Updating...", "Up to date", "Failed").
        /// Used to display feedback during the "Update sensor... firmware" process.
        /// </summary>
        public string FirmwareUpdateStatus
        {
            get => _firmwareUpdateStatus;
            set => SetProperty(ref _firmwareUpdateStatus, value);
        }


        // --- INotifyPropertyChanged Implementation ---
        /// <summary>
        /// Occurs when a property value changes. Required for data binding to update the UI automatically.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event, notifying listeners that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. Automatically inferred using CallerMemberName.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper method to set a property's backing field and raise the PropertyChanged event only if the value has actually changed.
        /// Reduces redundant event raising.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">A reference to the backing field of the property.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="propertyName">The name of the property being changed. Automatically inferred.</param>
        /// <returns>True if the value was changed and the event was raised, false otherwise.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            // Check if the value is actually different before updating and raising the event
            if (object.Equals(storage, value)) return false;

            storage = value; // Update the backing field
            OnPropertyChanged(propertyName); // Raise the event
            return true;
        }
        // --- End INotifyPropertyChanged ---
    }
}
// --- END OF FILE Sensors.cs ---