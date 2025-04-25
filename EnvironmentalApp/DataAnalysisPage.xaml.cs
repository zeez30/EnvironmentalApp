using EnvironmentalApp.Data;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using OfficeOpenXml; // Namespace for EPPlus
using System.Linq; // Required for Linq queries like .ToList()

namespace EnvironmentalApp
{
    public partial class DataAnalysisPage : ContentPage
    {
        // Collections to hold the loaded data for each type
        public ObservableCollection<AirQualityReading> AirQualityData { get; set; }
        public ObservableCollection<WaterQualityReading> WaterQualityData { get; set; }
        public ObservableCollection<WeatherReading> WeatherData { get; set; }

        public DataAnalysisPage()
        {
            InitializeComponent();

            // Initialize collections
            AirQualityData = new ObservableCollection<AirQualityReading>();
            WaterQualityData = new ObservableCollection<WaterQualityReading>();
            WeatherData = new ObservableCollection<WeatherReading>();

            // Set default selected data type (optional)
            DataTypePicker.SelectedIndex = 0; // Default to Air Quality

            // Set Binding Context for the CollectionViews (can also be done in XAML)
            AirQualityCollectionView.ItemsSource = AirQualityData;
            WaterQualityCollectionView.ItemsSource = WaterQualityData;
            WeatherCollectionView.ItemsSource = WeatherData;

            // Set EPPlus License context - VERY IMPORTANT for EPPlus v5+
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Or LicenseContext.Commercial if applicable

            // Set initial visibility based on default picker selection
            UpdateVisibleGrid();
        }

        private async void LoadDataButton_Clicked(object sender, EventArgs e)
        {
            if (DataTypePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Selection Required", "Please select a data type.", "OK");
                return;
            }

            string selectedType = DataTypePicker.SelectedItem.ToString();
            DateTime startDate = StartDatePicker.Date;
            DateTime endDate = EndDatePicker.Date.AddDays(1).AddTicks(-1); // Include the whole end day

            // Clear previous data and status
            AirQualityData.Clear();
            WaterQualityData.Clear();
            WeatherData.Clear();
            StatusLabel.Text = "";
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            UpdateVisibleGrid(); // Hide all grids initially

            try
            {
                StatusLabel.Text = $"Loading {selectedType} data...";
                switch (selectedType)
                {
                    case "Air Quality":
                        await LoadAirQualityDataAsync(startDate, endDate);
                        StatusLabel.Text = AirQualityData.Any() ? $"{AirQualityData.Count} Air Quality records loaded." : "No Air Quality data found for the selected period.";
                        break;
                    case "Water Quality":
                        await LoadWaterQualityDataAsync(startDate, endDate);
                        StatusLabel.Text = WaterQualityData.Any() ? $"{WaterQualityData.Count} Water Quality records loaded." : "No Water Quality data found for the selected period.";
                        break;
                    case "Weather":
                        await LoadWeatherDataAsync(startDate, endDate);
                        StatusLabel.Text = WeatherData.Any() ? $"{WeatherData.Count} Weather records loaded." : "No Weather data found for the selected period.";
                        break;
                }
                UpdateVisibleGrid(); // Show the correct grid
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Error loading data.";
                await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Data Loading Error: {ex}"); // Log detailed error
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private void DataTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optionally clear data or just update visibility when type changes
            // AirQualityData.Clear();
            // WaterQualityData.Clear();
            // WeatherData.Clear();
            // StatusLabel.Text = "Select data type and date range, then load data.";
            UpdateVisibleGrid();
        }

        private void UpdateVisibleGrid()
        {
            // Hide all grids first
            AirQualityGrid.IsVisible = false;
            WaterQualityGrid.IsVisible = false;
            WeatherGrid.IsVisible = false;

            if (DataTypePicker.SelectedIndex != -1)
            {
                string selectedType = DataTypePicker.SelectedItem.ToString();

                // Show the relevant grid based on selection (only if data exists for it)
                if (selectedType == "Air Quality" && AirQualityData.Any()) AirQualityGrid.IsVisible = true;
                else if (selectedType == "Water Quality" && WaterQualityData.Any()) WaterQualityGrid.IsVisible = true;
                else if (selectedType == "Weather" && WeatherData.Any()) WeatherGrid.IsVisible = true;
                // If no data loaded yet for the selected type, no grid will show, StatusLabel provides feedback
            }
        }

        private async Task LoadAirQualityDataAsync(DateTime startDate, DateTime endDate)
        {
            // Resource name format: DefaultNamespace.FolderPath.FileName
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Air_quality.xlsx"; // Adjust namespace/path if needed

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
                }

                using (var package = new ExcelPackage(stream))
                {
                    // Assuming data is on the first sheet (or adjust by name)
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Or use package.Workbook.Worksheets["SheetName"];
                    if (worksheet == null)
                    {
                        throw new Exception("No worksheet found in the Air Quality Excel file.");
                    }

                    // Find the actual start row for data (skip headers) - adjust based on your file
                    int startRow = 11; // Based on screenshot, data starts at row 11
                    int dateCol = 1;
                    int timeCol = 2;
                    int no2Col = 3;
                    int so2Col = 4;
                    int pm25Col = 5;
                    int pm10Col = 6;

                    // --- Get Site Name (Optional but useful context) ---
                    string siteName = worksheet.Cells["C3"].GetValue<string>() ?? "Unknown Site"; // Adjust cell ref if needed

                    for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                    {
                        try
                        {
                            // Read Date and Time - Excel stores them as doubles (OLE Automation date)
                            var dateVal = worksheet.Cells[row, dateCol].Value;
                            var timeVal = worksheet.Cells[row, timeCol].Value;

                            if (dateVal == null || timeVal == null) continue; // Skip row if no date/time

                            // Combine Date and Time from Excel's OLE Automation format
                            DateTime datePart = DateTime.FromOADate(Convert.ToDouble(dateVal));
                            DateTime timePart = DateTime.FromOADate(Convert.ToDouble(timeVal));
                            DateTime timestamp = datePart.Date + timePart.TimeOfDay;

                            // Apply Date Filter
                            if (timestamp < startDate || timestamp > endDate)
                            {
                                continue;
                            }

                            var reading = new AirQualityReading
                            {
                                Timestamp = timestamp,
                                SiteName = siteName,
                                NitrogenDioxide = GetNullableDouble(worksheet.Cells[row, no2Col].Value),
                                SulphurDioxide = GetNullableDouble(worksheet.Cells[row, so2Col].Value),
                                PM25 = GetNullableDouble(worksheet.Cells[row, pm25Col].Value),
                                PM10 = GetNullableDouble(worksheet.Cells[row, pm10Col].Value)
                            };
                            AirQualityData.Add(reading);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error parsing Air Quality row {row}: {ex.Message}");
                            // Optionally log or display an error for specific row failures
                        }
                    }
                }
            }
            await Task.CompletedTask; // Indicate async completion if no async file I/O was awaited
        }

