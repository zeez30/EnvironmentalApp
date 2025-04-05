using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using EnvironmentalApp.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;

namespace EnvironmentalApp
{
    public partial class MainPage : ContentPage
    {
        private readonly ILogger<MainPage> _logger;

        // ObservableCollections for data binding to the UI.
        public ObservableCollection<Alert> RecentAlerts { get; set; } = new ObservableCollection<Alert>();
        public ObservableCollection<SensorStatus> SensorStatuses { get; set; } = new ObservableCollection<SensorStatus>();
        public ObservableCollection<AirQualityData> AirQualityData { get; set; } = new ObservableCollection<AirQualityData>();

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage(ILogger<MainPage> logger)
        {
            InitializeComponent();
            _logger = logger;
            BindingContext = this; //Set DataContext here
            InitializeDataAsync();
            ImportExcelDataAsync();
        }

        private async Task InitializeDataAsync()
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

        private async Task ImportExcelDataAsync()
        {
            string excelFilePath = Path.Combine(FileSystem.AppDataDirectory, "Air quality.xlsx");

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
                    _logger.LogError(ex, "Error copying Excel file");
                    await DisplayAlert("Error", "Failed to copy Excel file", "OK");
                    return;
                }
            }
           
            try
            {
                var database = DatabaseService.Instance; // Access the Instance
                await database.ImportAirQualityDataFromExcel(excelFilePath); // Await the import process
                                                                             // Retrieve data after import and update UI
                var airQualityData = await database.GetAirQualityDataAsync();

                // Convert the database data to an ObservableCollection for UI binding
                AirQualityData = new ObservableCollection<AirQualityData>(airQualityData);
                OnPropertyChanged(nameof(AirQualityData)); // Notify UI to update
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing or fetching data from Excel");
                await DisplayAlert("Error", "Failed to import/fetch data", "OK");
            } 
        }

        private async void OnMapViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapPage());
        }

        private async void OnSensorManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SensorManagementPage());
        }

        private async void OnDataAnalysisClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DataAnalysisPage());
        }

        private async void OnReportsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReportsPage());
        }

        private async void OnUserManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserManagementPage());
        }

        public class Alert
        {
            public string AlertMessage { get; set; } = string.Empty;
            public DateTime AlertTime { get; set; } = DateTime.Now;
        }

        public class SensorStatus
        {
            public string SensorID { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string AirQuality { get; set; } = string.Empty;
            public string WaterQuality { get; set; } = string.Empty;
        }
    }
}