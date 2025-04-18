using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using EnvironmentalApp.Data;

namespace EnvironmentalApp.Services
{
    /// <summary>
    /// Provides access to sensor data, currently using mock data for demonstration.
    /// </summary>
    public class SensorService
    {
        /// <summary>
        /// Retrieves a collection of sample sensor data for UI display.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{Sensor}"/> containing mock <see cref="Sensor"/> objects.</returns>
        public ObservableCollection<Sensor> GetMockSensors()
        {
            var mockSensors = new ObservableCollection<Sensor>()
            {
                new Sensor { Id = 1, Name = "Air Sensor 1", Type = "Air", Status = "Online", Latitude = 55.9533, Longitude = -3.1883, LastReadingValue = "NO2: 25 ppm", LastReadingTimestamp = DateTime.Now.AddMinutes(-5) },
                new Sensor { Id = 2, Name = "Water Sensor A", Type = "Water", Status = "Online", Latitude = 55.9450, Longitude = -3.2000, LastReadingValue = "pH: 7.2", LastReadingTimestamp = DateTime.Now.AddMinutes(-10) },
                new Sensor { Id = 3, Name = "Weather Station X", Type = "Weather", Status = "Offline", Latitude = 55.9600, Longitude = -3.1700, LastReadingValue = "Temp: 15°C", LastReadingTimestamp = DateTime.Now.AddHours(-1) },
                new Sensor { Id = 4, Name = "Air Sensor 2", Type = "Air", Status = "Malfunctioning", Latitude = 55.9500, Longitude = -3.1950, LastReadingValue = "Error", LastReadingTimestamp = DateTime.Now.AddMinutes(-2) },
                new Sensor { Id = 5, Name = "Water Sensor B", Type = "Water", Status = "Online", Latitude = 55.9400, Longitude = -3.2100, LastReadingValue = "Turbidity: Low", LastReadingTimestamp = DateTime.Now.AddMinutes(-7) }
            };

            return mockSensors;
        }
    }
}
