using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bcl.AsyncInterfaces;
using OfficeOpenXml;

namespace EnvironmentalApp.Data
{
    public class DatabaseService
    {
        static SQLiteAsyncConnection Database;
        public static readonly AsyncLazy<DatabaseService> Instance =
            new AsyncLazy<DatabaseService>(async () =>
            {
                var instance = new DatabaseService();
                await instance.InitializeDatabase();
                return instance;
            });

        public DatabaseService()
        {
        }

        async Task InitializeDatabase()
        {
            if (Database != null)
                return;

            Database = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "AirQuality.db3"));
            await Database.CreateTableAsync<AirQualityData>();
        }

        public async Task ImportAirQualityDataFromExcel(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                for (int row = 11; row <= rowCount; row++)
                {
                    string dateStr = worksheet.Cells[row, 1].GetValue<string>();
                    string timeStr = worksheet.Cells[row, 2].GetValue<string>();
                    DateTime dateTime = DateTime.Parse(dateStr + " " + timeStr);

                    AirQualityData data = new AirQualityData
                    {
                        DateTime = dateTime,
                        NitrogenDioxide = worksheet.Cells[row, 3].GetValue<double>(),
                        SulphurDioxide = worksheet.Cells[row, 4].GetValue<double>(),
                        PM2_5 = worksheet.Cells[row, 5].GetValue<double>(),
                        PM10 = worksheet.Cells[row, 6].GetValue<double>()
                    };

                    await Database.InsertAsync(data);
                }
            }
        }

        public async Task<List<AirQualityData>> GetAirQualityDataAsync()
        {
            await InitializeDatabase();
            return await Database.Table<AirQualityData>().ToListAsync();
        }

        
    }
}