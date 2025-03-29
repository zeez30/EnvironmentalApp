// WaterQualityData.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace EnvironmentalApp.Data
{
    public class WaterQualityData
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }       // Column A
        public TimeSpan Time { get; set; }       // Column B (adjust if Time is stored differently)
        public double Nitrate { get; set; }      // Column C
        public double Nitrite { get; set; }      // Column D
        public double Phosphate { get; set; }    // Column E
        public double EC { get; set; }           // Column F (E. coli)

        // Add other properties for additional columns here
    }
}