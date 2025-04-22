// Alert.cs
using System;

namespace EnvironmentalApp.Data
{
    /// <summary>
    /// Represents an alert message with a timestamp.
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// Gets or sets the message content of the alert.
        /// </summary>
        public string AlertMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the alert was generated.
        /// </summary>
        public DateTime AlertTime { get; set; }
    }
}