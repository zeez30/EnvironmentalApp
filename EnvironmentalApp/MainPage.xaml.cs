using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System;

namespace EnvironmentalApp
{
    public partial class MainPage : ContentPage
    {
        // ObservableCollections to hold data that will be displayed in the UI.
        // These are used for data binding, so changes to these collections will automatically update the UI.
        public ObservableCollection<Alert> RecentAlerts { get; set; }
        public ObservableCollection<SensorStatus> SensorStatuses { get; set; }

        // Constructor for the MainPage class.
        public MainPage()
        {
            InitializeComponent(); // Initializes the XAML components of the page.
            InitializeData(); // Initializes the data for the page.
            BindingContext = this; // Sets the binding context of the page to this instance, allowing data binding.
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

            
            //sample data here is for demonstration purposes.
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
            await Navigation.PushAsync(new UserManagementPage()); // Navigates to the UserManagementPage.
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