using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using EnvironmentalApp.Data; // Assuming your data models and database service are in this namespace

namespace EnvironmentalApp
{
    /// <summary>
    /// Represents the main page of the application, displaying alerts and sensor statuses.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        // ObservableCollections for data binding to the UI.
        public ObservableCollection<Alert> RecentAlerts { get; set; }
        public ObservableCollection<SensorStatus> SensorStatuses { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent(); // Initializes the XAML components.
            InitializeData(); // Initializes the initial data.
            BindingContext = this; // Sets the binding context for data binding.
            ImportExcelDataAsync(); // Imports data from the Excel file.
        }

        /// <summary>
        /// Initializes the sample data for alerts and sensor statuses.
        /// </summary>
        private void InitializeData()
        {
            RecentAlerts = new ObservableCollection<Alert>
            {
                new Alert { AlertMessage = "High air pollution detected at sensor 123", AlertTime = DateTime.Now.AddHours(-1) },
                new Alert { AlertMessage = "Water pH level exceeded threshold at sensor 456", AlertTime = DateTime.Now.AddHours(-2) },
                new Alert { AlertMessage = "Sensor 789 offline. Please check.", AlertTime = DateTime.Now.AddHours(-3) }
            };

            SensorStatuses = new ObservableCollection<SensorStatus>
            {
                new SensorStatus { SensorID = "Sensor 123", Status = "Online", AirQuality = "High", WaterQuality = "Normal" },
                new SensorStatus { SensorID = "Sensor 456", Status = "Online", AirQuality = "Normal", WaterQuality = "Critical" },
                new SensorStatus { SensorID = "Sensor 789", Status = "Offline", AirQuality = "N/A", WaterQuality = "N/A" }
            };
        }

        /// <summary>
        /// Imports air quality data from an Excel file into the SQLite database.
        /// </summary>
        private async Task ImportExcelDataAsync()
        {
            string excelFilePath = Path.Combine(FileSystem.AppDataDirectory, "Air quality.xlsx");

            // Check if the Excel file exists in the correct location; if not, copy it from resources.
            if (!File.Exists(excelFilePath))
            {
                try
                {
                    using var sourceStream = await FileSystem.OpenAppPackageFileAsync("Air quality.xlsx");
                    using var destinationStream = File.OpenWrite(excelFilePath);
                    await sourceStream.CopyToAsync(destinationStream);
                }
                catch (Exception ex)
                {
                    // Log the exception and handle the error gracefully.
                    Console.WriteLine($"Error copying Excel file: {ex.Message}");
                    // Optionally, show an error message to the user.
                    return; // Exit the method to prevent further errors.
                }
            }

            try
            {
                var database = await DatabaseService.Instance;
                await database.ImportAirQualityDataFromExcel(excelFilePath);

                // Optionally, fetch and display the imported data in the UI.
                var airQualityData = await database.GetAirQualityDataAsync();
                // Process and display airQualityData in the UI here.
            }
            catch (Exception ex)
            {
                // Log the exception and handle the error gracefully.
                Console.WriteLine($"Error importing data from Excel: {ex.Message}");
                // Optionally, show an error message to the user.
            }
        }

        /// <summary>
        /// Event handler for the "View Map" button click, navigates to the MapPage.
        /// </summary>
        private async void OnMapViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapPage());
        }

        /// <summary>
        /// Event handler for the "Sensor Management" button click, navigates to the SensorManagementPage.
        /// </summary>
        private async void OnSensorManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SensorManagementPage());
        }

        /// <summary>
        /// Event handler for the "Data Analysis" button click, navigates to the DataAnalysisPage.
        /// </summary>
        private async void OnDataAnalysisClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DataAnalysisPage());
        }

        /// <summary>
        /// Event handler for the "Reports" button click, navigates to the ReportsPage.
        /// </summary>
        private async void OnReportsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReportsPage());
        }

        /// <summary>
        /// Event handler for the "User Management" button click, navigates to the UserManagementPage.
        /// </summary>
        private async void OnUserManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserManagementPage());
        }

        /// <summary>
        /// Represents an alert message with a timestamp.
        /// </summary>
        public class Alert
        {
            public string AlertMessage { get; set; }
            public DateTime AlertTime { get; set; }
        }

        /// <summary>
        /// Represents the status of a sensor, including air and water quality.
        /// </summary>
        public class SensorStatus
        {
            public string SensorID { get; set; }
            public string Status { get; set; }
            public string AirQuality { get; set; }
            public string WaterQuality { get; set; }
        }
    }
}