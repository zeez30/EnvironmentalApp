// Sensor.cs
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents a sensor that collects environmental data.
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// Gets or sets the unique identifier for the sensor.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the sensor.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the sensor's location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the sensor's location.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the operational status of the sensor (e.g., "Active", "Inactive").
        /// </summary>
        public string OperationalStatus { get; set; }

        /// <summary>
        /// Gets or sets the date of the last maintenance performed on the sensor, or null if no maintenance has occurred.
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; } // Nullable DateTime

        /// <summary>
        /// Gets or sets a description of the sensor and its purpose.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type of the sensor (e.g., "Air Quality", "Weather", "Water Quality").
        /// </summary>