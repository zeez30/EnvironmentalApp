// AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using EnvironmentalApp.Data;

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents the database context for the EnvironmentalApp, using Entity Framework Core.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the DbSet for AirQualityData, representing the air quality readings table.
        /// </summary>
        public DbSet<AirQualityData> AirQualityDatas { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for MetaData, representing the metadata table.
        /// </summary>
        public DbSet<MetaData> MetaDatas { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for WaterQualityData, representing the water quality readings table.
        /// </summary>
        public DbSet<WaterQualityData> WaterQualityDatas { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for WeatherData, representing the weather data readings table.
        /// </summary>
        public DbSet<WeatherData> WeatherDatas { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for Sensor, representing the sensors table.
        /// </summary>
        public DbSet<Sensor> Sensors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class with the specified options.
        /// </summary>
        /// <param name="options">The options for configuring the database context.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Called to configure the database context.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify the options for the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Specify the path to the SQLite database file
            optionsBuilder.UseSqlite("Data Source=environmentaldata.db");  
        }
    }
}
