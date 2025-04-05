// AppDbContext.cs (create this file if it doesn't exist, put it in the Data folder)
using Microsoft.EntityFrameworkCore;
using EnvironmentalApp.Data;

namespace EnvironmentalApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AirQualityData> AirQualities { get; set; }
        public DbSet<MetaData> MetaDatas { get; set; }
        public DbSet<WaterQualityData> WaterQualityDatas { get; set; }
        public DbSet<WeatherData> WeatherDatas { get; set; }
        public DbSet<Sensor> Sensors { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "environmentaldata.db");
            System.Console.WriteLine($"Database Path (from constructor): {dbPath}");
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Specify the path to the SQLite database file
            optionsBuilder.UseSqlite("Data Source=nvironmentaldata.db");  // Change the filename if needed
        }
    }
}