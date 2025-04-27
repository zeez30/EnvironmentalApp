// --- START OF FILE DataAnalysisPage.xaml.cs ---

// File: DataAnalysisPage.xaml.cs
using EnvironmentalApp.Data;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ExcelDataReader; // Required for reading Excel files
using System.Data;     // Required for DataSet operations with ExcelDataReader
using System.Linq;     // Required for LINQ operations like Any()

namespace EnvironmentalApp
{
    /// <summary>
    /// Represents the code-behind for the Data Analysis page.
    /// Allows users (like Environmental Scientists) to load and view historical environmental data
    /// (Air Quality, Water Quality, Weather) from embedded Excel files based on selected criteria (data type, date range).
    /// This addresses the user story: "View and analyse historical environmental data".
    /// </summary>
    public partial class DataAnalysisPage : ContentPage
    {
        /// <summary>
        /// Collection holding the loaded Air Quality data for the selected period. Bound to the AirQualityCollectionView.
        /// </summary>
        public ObservableCollection<AirQualityReading> AirQualityData { get; set; }
        /// <summary>
        /// Collection holding the loaded Water Quality data for the selected period. Bound to the WaterQualityCollectionView.
        /// </summary>
        public ObservableCollection<WaterQualityReading> WaterQualityData { get; set; }
        /// <summary>
        /// Collection holding the loaded Weather data for the selected period. Bound to the WeatherCollectionView.
        /// </summary>
        public ObservableCollection<WeatherReading> WeatherData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnalysisPage"/> class.
        /// Sets up data collections, default UI states, and data bindings.
        /// </summary>
        public DataAnalysisPage()
        {
            InitializeComponent();

            // Initialize data collections
            AirQualityData = new ObservableCollection<AirQualityReading>();
            WaterQualityData = new ObservableCollection<WaterQualityReading>();
            WeatherData = new ObservableCollection<WeatherReading>();

            // Set default selection for the data type picker
            DataTypePicker.SelectedIndex = 0; // Default to "Air Quality"

            // Bind collections to the respective CollectionView controls in XAML
            AirQualityCollectionView.ItemsSource = AirQualityData;
            WaterQualityCollectionView.ItemsSource = WaterQualityData;
            WeatherCollectionView.ItemsSource = WeatherData;

            // Set initial visibility of data grids (none visible until data is loaded)
            UpdateVisibleGrid();
        }

