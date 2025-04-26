using System;
using System.ComponentModel; // Add for INotifyPropertyChanged

namespace EnvironmentalApp.Data
{
    // Implement INotifyPropertyChanged for better UI updates when individual properties change
    public class Sensor : INotifyPropertyChanged
    {
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
        private double? _reportingThreshold = null; // Example: Alert threshold
        private bool _isEnabled = true;

        private string _currentFirmwareVersion = "1.0.0";
        private string _latestFirmwareVersion = "1.1.0"; // Simulate a newer version available
        private string _firmwareUpdateStatus = "Idle"; // e.g., Idle, Updating, Success, Failed

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Type // Air, Water, Weather
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        public string Status // Online, Offline, Malfunctioning
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }
        public string LastReadingValue
        {
            get => _lastReadingValue;
            set => SetProperty(ref _lastReadingValue, value);
        }
        public DateTime LastReadingTimestamp
        {
            get => _lastReadingTimestamp;
            set => SetProperty(ref _lastReadingTimestamp, value);
        }

        // --- Maintenance Properties ---
        public DateTime? NextMaintenanceDate
        {
            get => _nextMaintenanceDate;
            set => SetProperty(ref _nextMaintenanceDate, value);
        }
        public DateTime? LastMaintenanceDate
        {
            get => _lastMaintenanceDate;
            set => SetProperty(ref _lastMaintenanceDate, value);
        }
        public string MaintenanceNotes
        {
            get => _maintenanceNotes;
            set => SetProperty(ref _maintenanceNotes, value);
        }

        // --- Configuration Properties ---
        public int SamplingIntervalSeconds
        {
            get => _samplingIntervalSeconds;
            set => SetProperty(ref _samplingIntervalSeconds, value);
        }
        public double? ReportingThreshold // Example property
        {
            get => _reportingThreshold;
            set => SetProperty(ref _reportingThreshold, value);
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        // --- Firmware Properties ---
        public string CurrentFirmwareVersion
        {
            get => _currentFirmwareVersion;
            set => SetProperty(ref _currentFirmwareVersion, value);
        }
        public string LatestFirmwareVersion
        {
            get => _latestFirmwareVersion;
            set => SetProperty(ref _latestFirmwareVersion, value);
        }
        public string FirmwareUpdateStatus
        {
            get => _firmwareUpdateStatus;
            set => SetProperty(ref _firmwareUpdateStatus, value);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}