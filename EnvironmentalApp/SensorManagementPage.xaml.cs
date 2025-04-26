// File: EnvironmentalApp/SensorManagementPage.xaml.cs
using System.Collections.ObjectModel;
using EnvironmentalApp.Data;
using EnvironmentalApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks; // Add for Task

namespace EnvironmentalApp
{
    public partial class SensorManagementPage : ContentPage, INotifyPropertyChanged
    {
        private bool _isAdminVisible;
        public bool IsAdminVisible
        {
            get => _isAdminVisible;
            set => SetProperty(ref _isAdminVisible, value, nameof(IsAdminVisible));
        }

        private ObservableCollection<Sensor> _sensors;
        public ObservableCollection<Sensor> Sensors
        {
            get => _sensors;
            set => SetProperty(ref _sensors, value, nameof(Sensors));
        }

        private readonly SensorService _sensorService;
        private readonly FirebaseAuthService _authService; // Assuming it's injected or available

        // Constructor needs FirebaseAuthService for role check
        public SensorManagementPage(FirebaseAuthService authService) // Corrected constructor dependency
        {
            InitializeComponent();
            _sensorService = new SensorService();
            _authService = authService; // Assign injected service

            // Set initial role visibility based on logged-in user
            SetAdminVisibility();

            LoadSensors();
            BindingContext = this;
        }

        // Alternative constructor if not using DI for SensorManagementPage itself
        // but still passing authService from MainPage
        /*
        public SensorManagementPage(FirebaseAuthService authService)
        {
            InitializeComponent();
            _sensorService = new SensorService();
            _authService = authService; // Use the passed service
            SetAdminVisibility();
            LoadSensors();
            BindingContext = this;
        }
        */

        private void SetAdminVisibility()
        {
            // Check role from the auth service
            IsAdminVisible = _authService.CurrentUserRole == "admin";
            System.Diagnostics.Debug.WriteLine($"Admin Visibility Set To: {IsAdminVisible}"); // Debug output
        }

        // --- Existing Methods (LoadSensors, Maintenance Handlers) ---
        private void LoadSensors()
        {
            Sensors = new ObservableCollection<Sensor>(_sensorService.GetMockSensors());
            // Since Sensor implements INotifyPropertyChanged, we don't need to re-assign
            // the whole collection for property updates to reflect in UI.
            // But for Add/Remove or initial load, we still set the collection.
        }

        // Event Handlers for Maintenance (keep previous implementation)
        private async void OnScheduleMaintenanceClicked(object sender, EventArgs e)
        {
            // ... same as before ...
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                string dateStr = await DisplayPromptAsync($"Schedule Maintenance for {sensor.Name}",
                                                          "Enter next maintenance date (YYYY-MM-DD):",
                                                          "OK", "Cancel",
                                                          placeholder: DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"),
                                                          initialValue: sensor.NextMaintenanceDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"));

                if (DateTime.TryParse(dateStr, out DateTime nextDate))
                {
                    string notes = await DisplayPromptAsync("Maintenance Notes",
                                                           "Enter any specific notes for this task:",
                                                           "Save", "Skip",
                                                           placeholder: "e.g., Check calibration",
                                                           initialValue: sensor.MaintenanceNotes ?? "");

                    bool success = _sensorService.ScheduleMaintenance(sensor.Id, nextDate, notes ?? "");

                    if (success)
                    {
                        await DisplayAlert("Success", "Maintenance scheduled.", "OK");
                        // No need to call LoadSensors() if INotifyPropertyChanged works
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to schedule maintenance.", "OK");
                    }
                }
                else if (dateStr != null)
                {
                    await DisplayAlert("Invalid Date", "Please enter a valid date in YYYY-MM-DD format.", "OK");
                }
            }
        }

        private async void OnMarkMaintenanceCompleteClicked(object sender, EventArgs e)
        {
            // ... same as before ...
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                bool confirm = await DisplayAlert("Confirm Completion",
                                                  $"Mark maintenance as complete for {sensor.Name} (Scheduled for: {sensor.NextMaintenanceDate:yyyy-MM-dd})?",
                                                  "Yes, Complete", "Cancel");
                if (confirm)
                {
                    DateTime completionDate = DateTime.Now;
                    bool success = _sensorService.MarkMaintenanceComplete(sensor.Id, completionDate);
                    if (success)
                    {
                        await DisplayAlert("Success", "Maintenance marked as complete.", "OK");
                        // No need to call LoadSensors() if INotifyPropertyChanged works
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to mark maintenance as complete.", "OK");
                    }
                }
            }
        }

