// File: DataAnalysisPage.xaml.cs
using EnvironmentalApp.Data;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ExcelDataReader; // <-- Add this
using System.Data;     // <-- Add this
using System.Linq;

namespace EnvironmentalApp
{
    public partial class DataAnalysisPage : ContentPage
    {
        public ObservableCollection<AirQualityReading> AirQualityData { get; set; }
        public ObservableCollection<WaterQualityReading> WaterQualityData { get; set; }
        public ObservableCollection<WeatherReading> WeatherData { get; set; }

        public DataAnalysisPage()
        {
            InitializeComponent();

            AirQualityData = new ObservableCollection<AirQualityReading>();
            WaterQualityData = new ObservableCollection<WaterQualityReading>();
            WeatherData = new ObservableCollection<WeatherReading>();

            DataTypePicker.SelectedIndex = 0;

            AirQualityCollectionView.ItemsSource = AirQualityData;
            WaterQualityCollectionView.ItemsSource = WaterQualityData;
            WeatherCollectionView.ItemsSource = WeatherData;

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

            AirQualityData.Clear();
            WaterQualityData.Clear();
            WeatherData.Clear();
            StatusLabel.Text = "";
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            UpdateVisibleGrid();

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
                UpdateVisibleGrid();
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Error loading data.";
                await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Data Loading Error: {ex}");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        private void DataTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisibleGrid();
        }

        private void UpdateVisibleGrid()
        {
            AirQualityGrid.IsVisible = false;
            WaterQualityGrid.IsVisible = false;
            WeatherGrid.IsVisible = false;

            if (DataTypePicker.SelectedIndex != -1)
            {
                string selectedType = DataTypePicker.SelectedItem.ToString();
                if (selectedType == "Air Quality" && AirQualityData.Any()) AirQualityGrid.IsVisible = true;
                else if (selectedType == "Water Quality" && WaterQualityData.Any()) WaterQualityGrid.IsVisible = true;
                else if (selectedType == "Weather" && WeatherData.Any()) WeatherGrid.IsVisible = true;
            }
        }

        // --- Load methods using ExcelDataReader ---