        private async Task LoadWaterQualityDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Water_quality.xlsx"; // Adjust namespace/path

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) throw new Exception("No worksheet found in Water Quality file.");

                    int startRow = 6; // Data starts row 6
                    int dateCol = 1;
                    int timeCol = 2;
                    int nitrateCol = 3;
                    int nitriteCol = 4;
                    int phosphateCol = 5;
                    int ecCol = 6;

                    string siteName = worksheet.Cells["B1"].GetValue<string>() ?? "Unknown Site"; // Adjust if needed

                    for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                    {
                        try
                        {
                            var dateVal = worksheet.Cells[row, dateCol].Value;
                            var timeVal = worksheet.Cells[row, timeCol].Value;

                            if (dateVal == null || timeVal == null) continue;

                            DateTime datePart = DateTime.FromOADate(Convert.ToDouble(dateVal));
                            DateTime timePart = DateTime.FromOADate(Convert.ToDouble(timeVal));
                            DateTime timestamp = datePart.Date + timePart.TimeOfDay;

                            // Apply Date Filter
                            if (timestamp < startDate || timestamp > endDate) continue;

                            var reading = new WaterQualityReading
                            {
                                Timestamp = timestamp,
                                SiteName = siteName,
                                Nitrate = GetNullableDouble(worksheet.Cells[row, nitrateCol].Value),
                                Nitrite = GetNullableDouble(worksheet.Cells[row, nitriteCol].Value),
                                Phosphate = GetNullableDouble(worksheet.Cells[row, phosphateCol].Value),
                                EC = GetNullableDouble(worksheet.Cells[row, ecCol].Value) // EC might often be null here
                            };
                            WaterQualityData.Add(reading);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error parsing Water Quality row {row}: {ex.Message}");
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }


        private async Task LoadWeatherDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Weather.xlsx"; // Adjust namespace/path

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) throw new Exception("No worksheet found in Weather file.");

                    // --- Get Location (Optional context) ---
                    double lat = GetNullableDouble(worksheet.Cells["A2"].Value) ?? 0;
                    double lon = GetNullableDouble(worksheet.Cells["B2"].Value) ?? 0;

                    int startRow = 5; // Data starts row 5
                    int timestampCol = 1; // Combined ISO 8601 Timestamp
                    int tempCol = 2;
                    int humidityCol = 3;
                    int windSpeedCol = 4;
                    int windDirCol = 5;


                    for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                    {
                        try
                        {
                            var timestampStr = worksheet.Cells[row, timestampCol].GetValue<string>();
                            if (string.IsNullOrWhiteSpace(timestampStr)) continue;

                            // Parse ISO 8601 timestamp
                            if (DateTime.TryParse(timestampStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime timestamp))
                            {
                                // Apply Date Filter
                                if (timestamp < startDate || timestamp > endDate) continue;

                                var reading = new WeatherReading
                                {
                                    Timestamp = timestamp,
                                    Latitude = lat,
                                    Longitude = lon,
                                    Temperature = GetNullableDouble(worksheet.Cells[row, tempCol].Value),
                                    Humidity = GetNullableDouble(worksheet.Cells[row, humidityCol].Value),
                                    WindSpeed = GetNullableDouble(worksheet.Cells[row, windSpeedCol].Value),
                                    WindDirection = GetNullableDouble(worksheet.Cells[row, windDirCol].Value)
                                };
                                WeatherData.Add(reading);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Could not parse timestamp '{timestampStr}' in Weather file, row {row}");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error parsing Weather row {row}: {ex.Message}");
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }


        // Helper function to safely convert cell values to nullable double
        private double? GetNullableDouble(object cellValue)
        {
            if (cellValue == null || cellValue == DBNull.Value) return null;

            if (double.TryParse(cellValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            // Handle specific string cases like "No data" if necessary
            if (cellValue is string strVal && strVal.Trim().Equals("No data", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"Warning: Could not parse '{cellValue}' as double.");
            return null; // Return null if parsing fails
        }
    }
}