        /// <summary>
        /// Handles the click event for the 'Load Historical Data' button.
        /// Validates user selections (data type, dates).
        /// Clears previous data, shows a loading indicator, and calls the appropriate asynchronous data loading method
        /// based on the selected data type. Updates the status label and grid visibility upon completion or error.
        /// </summary>
        /// <param name="sender">The button object.</param>
        /// <param name="e">Event arguments.</param>
        private async void LoadDataButton_Clicked(object sender, EventArgs e)
        {
            if (DataTypePicker.SelectedIndex == -1)
            {
                await DisplayAlert("Selection Required", "Please select a data type.", "OK");
                return;
            }

            string selectedType = DataTypePicker.SelectedItem.ToString();
            DateTime startDate = StartDatePicker.Date;
            // Ensure the end date includes the entire day selected
            DateTime endDate = EndDatePicker.Date.AddDays(1).AddTicks(-1);

            // Validate date range
            if (endDate < startDate)
            {
                await DisplayAlert("Invalid Date Range", "The end date cannot be before the start date.", "OK");
                return;
            }


            // Clear previous data and reset UI state
            AirQualityData.Clear();
            WaterQualityData.Clear();
            WeatherData.Clear();
            StatusLabel.Text = ""; // Clear status message
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            UpdateVisibleGrid(); // Hide all grids initially

            try
            {
                StatusLabel.Text = $"Loading {selectedType} data...";
                // Load data based on selected type
                switch (selectedType)
                {
                    case "Air Quality":
                        await LoadAirQualityDataAsync(startDate, endDate);
                        // Update status based on whether data was found
                        StatusLabel.Text = AirQualityData.Any()
                            ? $"{AirQualityData.Count} Air Quality records loaded for {startDate:yyyy-MM-dd} to {EndDatePicker.Date:yyyy-MM-dd}."
                            : "No Air Quality data found for the selected period.";
                        break;
                    case "Water Quality":
                        await LoadWaterQualityDataAsync(startDate, endDate);
                        StatusLabel.Text = WaterQualityData.Any()
                            ? $"{WaterQualityData.Count} Water Quality records loaded for {startDate:yyyy-MM-dd} to {EndDatePicker.Date:yyyy-MM-dd}."
                            : "No Water Quality data found for the selected period.";
                        break;
                    case "Weather":
                        await LoadWeatherDataAsync(startDate, endDate);
                        StatusLabel.Text = WeatherData.Any()
                            ? $"{WeatherData.Count} Weather records loaded for {startDate:yyyy-MM-dd} to {EndDatePicker.Date:yyyy-MM-dd}."
                            : "No Weather data found for the selected period.";
                        break;
                    default:
                        StatusLabel.Text = "Invalid data type selected.";
                        break;
                }
                UpdateVisibleGrid(); // Show the relevant grid if data was loaded
            }
            catch (FileNotFoundException fnfEx)
            {
                StatusLabel.Text = "Error loading data file.";
                await DisplayAlert("Error", $"Could not find the required data file: {fnfEx.Message}. Ensure it's set as an EmbeddedResource.", "OK");
                System.Diagnostics.Debug.WriteLine($"Data Loading File Not Found Error: {fnfEx}");
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "An error occurred while loading data.";
                await DisplayAlert("Error", $"Failed to load or process data: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Data Loading Error: {ex}");
            }
            finally
            {
                // Stop and hide the loading indicator regardless of outcome
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        /// <summary>
        /// Handles the selection change event for the DataTypePicker.
        /// Updates the visibility of the data grids to show the grid corresponding
        /// to the newly selected data type (if data for it has already been loaded).
        /// </summary>
        /// <param name="sender">The picker object.</param>
        /// <param name="e">Event arguments.</param>
        private void DataTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateVisibleGrid();
        }

        /// <summary>
        /// Updates the visibility of the Air Quality, Water Quality, and Weather data grids.
        /// Only the grid corresponding to the selected data type in the DataTypePicker AND
        /// which currently contains data will be made visible. All others are hidden.
        /// </summary>
        private void UpdateVisibleGrid()
        {
            // Hide all grids by default
            AirQualityGrid.IsVisible = false;
            WaterQualityGrid.IsVisible = false;
            WeatherGrid.IsVisible = false;

            // Determine which grid to show based on picker selection and data availability
            if (DataTypePicker.SelectedIndex != -1)
            {
                string selectedType = DataTypePicker.SelectedItem.ToString();
                if (selectedType == "Air Quality" && AirQualityData.Any()) AirQualityGrid.IsVisible = true;
                else if (selectedType == "Water Quality" && WaterQualityData.Any()) WaterQualityGrid.IsVisible = true;
                else if (selectedType == "Weather" && WeatherData.Any()) WeatherGrid.IsVisible = true;
            }
        }

        // --- Asynchronous Data Loading Methods ---

        /// <summary>
        /// Loads Air Quality data asynchronously from the embedded Excel resource file.
        /// Uses ExcelDataReader to parse the file, handling specific sheet structure (header row, data start).
        /// Filters data based on the provided start and end dates.
        /// Populates the <see cref="AirQualityData"/> collection.
        /// </summary>
        /// <param name="startDate">The start date for filtering data (inclusive).</param>
        /// <param name="endDate">The end date for filtering data (inclusive).</param>
        /// <returns>A task representing the asynchronous loading operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the embedded resource stream cannot be found.</exception>
        /// <exception cref="Exception">Thrown if reading the Excel file fails (e.g., reaching end before header).</exception>
        private async Task LoadAirQualityDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // Ensure the resource name matches the actual embedded resource path
            var resourceName = "EnvironmentalApp.Resources.Raw.Air_quality.xlsx";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found. Check Build Action is 'EmbeddedResource'.");

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance); // Ensure required encoding is registered
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    AirQualityData.Clear(); // Clear existing data before loading new
                    string siteName = "Edinburgh Nicolson Street"; // Site name assumed from file context

                    // --- Manual Reading Logic for Air Quality File Structure ---
                    // This file has metadata before the header (header is row 10).
                    int headerRowInSheet = 10; // 1-based index in the Excel sheet

                    // 1. Skip rows before the header row
                    for (int i = 1; i < headerRowInSheet; i++)
                    {
                        if (!reader.Read()) throw new Exception($"Reached end of file before finding header row {headerRowInSheet} in {resourceName}.");
                    }

                    // 2. Read the header row itself (row 10) - we don't strictly need the values if we know the column indices
                    if (!reader.Read()) throw new Exception($"Could not read expected header row {headerRowInSheet} in {resourceName}.");
                    // Column indices based on the expected structure (0-based for the reader)
                    const int dateCol = 0;    // Column A: Date
                    const int timeCol = 1;    // Column B: Time
                    const int no2Col = 2;     // Column C: NO2 ug/m3
                    const int so2Col = 3;     // Column D: SO2 ug/m3
                    const int pm25Col = 4;    // Column E: PM2.5 ug/m3
                    const int pm10Col = 5;    // Column F: PM10 ug/m3

