// DatabaseService.cs
using Microsoft.EntityFrameworkCore;
using EnvironmentalApp.Data;
using ClosedXML.Excel;
using System.Reflection;
using System;

namespace EnvironmentalApp.Services
{
    /// <summary>
    /// Provides services for interacting with the database, including initialization and data import from Excel files.
    /// </summary>
    public class DatabaseService
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseService"/> class.
        /// </summary>
        /// <param name="dbContext">The application's database context, injected via dependency injection.</param>
        public DatabaseService(AppDbContext dbContext) // Inject the DbContext
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Asynchronously initializes the database by applying any pending migrations.
        /// This ensures the database schema is up-to-date.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitializeDatabaseAsync()
        {
            
            await _dbContext.Database.MigrateAsync(); // This will create the database and apply pending migrations
        }

        /// <summary>
        /// Asynchronously imports data from several Excel files into the corresponding database tables.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ImportExcelDataAsync()
        {
            await ImportAirQualityDataAsync("Air_quality.xlsx");
            await ImportMetaDataAsync("MetaData.xlsx");
            await ImportWaterQualityDataAsync("WaterQualityData.xlsx");
            await ImportWeatherDataAsync("WeatherData.xlsx");
        }

        /// <summary>
        /// Asynchronously imports air quality data from an Excel file into the AirQualityDatas table.
        /// </summary>
        /// <param name="fileName">The name of the Excel file to import.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ImportAirQualityDataAsync(string fileName)
        {
            try
            {
                // Get the assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Get the stream from the resource
                Stream stream = assembly.GetManifestResourceStream($"EnvironmentalApp.Resources.Raw.{fileName}");

                if (stream == null)
                {
                    Console.WriteLine($"Error: Resource stream not found for {fileName}");
                    return;
                }
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var lastRow = worksheet.LastRowUsed().RowNumber();
                    for (int row = 2; row <= lastRow; row++)
                    {
                        try
                        {
                            DateTime dateTime = worksheet.Cell(row, 1).GetDateTime();
                            double NitrogenDioxide = worksheet.Cell(row, 2).GetDouble();
                            double SulphurDioxide = worksheet.Cell(row, 3).GetDouble();
                            double PM2_5 = worksheet.Cell(row, 4).GetDouble();
                            double PM10 = worksheet.Cell(row, 5).GetDouble();


                            var airQualityData = new AirQualityData
                            {
                                DateTime = dateTime,
                                NitrogenDioxide = NitrogenDioxide,
                                SulphurDioxide = SulphurDioxide,
                                PM2_5 = PM2_5,
                                PM10 = PM10,

                            };
                            _dbContext.AirQualities.Add(airQualityData);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing row {row}: {ex.Message}");
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing {fileName}: {ex.Message}");
            }

        }

        /// <summary>
        /// Asynchronously imports metadata from an Excel file into the MetaDatas table.
        /// </summary>
        /// <param name="fileName">The name of the Excel file to import.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ImportMetaDataAsync(string fileName)
        {
            try
            {
                // Get the assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Get the stream from the resource
                Stream stream = assembly.GetManifestResourceStream($"EnvironmentalApp.Resources.Raw.{fileName}");

                if (stream == null)
                {
                    Console.WriteLine($"Error: Resource stream not found for {fileName}");
                    return;
                }

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var lastRow = worksheet.LastRowUsed().RowNumber();

                    for (int row = 2; row <= lastRow; row++)
                    {
                        try
                        {
                            // Read data from the cells
                            string category = worksheet.Cell(row, 1).GetString();        // Column A
                            string quantity = worksheet.Cell(row, 2).GetString();        // Column B
                            string symbol = worksheet.Cell(row, 3).GetString();          // Column C
                            string unit = worksheet.Cell(row, 4).GetString();            // Column D
                            string unitDescription = worksheet.Cell(row, 5).GetString(); // Column E
                            string measurementFrequency = worksheet.Cell(row, 6).GetString(); // Column F

                            // Read SafeLevel as nullable double
                            double? safeLevel = null; // Initialize to null
                            if (double.TryParse(worksheet.Cell(row, 7).Value.ToString(), out double parsedSafeLevel))
                            {
                                safeLevel = parsedSafeLevel;
                            }

                            string reference = worksheet.Cell(row, 8).GetString();        // Column H
                            string sensor = worksheet.Cell(row, 9).GetString();          // Column I
                            string url = worksheet.Cell(row, 10).GetString();            // Column J

                            // Create a new MetaData object
                            var metaData = new MetaData
                            {
                                Category = category,
                                Quantity = quantity,
                                Symbol = symbol,
                                Unit = unit,
                                UnitDescription = unitDescription,
                                MeasurementFrequency = measurementFrequency,
                                SafeLevel = safeLevel,
                                Reference = reference,
                                Sensor = sensor,
                                URL = url
                            };

                            // Add the object to the database context
                            _dbContext.MetaDatas.Add(metaData);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing MetaData row {row}: {ex.Message}");
                        }
                    }

                    // Save the changes to the database
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing MetaData: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously imports water quality data from an Excel file into the WaterQualityDatas table.
        /// </summary>
        /// <param name="fileName">The name of the Excel file to import.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ImportWaterQualityDataAsync(string fileName)
        {
            try
            {
                // Get the assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Get the stream from the resource
                Stream stream = assembly.GetManifestResourceStream($"EnvironmentalApp.Resources.Raw.{fileName}");

                if (stream == null)
                {
                    Console.WriteLine($"Error: Resource stream not found for {fileName}");
                    return;
                }

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var lastRow = worksheet.LastRowUsed().RowNumber();

                    // Find the row where the actual data starts.
                    int startRow = 0;
                    for (int row = 1; row <= lastRow; row++)
                    {
                        if (worksheet.Cell(row, 1).Value.ToString().ToLower() == "date") // Look for the "Date" column header
                        {
                            startRow = row + 1; // Data starts on the row after the header
                            break;
                        }
                    }

                    if (startRow == 0)
                    {
                        Console.WriteLine("Error: Could not find data start row (Date column header).");
                        return;
                    }

                    for (int row = startRow; row <= lastRow; row++)
                    {
                        try
                        {
                            // Read data from the cells
                            DateTime date = worksheet.Cell(row, 1).GetDateTime(); // Column A

                            // Handle the Time column
                            TimeSpan time;
                            if (TimeSpan.TryParse(worksheet.Cell(row, 2).Value.ToString(), out time))
                            {
                                // Parsing successful: the excel cell is actually a time.
                            }
                            else
                            {
                                // Parsing unsuccessful: get the data as a string, if needed.
                                Console.WriteLine($"Failed to parse Time {worksheet.Cell(row, 2).Value.ToString()} to TimeSpan.");
                                time = new TimeSpan(0, 0, 0);
                            }

                            double nitrate = worksheet.Cell(row, 3).GetDouble();    // Column C
                            double nitrite = worksheet.Cell(row, 4).GetDouble();    // Column D
                            double phosphate = worksheet.Cell(row, 5).GetDouble();  // Column E
                            double ec = worksheet.Cell(row, 6).GetDouble();        // Column F

                            // Create a new WaterQualityData object
                            var waterQualityData = new WaterQualityData
                            {
                                Date = date,
                                Time = time,
                                Nitrate = nitrate,
                                Nitrite = nitrite,
                                Phosphate = phosphate,
                                EC = ec
                            };

                            // Add the object to the database context
                            _dbContext.WaterQualityDatas.Add(waterQualityData);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing WaterQualityData row {row}: {ex.Message}");
                        }
                    }

                    // Save the changes to the database
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing WaterQualityData: {ex.Message}");
            }
        }

        /// <summary>
        /// Asynchronously imports weather data from an Excel file into the WeatherDatas table.
        /// </summary>
        /// <param name="fileName">The name of the Excel file to import.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task ImportWeatherDataAsync(string fileName)
        {
            try
            {
                // Get the assembly
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Get the stream from the resource
                Stream stream = assembly.GetManifestResourceStream($"EnvironmentalApp.Resources.Raw.{fileName}");

                if (stream == null)
                {
                    Console.WriteLine($"Error: Resource stream not found for {fileName}");
                    return;
                }

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var lastRow = worksheet.LastRowUsed().RowNumber();

                    // Find the row where the actual data starts (look for "time" header)
                    int startRow = 0;
                    for (int row = 1; row <= lastRow; row++)
                    {
                        if (worksheet.Cell(row, 1).Value.ToString().ToLower() == "time") // Check for the "time" header
                        {
                            startRow = row + 1; // Data starts on the row after the header
                            break;
                        }
                    }

                    if (startRow == 0)
                    {
                        Console.WriteLine("Error: Could not find data start row (time column header).");
                        return;
                    }

                    for (int row = startRow; row <= lastRow; row++)
                    {
                        try
                        {
                            // Read data from the cells
                            DateTime time = worksheet.Cell(row, 1).GetDateTime(); // Column A

                            double temperature = worksheet.Cell(row, 2).GetDouble(); // Column B
                            double relativeHumidity = worksheet.Cell(row, 3).GetDouble(); // Column C
                            double windSpeed = worksheet.Cell(row, 4).GetDouble(); // Column D
                            double windDirection = worksheet.Cell(row, 5).GetDouble(); // Column E

                            // Create a new WeatherData object
                            var weatherData = new WeatherData
                            {
                                Time = time,
                                Temperature = temperature,
                                RelativeHumidity = relativeHumidity,
                                WindSpeed = windSpeed,
                                WindDirection = windDirection
                            };

                            // Add the object to the database context
                            _dbContext.WeatherDatas.Add(weatherData);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing WeatherData row {row}: {ex.Message}");
                        }
                    }

                    // Save the changes to the database
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing WeatherData: {ex.Message}");
            }
        }
    }
}
