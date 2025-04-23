using System;
using System.Collections.ObjectModel;
using EnvironmentalApp.Data;

namespace EnvironmentalApp.Services
{
    public class SensorService
    {
        public ObservableCollection<Sensor> GetMockSensors()
        {
            // Create some sample sensor data
            var mockSensors = new ObservableCollection<Sensor>()
            {
                new Sensor
                {
                    Id = 1,
                    Name = "Temperature Sensor 1",
                    Type = "Weather",
                    Status = "Online",
                    Latitude = 55.9533,
                    Longitude = -3.1883,
                    LastReadingValue = "15.5°C",
                    LastReadingTimestamp = DateTime.Now.AddMinutes(-5)
                },
                new Sensor
                {
                    Id = 2,
                    Name = "Air Quality Monitor - City Center",
                    Type = "Air",
                    Status = "Online",
                    Latitude = 55.9500,
                    Longitude = -3.1900,
                    LastReadingValue = "Good",
                    LastReadingTimestamp = DateTime.Now.AddMinutes(-10)
                },
                new Sensor
                {
                    Id = 3,
                    Name = "Water Flow Sensor - River Esk",
                    Type = "Water",
                    Status = "Offline",
                    Latitude = 55.9200,
                    Longitude = -3.1500,
                    LastReadingValue = "N/A",
                    LastReadingTimestamp = DateTime.Now.AddHours(-1)
                },
                new Sensor
                {
                    Id = 4,
                    Name = "Humidity Sensor - Botanical Garden",
                    Type = "Weather",
                    Status = "Online",
                    Latitude = 55.9600,
                    Longitude = -3.1950,
                    LastReadingValue = "78%",
                    LastReadingTimestamp = DateTime.Now.AddMinutes(-2)
                }
            };
            return mockSensors;
        }

        public void DetectAnomalies(ObservableCollection<Sensor> sensors)
        {
            foreach (var sensor in sensors)
            {
                sensor.IsAnomalous = false; // Reset

                if (sensor.Status == "Offline" || sensor.Status == "Malfunctioning")
                {
                    sensor.IsAnomalous = true;
                    sensor.AnomalyReason = $"Sensor is marked as {sensor.Status}";
                }
                else if ((DateTime.Now - sensor.LastReadingTimestamp).TotalMinutes > 30)
                {
                    sensor.IsAnomalous = true;
                    sensor.AnomalyReason = "Sensor has not reported in over 30 minutes.";
                }
            }
        }

    }
}