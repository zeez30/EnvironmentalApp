//MetaData.cs
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    public class MetaData
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; } // Column A
        public string Quantity { get; set; } // Column B
        public string Symbol { get; set; }   // Column C
        public string Unit { get; set; }     // Column D
        public string UnitDescription { get; set; } // Column E
        public string MeasurementFrequency { get; set; } // Column F
        public double? SafeLevel { get; set; }   // Column G (Nullable double)
        public string Reference { get; set; }   // Column H
        public string Sensor { get; set; }      // Column I
        public string URL { get; set; }         // Column J
    }
}