        private async Task LoadAirQualityDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Air_quality.xlsx";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Use AsDataSet, telling it the first row it reads should be the header
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true // Let it use the first row it reads as header
                                                // ** REMOVED HeaderRowIndex **
                        }
                    });

                    var dataTable = result.Tables[0];
                    string siteName = "Edinburgh Nicolson Street"; // Assume known

                    // ** ADJUSTMENT: Skip initial rows MANUALLY if AsDataSet doesn't handle the offset **
                    // Header is row 10. AsDataSet likely used row 1 as header.
                    // Data starts row 11. The DataTable rows collection will contain data from row 2 onwards.
                    // We need to effectively skip rows 2-10 from the original sheet's perspective,
                    // which corresponds to rows 0-8 in the dataTable.Rows collection IF the header was row 1.
                    // --> Since we *know* the real header is row 10, and data starts row 11,
                    // we need to find the index where the actual data begins.
                    // Let's find the header row manually first to get column indices reliably.

                    // Find the actual header row (row 10 in the sheet)
                    int headerRowInSheet = 10; // 1-based index in Excel sheet
                    DataRow actualHeaderRow = null;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        // Check if the content matches expected header values (crude but effective for known structure)
                        // Compare based on 1-based index from sheet. dataTable.Rows index is 0-based and starts after header row used by AsDataSet.
                        // This logic is getting complex. Let's revert to manual reading for Air/Water.

                        // *** REVERTING TO MANUAL READ FOR AIR/WATER ***
                        // AsDataSet struggles when headers aren't near the top.

                        AirQualityData.Clear(); // Clear again before manual add
                        reader.Reset(); // Reset reader position to the start

                        // Manually skip rows before the header
                        for (int skipIndex = 1; i < headerRowInSheet; i++)
                        {
                            if (!reader.Read()) throw new Exception("Reached end of file before finding header row.");
                        }

                        // Read the header row itself (row 10)
                        if (!reader.Read()) throw new Exception("Could not read header row.");
                        // We can optionally store header names here if needed, but indices are fine too.

                        // Column indices (0-based for reader)
                        int dateCol = 0; // Column A
                        int timeCol = 1; // Column B
                        int no2Col = 2;
                        int so2Col = 3;
                        int pm25Col = 4;
                        int pm10Col = 5;

                        // Now read the data rows (row 11 onwards)
                        while (reader.Read())
                        {
                            try
                            {
                                var dateVal = reader.GetValue(dateCol);
                                var timeVal = reader.GetValue(timeCol);

                                if (dateVal == null || timeVal == null) continue;

                                DateTime datePart;
                                DateTime timePart;

                                // Handle potential OLE Automation dates if standard parsing fails
                                if (dateVal is DateTime dVal) datePart = dVal;
                                else if (double.TryParse(dateVal.ToString(), out double dateOAD)) datePart = DateTime.FromOADate(dateOAD);
                                else continue; // Skip row if date cannot be parsed

                                if (timeVal is DateTime tVal) timePart = tVal; // Might contain full date, take only time
                                else if (double.TryParse(timeVal.ToString(), out double timeOAD)) timePart = DateTime.FromOADate(timeOAD);
                                else continue; // Skip row if time cannot be parsed

                                DateTime timestamp = datePart.Date + timePart.TimeOfDay;

                                // Apply Date Filter
                                if (timestamp < startDate || timestamp > endDate) continue;

                                var reading = new AirQualityReading
                                {
                                    Timestamp = timestamp,
                                    SiteName = siteName,
                                    NitrogenDioxide = GetNullableDouble(reader.GetValue(no2Col)),
                                    SulphurDioxide = GetNullableDouble(reader.GetValue(so2Col)),
                                    PM25 = GetNullableDouble(reader.GetValue(pm25Col)),
                                    PM10 = GetNullableDouble(reader.GetValue(pm10Col))
                                };
                                AirQualityData.Add(reading);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error processing Air Quality row: {ex.Message}");
                            }
                        }
                        break; // Exit outer loop once manual reading is done
                    }
                }
            }
            await Task.CompletedTask;
        }

        private async Task LoadWaterQualityDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Water_quality.xlsx";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    WaterQualityData.Clear(); // Clear before manual add
                    string siteName = "Glencorse B"; // Assume known

                    int headerRowInSheet = 5; // Header is row 5

                    // Manually skip rows before the header
                    for (int i = 1; i < headerRowInSheet; i++)
                    {
                        if (!reader.Read()) throw new Exception("Reached end of file before finding header row.");
                    }
                    // Read the header row itself (row 5)
                    if (!reader.Read()) throw new Exception("Could not read header row.");

                    // Column indices (0-based for reader)
                    int dateCol = 0;
                    int timeCol = 1;
                    int nitrateCol = 2;
                    int nitriteCol = 3;
                    int phosphateCol = 4;
                    int ecCol = 5;

                    // Now read the data rows (row 6 onwards)
                    while (reader.Read())
                    {
                        try
                        {
                            var dateVal = reader.GetValue(dateCol);
                            var timeVal = reader.GetValue(timeCol);

                            if (dateVal == null || timeVal == null) continue;

                            DateTime datePart;
                            DateTime timePart;

                            if (dateVal is DateTime dVal) datePart = dVal;
                            else if (double.TryParse(dateVal.ToString(), out double dateOAD)) datePart = DateTime.FromOADate(dateOAD);
                            else continue;

                            if (timeVal is DateTime tVal) timePart = tVal;
                            else if (double.TryParse(timeVal.ToString(), out double timeOAD)) timePart = DateTime.FromOADate(timeOAD);
                            else continue;

                            DateTime timestamp = datePart.Date + timePart.TimeOfDay;

                            // Apply Date Filter
                            if (timestamp < startDate || timestamp > endDate) continue;

                            var reading = new WaterQualityReading
                            {
                                Timestamp = timestamp,
                                SiteName = siteName,
                                Nitrate = GetNullableDouble(reader.GetValue(nitrateCol)),
                                Nitrite = GetNullableDouble(reader.GetValue(nitriteCol)),
                                Phosphate = GetNullableDouble(reader.GetValue(phosphateCol)),
                                EC = GetNullableDouble(reader.GetValue(ecCol))
                            };
                            WaterQualityData.Add(reading);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing Water Quality row: {ex.Message}");
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }

        // LoadWeatherDataAsync can remain as it was (reading metadata then looping)
        private async Task LoadWeatherDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Weather.xlsx"; // Adjust namespace/path

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    WeatherData.Clear(); // Clear before adding

                    // Weather file has metadata, then headers, then data.
                    // Read metadata first manually
                    reader.Read(); // Skip header 1: latitude etc labels
                    reader.Read(); // Read data row 2: lat/lon values
                    double lat = GetNullableDouble(reader.GetValue(0)) ?? 0; // Column A
                    double lon = GetNullableDouble(reader.GetValue(1)) ?? 0; // Column B
                    reader.Read(); // Skip empty row 3
                    reader.Read(); // Skip header row 4: time, temp labels etc.

                    // Column indices (0-based for reader)
                    int timestampCol = 0;
                    int tempCol = 1;
                    int humidityCol = 2;
                    int windSpeedCol = 3;
                    int windDirCol = 4;

                    // Now read the actual data rows (row 5 onwards)
                    while (reader.Read())
                    {
                        try
                        {
                            var timestampObj = reader.GetValue(timestampCol); // Column A: Timestamp string
                            if (timestampObj == null) continue;

                            DateTime timestamp;
                            if (timestampObj is DateTime dt)
                            {
                                timestamp = dt;
                            }
                            else if (double.TryParse(timestampObj.ToString(), out double oaDate)) // Check if OLE Automation date first
                            {
                                timestamp = DateTime.FromOADate(oaDate);
                            }
                            else if (DateTime.TryParse(timestampObj.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime parsedDt)) // Fallback to ISO string parsing
                            {
                                timestamp = parsedDt;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Skipping Weather row due to unparseable timestamp: {timestampObj}");
                                continue;
                            }

                            // Apply Date Filter
                            if (timestamp < startDate || timestamp > endDate) continue;

                            var reading = new WeatherReading
                            {
                                Timestamp = timestamp,
                                Latitude = lat,
                                Longitude = lon,
                                Temperature = GetNullableDouble(reader.GetValue(tempCol)),
                                Humidity = GetNullableDouble(reader.GetValue(humidityCol)),
                                WindSpeed = GetNullableDouble(reader.GetValue(windSpeedCol)),
                                WindDirection = GetNullableDouble(reader.GetValue(windDirCol))
                            };
                            WeatherData.Add(reading);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing Weather row: {ex.Message}");
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }


        // Helper function remains the same
        // Helper function remains the same
        private double? GetNullableDouble(object cellValue)
        {
            if (cellValue == null || cellValue == DBNull.Value) return null;
            if (cellValue is double dbl) return dbl;
            if (double.TryParse(cellValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            if (cellValue is string strVal && strVal.Trim().Equals("No data", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            System.Diagnostics.Debug.WriteLine($"Warning: Could not parse '{cellValue}' as double.");
            return null;
        }
    }
}
