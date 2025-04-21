using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System;
using EnvironmentalApp.Data;

namespace EnvironmentalApp
{

    public partial class MainPage : ContentPage
    {
        // ObservableCollections to hold data that will be displayed in the UI.
        // These are used for data binding, so changes to these collections will automatically update the UI.
        public ObservableCollection<Alert> RecentAlerts { get; set; }
        public ObservableCollection<SensorStatus> SensorStatuses { get; set; }

        public bool IsAdminVisible { get; set; }

        private readonly FirebaseAuthService _authService;

        // Constructor for the MainPage class.
        public MainPage(FirebaseAuthService authService)
        {
            InitializeComponent();
            InitializeData();
            BindingContext = this;
            _authService = authService;
            SetRoleBasedVisibility();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            string role = _authService.CurrentUserRole;
            IsAdminVisible = role == "admin";

            OnPropertyChanged(nameof(IsAdminVisible));
        }
        private void SetRoleBasedVisibility()
        {
            // Based on the role, show or hide admin-specific buttons
            if (_authService.CurrentUserRole == "admin")
            {
                IsAdminVisible = true; // Show admin buttons
            }
            else
            {
                IsAdminVisible = false; // Hide admin buttons
            }

            // Notify the UI that bindings have been updated
            OnPropertyChanged(nameof(IsAdminVisible));
        }

        // Initializes the data for the page.
        private void InitializeData()
        {
            // Initializes the RecentAlerts collection with sample data.
            RecentAlerts = new ObservableCollection<Alert>
            {
                new Alert { AlertMessage = "High air pollution detected at sensor 123", AlertTime = DateTime.Now.AddHours(-1) },
                new Alert { AlertMessage = "Water pH level exceeded threshold at sensor 456", AlertTime = DateTime.Now.AddHours(-2) },
                new Alert { AlertMessage = "Sensor 789 offline. Please check.", AlertTime = DateTime.Now.AddHours(-3) }
            };

            // Initializes the SensorStatuses collection with sample data.
            SensorStatuses = new ObservableCollection<SensorStatus>
            {
                new SensorStatus { SensorID = "Sensor 123", Status = "Online", AirQuality = "High", WaterQuality = "Normal" },
                new SensorStatus { SensorID = "Sensor 456", Status = "Online", AirQuality = "Normal", WaterQuality = "Critical" },
                new SensorStatus { SensorID = "Sensor 789", Status = "Offline", AirQuality = "N/A", WaterQuality = "N/A" }
            };

            // Sample data here is for demonstration purposes.
        }

        // Event handler for the "View Map" button click.
        private async void OnMapViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapPage()); // Navigates to the MapPage.
        }

        // Event handler for the "Sensor Management" button click.
        private async void OnSensorManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SensorManagementPage()); // Navigates to the SensorManagementPage.
        }

        // Event handler for the "Data Analysis" button click.
        private async void OnDataAnalysisClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DataAnalysisPage()); // Navigates to the DataAnalysisPage.
        }

        // Event handler for the "Reports" button click.
        private async void OnReportsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReportsPage()); // Navigates to the ReportsPage.
        }

        // Event handler for the "User Management" button click.
        private async void OnUserManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserManagementPage(_authService)); // Navigates to the UserManagementPage.
        }

        // Class to represent an alert.
        public class Alert
        {
            public string AlertMessage { get; set; }
            public DateTime AlertTime { get; set; }
        }

        // Class to represent the status of a sensor.
        public class SensorStatus
        {
            public string SensorID { get; set; }
            public string Status { get; set; }
            public string AirQuality { get; set; }
            public string WaterQuality { get; set; }
        }
    }
}
