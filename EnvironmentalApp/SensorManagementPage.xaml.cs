// --- START OF FILE SensorManagementPage.xaml.cs ---

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
    /// <summary>
    /// Represents the code-behind for the Sensor Management page.
    /// Allows users (primarily Administrators and Operations Managers) to view sensor details,
    /// schedule and manage maintenance, configure sensor settings, and update firmware.
    /// Implements INotifyPropertyChanged for dynamic UI updates.
    /// </summary>
    public partial class SensorManagementPage : ContentPage, INotifyPropertyChanged
    {
        private bool _isAdminVisible;
        /// <summary>
        /// Gets or sets a value indicating whether administrative controls (configuration, firmware updates) are visible.
        /// Determined by the logged-in user's role via FirebaseAuthService.
        /// Notifies the UI upon change.
        /// </summary>
        public bool IsAdminVisible
        {
            get => _isAdminVisible;
            set => SetProperty(ref _isAdminVisible, value, nameof(IsAdminVisible));
        }

        private ObservableCollection<Sensor> _sensors;
        /// <summary>
        /// Gets or sets the collection of sensors displayed on the page.
        /// Bound to the CollectionView in the XAML.
        /// Notifies the UI upon change.
        /// </summary>
        public ObservableCollection<Sensor> Sensors
        {
            get => _sensors;
            set => SetProperty(ref _sensors, value, nameof(Sensors));
        }

        private readonly SensorService _sensorService;
        private readonly FirebaseAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorManagementPage"/> class.
        /// Requires an instance of <see cref="FirebaseAuthService"/> to determine user role for admin controls.
        /// </summary>
        /// <param name="authService">The authentication service instance, used to check the current user's role.</param>
        public SensorManagementPage(FirebaseAuthService authService) // Corrected constructor dependency
        {
            InitializeComponent();
            _sensorService = new SensorService(); // Instantiate the service that manages sensor data
            _authService = authService ?? throw new ArgumentNullException(nameof(authService)); // Assign injected service, ensuring it's not null

            // Set initial role visibility based on logged-in user
            SetAdminVisibility();

            LoadSensors(); // Load initial sensor data
            BindingContext = this; // Set the binding context for XAML bindings
        }

        /// <summary>
        /// Sets the <see cref="IsAdminVisible"/> property based on the current user's role
        /// obtained from the FirebaseAuthService. Called during initialization.
        /// Relates to the Administrator's need to manage configurations/firmware.
        /// </summary>
        private void SetAdminVisibility()
        {
            // Check role from the auth service
            IsAdminVisible = _authService.CurrentUserRole == "admin";
            System.Diagnostics.Debug.WriteLine($"SensorManagementPage: Admin Visibility Set To: {IsAdminVisible} based on role '{_authService.CurrentUserRole}'"); // Debug output
        }

        /// <summary>
        /// Loads the list of sensors from the <see cref="SensorService"/>.
        /// Populates the <see cref="Sensors"/> collection, which updates the UI.
        /// </summary>
        private void LoadSensors()
        {
            // Get mock sensor data from the service
            Sensors = new ObservableCollection<Sensor>(_sensorService.GetMockSensors());
            // Since Sensor implements INotifyPropertyChanged, individual property updates
            // within a Sensor object will reflect automatically in the UI.
            // We only need to set the collection itself on initial load or if the collection reference changes.
        }

        // --- Maintenance Event Handlers ---

        /// <summary>
        /// Handles the click event for the 'Schedule Maintenance' button associated with a specific sensor.
        /// Prompts the user for the next maintenance date and optional notes using DisplayPromptAsync.
        /// Calls the SensorService to update the sensor's maintenance schedule.
        /// Displays success or error alerts to the user.
        /// This directly addresses the Operations Manager's need to schedule maintenance.
        /// </summary>
        /// <param name="sender">The button object that raised the event. Expected to have the Sensor object as CommandParameter.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnScheduleMaintenanceClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                // Prompt for date
                string dateStr = await DisplayPromptAsync($"Schedule Maintenance for {sensor.Name}",
                                                          "Enter next maintenance date (YYYY-MM-DD):",
                                                          "OK", "Cancel",
                                                          placeholder: DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"), // Default placeholder
                                                          initialValue: sensor.NextMaintenanceDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddDays(30).ToString("yyyy-MM-dd")); // Pre-fill if exists

                if (DateTime.TryParse(dateStr, out DateTime nextDate))
                {
                    // Prompt for notes
                    string notes = await DisplayPromptAsync("Maintenance Notes",
                                                           "Enter any specific notes for this task:",
                                                           "Save", "Skip",
                                                           placeholder: "e.g., Check calibration",
                                                           initialValue: sensor.MaintenanceNotes ?? ""); // Pre-fill notes if they exist

                    // Call the service to update the data
                    bool success = _sensorService.ScheduleMaintenance(sensor.Id, nextDate, notes ?? "");

                    if (success)
                    {
                        await DisplayAlert("Success", "Maintenance scheduled successfully.", "OK");
                        // UI updates automatically because Sensor.NextMaintenanceDate and Sensor.MaintenanceNotes
                        // raise PropertyChanged events (due to Sensor implementing INotifyPropertyChanged).
                        // No need to manually call LoadSensors() here.
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to schedule maintenance. Sensor not found?", "OK");
                    }
                }
                else if (dateStr != null) // User entered something, but it wasn't a valid date
                {
                    await DisplayAlert("Invalid Date", "Please enter a valid date in YYYY-MM-DD format.", "OK");
                }
                // If dateStr is null, the user cancelled the prompt. Do nothing.
            }
        }

        /// <summary>
        /// Handles the click event for the 'Mark Maintenance Complete' button for a specific sensor.
        /// Confirms the action with the user via DisplayAlert.
        /// Calls the SensorService to mark the maintenance as complete, updating the last maintenance date
        /// and clearing the next scheduled date and notes.
        /// Displays success or error alerts.
        /// This supports the Operations Manager's workflow for tracking completed maintenance.
        /// </summary>
        /// <param name="sender">The button object that raised the event. Expected to have the Sensor object as CommandParameter.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnMarkMaintenanceCompleteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                // Confirm action with the user
                bool confirm = await DisplayAlert("Confirm Completion",
                                                  $"Mark maintenance as complete for {sensor.Name} (Scheduled for: {sensor.NextMaintenanceDate:yyyy-MM-dd})?",
                                                  "Yes, Complete", "Cancel");
                if (confirm)
                {
                    DateTime completionDate = DateTime.Now; // Use current time as completion time
                    // Call the service
                    bool success = _sensorService.MarkMaintenanceComplete(sensor.Id, completionDate);
                    if (success)
                    {
                        await DisplayAlert("Success", "Maintenance marked as complete.", "OK");
                        // UI updates automatically due to INotifyPropertyChanged on Sensor properties.
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to mark maintenance as complete. Sensor not found?", "OK");
                    }
                }
            }
        }

        // --- Configuration and Firmware Event Handlers (Admin Functionality) ---

        /// <summary>
        /// Handles the click event for the 'Configure' button (Admin only).
        /// Prompts the administrator for new configuration settings (sampling interval, reporting threshold, enabled status)
        /// using DisplayPromptAsync and DisplayActionSheet.
        /// Calls the SensorService to update the sensor's configuration.
        /// Displays success or error alerts.
        /// This directly addresses the Administrator's need to update sensor configurations.
        /// </summary>
        /// <param name="sender">The button object that raised the event. Expected to have the Sensor object as CommandParameter.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnConfigureClicked(object sender, EventArgs e)
        {
            // This action should only be available if IsAdminVisible is true, enforced by the Button's IsVisible binding in XAML.
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                // --- Get New Configuration via Prompts ---
                // Sampling Interval
                string intervalStr = await DisplayPromptAsync($"Configure {sensor.Name}", "Sampling Interval (seconds):", "OK", "Cancel", sensor.SamplingIntervalSeconds.ToString(), keyboard: Keyboard.Numeric);
                if (intervalStr == null || !int.TryParse(intervalStr, out int newInterval) || newInterval <= 0)
                {
                    if (intervalStr != null) // Only show error if input was invalid, not if cancelled
                        await DisplayAlert("Invalid Input", "Please enter a valid positive number for sampling interval.", "OK");
                    return; // Cancelled or invalid input
                }

                // Reporting Threshold (nullable double)
                string thresholdStr = await DisplayPromptAsync(
                    $"Configure {sensor.Name}",                       // title
                    "Reporting Threshold (optional, numeric, e.g., for alerts):", // message
                    "OK",                                             // accept
                    "Cancel",                                         // cancel
                    placeholder: "Leave empty for none",             // placeholder
                    initialValue: sensor.ReportingThreshold?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "" // Use invariant culture for parsing/formatting
                );

                double? newThreshold = null;
                if (thresholdStr == null) return; // User cancelled threshold prompt

                if (!string.IsNullOrWhiteSpace(thresholdStr))
                {
                    if (double.TryParse(thresholdStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsedThreshold))
                    {
                        newThreshold = parsedThreshold;
                    }
                    else
                    {
                        await DisplayAlert("Invalid Input", "Please enter a valid number for threshold or leave empty.", "OK");
                        return; // Invalid numeric input
                    }
                }
                // If thresholdStr is whitespace or empty, newThreshold remains null, which is valid.

                // Enabled Status (using ActionSheet for boolean choice)
                string enabledAction = await DisplayActionSheet($"Configure {sensor.Name} - Set Sensor Status", "Cancel", null, "Enable", "Disable");
                bool newIsEnabled;
                if (enabledAction == "Enable") newIsEnabled = true;
                else if (enabledAction == "Disable") newIsEnabled = false;
                else return; // User cancelled action sheet or chose null option

                // --- Call Service to Update Configuration ---
                bool success = _sensorService.UpdateSensorConfiguration(sensor.Id, newInterval, newThreshold, newIsEnabled);

                if (success)
                {
                    await DisplayAlert("Success", "Sensor configuration updated successfully.", "OK");
                    // UI updates automatically via INotifyPropertyChanged on the Sensor object.
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update configuration. Sensor not found?", "OK");
                }
            }
        }

        /// <summary>
        /// Handles the click event for the 'Update Firmware' button (Admin only).
        /// Checks if an update is needed (current version != latest version).
        /// Confirms the update action with the administrator.
        /// Calls the asynchronous SensorService method <see cref="SensorService.InitiateFirmwareUpdate"/> to start the update process.
        /// The service simulates the update and updates the sensor's status properties, which trigger UI changes.
        /// This directly addresses the Administrator's need to update sensor firmware.
        /// </summary>
        /// <param name="sender">The button object that raised the event. Expected to have the Sensor object as CommandParameter.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnUpdateFirmwareClicked(object sender, EventArgs e)
        {
            // This action should only be available if IsAdminVisible is true, enforced by the Button's IsVisible binding in XAML.
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                // Check if firmware is already up-to-date
                if (sensor.CurrentFirmwareVersion == sensor.LatestFirmwareVersion)
                {
                    await DisplayAlert("Up-to-date", "Sensor firmware is already the latest version.", "OK");
                    sensor.FirmwareUpdateStatus = "Up to date"; // Ensure status reflects this, triggering UI update if needed
                    return;
                }

                // Confirm update action
                bool confirm = await DisplayAlert("Confirm Firmware Update",
                                                  $"Update firmware for {sensor.Name} from version {sensor.CurrentFirmwareVersion} to {sensor.LatestFirmwareVersion}?",
                                                  "Start Update", "Cancel");

                if (confirm)
                {
                    // Optionally, provide immediate visual feedback that the process has started.
                    // The service method will also update this, but this makes the UI react instantly.
                    // sensor.FirmwareUpdateStatus = "Initiating...";

                    // Call the async service method to initiate the update.
                    // The service handles the simulation (delay) and status changes ("Updating...", "Up to date"/"Failed").
                    bool initiated = await _sensorService.InitiateFirmwareUpdate(sensor.Id);

                    if (initiated)
                    {
                        // The service takes over updating the status property.
                        // The UI will update automatically when the Sensor object's FirmwareUpdateStatus changes.
                        await DisplayAlert("In Progress", "Firmware update initiated. The status will refresh automatically upon completion.", "OK");
                    }
                    else
                    {
                        // This might happen if the sensor wasn't found or if the check inside the service method fails.
                        await DisplayAlert("Error", "Could not start firmware update. Sensor may be offline or already up-to-date.", "OK");
                        // Optionally reset status if it was changed prematurely
                        // sensor.FirmwareUpdateStatus = "Update Available"; // Or appropriate previous state
                    }
                }
            }
        }


        // --- INotifyPropertyChanged Implementation ---
        /// <summary>
        /// Occurs when a property value changes. Used for updating UI bindings.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Helper method to set a property value and raise the PropertyChanged event if the value has changed.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="propertyName">The name of the property (automatically inferred).</param>
        /// <returns>True if the value was changed, false otherwise.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false; // Value hasn't changed
            storage = value; // Update the backing field
            OnPropertyChanged(propertyName); // Raise the event
            return true;
        }
        // --- End INotifyPropertyChanged ---
    }
}
// --- END OF FILE SensorManagementPage.xaml.cs ---