        // --- New Handlers for Config/Firmware ---

        private async void OnConfigureClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                // --- Get New Configuration via Prompts (Simple Simulation) ---
                // Interval
                string intervalStr = await DisplayPromptAsync($"Configure {sensor.Name}", "Sampling Interval (seconds):", "OK", "Cancel", sensor.SamplingIntervalSeconds.ToString(), keyboard: Keyboard.Numeric);
                if (intervalStr == null || !int.TryParse(intervalStr, out int newInterval) || newInterval <= 0)
                {
                    await DisplayAlert("Invalid Input", "Please enter a valid positive number for interval.", "OK");
                    return; // Cancelled or invalid input
                }

                // Threshold (nullable double)
                string thresholdStr = await DisplayPromptAsync(
                    $"Configure {sensor.Name}",                       // title
                    "Reporting Threshold (optional, numeric):",       // message
                    "OK",                                             // accept
                    "Cancel",                                         // cancel
                    placeholder: "e.g., 150.0 or empty",             // NAMED placeholder
                    initialValue: sensor.ReportingThreshold?.ToString() ?? "" // NAMED initialValue
                                                                              // You could add keyboard: Keyboard.Numeric here if desired, but it's optional
                ); double? newThreshold = null;
                if (!string.IsNullOrWhiteSpace(thresholdStr))
                {
                    if (double.TryParse(thresholdStr, out double parsedThreshold))
                    {
                        newThreshold = parsedThreshold;
                    }
                    else
                    {
                        await DisplayAlert("Invalid Input", "Please enter a valid number for threshold or leave empty.", "OK");
                        return; // Invalid numeric input
                    }
                }

                // Enabled Status (using ActionSheet)
                string enabledAction = await DisplayActionSheet($"Configure {sensor.Name} - Set Status", "Cancel", null, "Enable", "Disable");
                bool? newIsEnabledNullable = null;
                if (enabledAction == "Enable") newIsEnabledNullable = true;
                else if (enabledAction == "Disable") newIsEnabledNullable = false;

                if (newIsEnabledNullable == null) return; // User cancelled action sheet

                bool newIsEnabled = newIsEnabledNullable.Value;

                // --- Call Service ---
                bool success = _sensorService.UpdateSensorConfiguration(sensor.Id, newInterval, newThreshold, newIsEnabled);

                if (success)
                {
                    await DisplayAlert("Success", "Sensor configuration updated.", "OK");
                    // UI should update automatically due to INotifyPropertyChanged on Sensor
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update configuration.", "OK");
                }
            }
        }

        private async void OnUpdateFirmwareClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                if (sensor.CurrentFirmwareVersion == sensor.LatestFirmwareVersion)
                {
                    await DisplayAlert("Up-to-date", "Sensor firmware is already the latest version.", "OK");
                    sensor.FirmwareUpdateStatus = "Up to date"; // Ensure status reflects this
                    return;
                }

                bool confirm = await DisplayAlert("Confirm Update", $"Update firmware for {sensor.Name} from {sensor.CurrentFirmwareVersion} to {sensor.LatestFirmwareVersion}?", "Start Update", "Cancel");

                if (confirm)
                {
                    // Visually indicate update is starting (optional if service does it quickly)
                    // sensor.FirmwareUpdateStatus = "Initiating...";

                    // Call the async service method
                    bool initiated = await _sensorService.InitiateFirmwareUpdate(sensor.Id);

                    if (initiated)
                    {
                        // The service now handles setting "Updating..." and eventually "Up to date" or "Failed"
                        // The UI updates via INotifyPropertyChanged
                        await DisplayAlert("In Progress", "Firmware update initiated. Status will refresh.", "OK");
                    }
                    else
                    {
                        // This happens if sensor wasn't found or update wasn't needed (redundant check)
                        await DisplayAlert("Error", "Could not start firmware update.", "OK");
                    }
                }
            }
        }


        // --- INotifyPropertyChanged Implementation ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        // Generic SetProperty helper for INotifyPropertyChanged
        protected bool SetProperty<T>(ref T storage, T value, string propertyName)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        // --- End INotifyPropertyChanged ---
    }
}