                    // 3. Read data rows (starting from row 11)
                    while (reader.Read()) // Read the next row
                    {
                        try
                        {
                            var dateVal = reader.GetValue(dateCol);
                            var timeVal = reader.GetValue(timeCol);

                            // Basic validation: Skip row if date or time is missing
                            if (dateVal == null || timeVal == null) continue;

                            // Attempt to parse date and time. ExcelDataReader might return DateTime, double (OADate), or string.
                            DateTime datePart;
                            DateTime timePart;

                            // Parse Date
                            if (dateVal is DateTime dVal) datePart = dVal.Date;
                            else if (dateVal is double dateOAD) datePart = DateTime.FromOADate(dateOAD).Date;
                            else if (DateTime.TryParse(dateVal.ToString(), out DateTime parsedDate)) datePart = parsedDate.Date;
                            else continue; // Skip row if date cannot be parsed

                            // Parse Time
                            if (timeVal is DateTime tVal) timePart = tVal; // May include date part, extract time later
                            else if (timeVal is double timeOAD) timePart = DateTime.FromOADate(timeOAD);
                            else if (DateTime.TryParse(timeVal.ToString(), out DateTime parsedTime)) timePart = parsedTime;
                            else continue; // Skip row if time cannot be parsed

                            // Combine date and time parts into a single timestamp
                            DateTime timestamp = datePart.Date + timePart.TimeOfDay;

                            // Apply Date Filter: Skip if outside the selected range
                            if (timestamp < startDate || timestamp > endDate) continue;

                            // Create reading object and parse numeric values
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
                            // Log error for the specific row but continue processing others
                            System.Diagnostics.Debug.WriteLine($"Error processing Air Quality row: {ex.Message}. Row Data: [Date: {reader.GetValue(dateCol)}, Time: {reader.GetValue(timeCol)}, ...]");
                        }
                    }
                }
            }
            // Ensure the operation is awaitable, even if purely synchronous file reading was done.
            await Task.CompletedTask;
        }


        /// <summary>
        /// Loads Water Quality data asynchronously from the embedded Excel resource file.
        /// Uses ExcelDataReader, handling specific sheet structure (header row 5).
        /// Filters data by date and populates the <see cref="WaterQualityData"/> collection.
        /// </summary>
        /// <param name="startDate">The start date for filtering data (inclusive).</param>
        /// <param name="endDate">The end date for filtering data (inclusive).</param>
        /// <returns>A task representing the asynchronous loading operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the embedded resource stream cannot be found.</exception>
        /// <exception cref="Exception">Thrown if reading the Excel file fails.</exception>
        private async Task LoadWaterQualityDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Water_quality.xlsx"; // Match embedded resource path

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found. Check Build Action.");

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    WaterQualityData.Clear();
                    string siteName = "Glencorse B"; // Assumed site name

                    // --- Manual Reading for Water Quality File ---
                    int headerRowInSheet = 5; // Header is row 5 (1-based)

                    // 1. Skip rows before header
                    for (int i = 1; i < headerRowInSheet; i++)
                    {
                        if (!reader.Read()) throw new Exception($"Reached end of file before finding header row {headerRowInSheet} in {resourceName}.");
                    }
                    // 2. Read header row
                    if (!reader.Read()) throw new Exception($"Could not read expected header row {headerRowInSheet} in {resourceName}.");
                    // Column indices (0-based)
                    const int dateCol = 0;      // Column A: Date
                    const int timeCol = 1;      // Column B: Time
                    const int nitrateCol = 2;   // Column C: Nitrate mg/l
                    const int nitriteCol = 3;   // Column D: Nitrite mg/l
                    const int phosphateCol = 4; // Column E: Phosphate mg/l
                    const int ecCol = 5;        // Column F: EC cfu/100ml (likely E. coli, not Electrical Conductivity based on unit)

                    // 3. Read data rows
                    while (reader.Read())
                    {
                        try
                        {
                            var dateVal = reader.GetValue(dateCol);
                            var timeVal = reader.GetValue(timeCol);

                            if (dateVal == null || timeVal == null) continue;

                            DateTime datePart;
                            DateTime timePart;

                            // Parse Date
                            if (dateVal is DateTime dVal) datePart = dVal.Date;
                            else if (dateVal is double dateOAD) datePart = DateTime.FromOADate(dateOAD).Date;
                            else if (DateTime.TryParse(dateVal.ToString(), out DateTime parsedDate)) datePart = parsedDate.Date;
                            else continue;

                            // Parse Time
                            if (timeVal is DateTime tVal) timePart = tVal;
                            else if (timeVal is double timeOAD) timePart = DateTime.FromOADate(timeOAD);
                            else if (DateTime.TryParse(timeVal.ToString(), out DateTime parsedTime)) timePart = parsedTime;
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
                                EC = GetNullableDouble(reader.GetValue(ecCol)) // E. coli count
                            };
                            WaterQualityData.Add(reading);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error processing Water Quality row: {ex.Message}. Row Data: [Date: {reader.GetValue(dateCol)}, Time: {reader.GetValue(timeCol)}, ...]");
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// Loads Weather data asynchronously from the embedded Excel resource file.
        /// Uses ExcelDataReader, reading metadata (lat/lon) first, then skipping headers to read data rows.
        /// Handles potential ISO8601 or OLE Automation date formats for timestamps.
        /// Filters data by date and populates the <see cref="WeatherData"/> collection.
        /// </summary>
        /// <param name="startDate">The start date for filtering data (inclusive).</param>
        /// <param name="endDate">The end date for filtering data (inclusive).</param>
        /// <returns>A task representing the asynchronous loading operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the embedded resource stream cannot be found.</exception>
        private async Task LoadWeatherDataAsync(DateTime startDate, DateTime endDate)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "EnvironmentalApp.Resources.Raw.Weather.xlsx"; // Adjust namespace/path if needed

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) throw new FileNotFoundException($"Embedded resource '{resourceName}' not found. Check Build Action.");

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    WeatherData.Clear();

                    // --- Manual Reading for Weather File Structure ---
                    // 1. Read metadata (Latitude, Longitude)
                    if (!reader.Read()) return; // Skip header row 1: latitude, longitude labels
                    if (!reader.Read()) return; // Read data row 2: values
                    double lat = GetNullableDouble(reader.GetValue(0)) ?? 0.0; // Column A: latitude
                    double lon = GetNullableDouble(reader.GetValue(1)) ?? 0.0; // Column B: longitude

                    // 2. Skip rows until the data header row
                    if (!reader.Read()) return; // Skip empty row 3
                    if (!reader.Read()) return; // Skip header row 4: time, temperature_2m... labels

                    // Column indices (0-based) for data starting row 5
                    const int timestampCol = 0;   // Column A: time (ISO8601 string or OADate)
                    const int tempCol = 1;        // Column B: temperature_2m (°C)
                    const int humidityCol = 2;    // Column C: relative_humidity_2m (%)
                    const int windSpeedCol = 3;   // Column D: wind_speed_10m (m/s)
                    const int windDirCol = 4;     // Column E: wind_direction_10m (°)

                    // 3. Read actual data rows (row 5 onwards)
                    while (reader.Read())
                    {
                        try
                        {
                            var timestampObj = reader.GetValue(timestampCol);
                            if (timestampObj == null) continue; // Skip row if timestamp is missing

                            DateTime timestamp;
                            // Attempt to parse timestamp - could be OLE Automation Date (double) or ISO string
                            if (timestampObj is DateTime dt)
                            {
                                timestamp = dt; // Already a DateTime
                            }
                            else if (timestampObj is double oaDate) // Check for OADate first
                            {
                                timestamp = DateTime.FromOADate(oaDate);
                            }
                            // Try parsing as ISO 8601 string (common in APIs/exports)
                            else if (DateTime.TryParse(timestampObj.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out DateTime parsedDt))
                            {
                                timestamp = parsedDt;
                            }
                            else
                            {
                                // Log and skip if timestamp cannot be parsed
                                System.Diagnostics.Debug.WriteLine($"Skipping Weather row due to unparseable timestamp: '{timestampObj}'");
                                continue;
                            }

                            // Apply Date Filter
                            if (timestamp < startDate || timestamp > endDate) continue;

                            // Create reading object
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
                            System.Diagnostics.Debug.WriteLine($"Error processing Weather row: {ex.Message}. Row Data: [Timestamp: {reader.GetValue(timestampCol)}, ...]");
                        }
                    }
                }
            }
            await Task.CompletedTask;
        }


        /// <summary>
        /// Helper function to safely parse a cell value from ExcelDataReader into a nullable double.
        /// Handles null, DBNull, existing doubles, string representations of numbers (using InvariantCulture),
        /// and specific string values like "No data". Logs a warning if parsing fails unexpectedly.
        /// </summary>
        /// <param name="cellValue">The object value read from the Excel cell.</param>
        /// <returns>A nullable double representing the parsed value, or null if parsing failed or the value indicates no data.</returns>
        private double? GetNullableDouble(object cellValue)
        {
            if (cellValue == null || cellValue == DBNull.Value) return null;

            // If it's already a double, return it directly
            if (cellValue is double dbl) return dbl;

            // Try parsing the string representation using invariant culture
            if (double.TryParse(cellValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            // Handle specific text indicating missing data (case-insensitive)
            if (cellValue is string strVal && strVal.Trim().Equals("No data", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Log a warning if parsing failed for an unexpected value
            System.Diagnostics.Debug.WriteLine($"Warning: Could not parse '{cellValue}' (Type: {cellValue.GetType()}) as double in GetNullableDouble.");
            return null; // Return null if parsing fails
        }
    }
}
// --- END OF FILE DataAnalysisPage.xaml.cs ---