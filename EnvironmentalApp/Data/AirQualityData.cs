//AirQualityData.cs
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentalApp.Data
{
    public class AirQualityData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime DateTime { get; set; } // Combine Date and Time into one DateTime
        public double NitrogenDioxide { get; set; }
        public double SulphurDioxide { get; set; }
        public double PM2_5 { get; set; }
        public double PM10 { get; set; }
    }
}
