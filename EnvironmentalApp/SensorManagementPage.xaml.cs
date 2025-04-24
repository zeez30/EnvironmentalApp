// File: EnvironmentalApp/SensorManagementPage.xaml.cs
using System.Collections.ObjectModel;
using EnvironmentalApp.Data;
using EnvironmentalApp.Services;
using Microsoft.Maui.Controls;
using System; // Add this
using System.Linq; // Add this
using System.ComponentModel; // Add this for INotifyPropertyChanged

namespace EnvironmentalApp
{
    public partial class SensorManagementPage : ContentPage, INotifyPropertyChanged
    {
        // --- Role Visibility ---
        private bool _isAdminVisible;
        public bool IsAdminVisible
        {
            get => _isAdminVisible;
            set
            {
                if (_isAdminVisible != value)
                {
                    _isAdminVisible = value;
                    OnPropertyChanged(nameof(IsAdminVisible)); // Notify UI of change
                }
            }
        }
        // --- End Role Visibility ---


        // Use ObservableCollection for automatic UI updates when items are added/removed
        // However, changes *within* an item (like NextMaintenanceDate) require INotifyPropertyChanged on the Sensor class
        // or reloading the data for the UI to update reliably without full MVVM.
        private ObservableCollection<Sensor> _sensors;
        public ObservableCollection<Sensor> Sensors
        {
            get => _sensors;
            set
            {
                _sensors = value;
                OnPropertyChanged(nameof(Sensors));
            }
        }


        private readonly SensorService _sensorService;
        // Inject FirebaseAuthService if needed for role check, assuming it's registered
        // private readonly FirebaseAuthService _authService;

        // Constructor - Modify if using Dependency Injection for services
        // public SensorManagementPage(SensorService sensorService, FirebaseAuthService authService)
        public SensorManagementPage(/*FirebaseAuthService authService*/) // Simplified constructor
        {
            InitializeComponent();
            _sensorService = new SensorService(); // Direct instantiation (consider DI later)
            // _authService = authService; // Uncomment if injecting auth service

            // Set initial role visibility (assuming Admin for now)
            // Replace with actual role check from _authService if available
            IsAdminVisible = true; // Placeholder: Assume admin/ops manager access

            LoadSensors();
            BindingContext = this; // Set binding context to the page itself
        }

        // --- INotifyPropertyChanged Implementation ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        // --- End INotifyPropertyChanged ---

        private void LoadSensors()
        {
            // Get sensors from the service
            Sensors = _sensorService.GetMockSensors();
            // Note: If Sensor class implemented INotifyPropertyChanged, UI would update
            // individual property changes automatically. Without it, changes made via
            // service methods might require reloading the whole collection to reflect in UI.
        }

        // Event Handler for Scheduling Maintenance
        private async void OnScheduleMaintenanceClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                // Prompt user for date and notes
                string dateStr = await DisplayPromptAsync($"Schedule Maintenance for {sensor.Name}",
                                                          "Enter next maintenance date (YYYY-MM-DD):",
                                                          "OK", "Cancel",
                                                          placeholder: DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"), // Suggest a date
                                                          initialValue: sensor.NextMaintenanceDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"));

                if (DateTime.TryParse(dateStr, out DateTime nextDate))
                {
                    string notes = await DisplayPromptAsync("Maintenance Notes",
                                                           "Enter any specific notes for this task:",
                                                           "Save", "Skip",
                                                           placeholder: "e.g., Check calibration",
                                                           initialValue: sensor.MaintenanceNotes ?? "");

                    // Call the service to update the sensor
                    bool success = _sensorService.ScheduleMaintenance(sensor.Id, nextDate, notes ?? ""); // Use empty string if notes are skipped

                    if (success)
                    {
                        await DisplayAlert("Success", "Maintenance scheduled.", "OK");
                        // Reload data to reflect changes in the UI (simplest way without INotifyPropertyChanged on Sensor)
                        LoadSensors();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to schedule maintenance.", "OK");
                    }
                }
                else if (dateStr != null) // User entered something invalid, not just cancelled
                {
                    await DisplayAlert("Invalid Date", "Please enter a valid date in YYYY-MM-DD format.", "OK");
                }
            }
        }

        // Event Handler for Marking Maintenance Complete
        private async void OnMarkMaintenanceCompleteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Sensor sensor)
            {
                bool confirm = await DisplayAlert("Confirm Completion",
                                                  $"Mark maintenance as complete for {sensor.Name} (Scheduled for: {sensor.NextMaintenanceDate:yyyy-MM-dd})?",
                                                  "Yes, Complete", "Cancel");

                if (confirm)
                {
                    // Use current date as completion date
                    DateTime completionDate = DateTime.Now;

                    // Call the service
                    bool success = _sensorService.MarkMaintenanceComplete(sensor.Id, completionDate);

                    if (success)
                    {
                        await DisplayAlert("Success", "Maintenance marked as complete.", "OK");
                        // Reload data to reflect changes
                        LoadSensors();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to mark maintenance as complete.", "OK");
                    }
                }
            }
        }

        // --- Optional: Converters Implementation (place here or in separate files) ---

        // Example: Convert null DateTime? to a color (e.g., Red if overdue, Black otherwise)
        /*
        public class DateToColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is DateTime nextDate)
                {
                    return nextDate < DateTime.Now ? Colors.Red : Colors.Black; // Red if overdue
                }
                return Colors.Gray; // Not scheduled
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
        */

        // Example: Convert null object to bool (useful for IsEnabled binding)
        /*
        public class NullToBoolConverter : IValueConverter
        {
             // Parameter allows inverting: True means null = true, False means null = false
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool shouldBeTrueIfNull = false;
                if (parameter is bool b) { shouldBeTrueIfNull = b; }
                else if (parameter is string s && bool.TryParse(s, out bool sb)) { shouldBeTrueIfNull = sb; }

                return (value == null) ? shouldBeTrueIfNull : !shouldBeTrueIfNull;
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
        */

        // Example: Convert non-empty string to bool
        /*
        public class StringToBoolConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return !string.IsNullOrEmpty(value as string);
            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
        }
        */
        // --- End Converters ---

    }
}