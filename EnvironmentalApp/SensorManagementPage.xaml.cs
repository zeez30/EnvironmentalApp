using System.Collections.ObjectModel;
using EnvironmentalApp.Data;
using EnvironmentalApp.Services;
using Microsoft.Maui.Controls;

namespace EnvironmentalApp
{
    /// <summary>
    /// Represents the page for managing and viewing sensor information.
    /// This page displays a list of sensors and their basic details.
    /// </summary>
    public partial class SensorManagementPage : ContentPage
    {
        /// <summary>
        /// The collection of sensors to be displayed in the UI.
        /// Using <see cref="ObservableCollection{Sensor}"/> allows for automatic
        /// UI updates when the collection changes.
        /// </summary>
        public ObservableCollection<Sensor> Sensors { get; set; }

        /// <summary>
        /// A private read-only instance of the <see cref="SensorService"/>
        /// used to retrieve sensor data.
        /// </summary>
        private readonly SensorService _sensorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorManagementPage"/> class.
        /// This constructor sets up the page, initializes the sensor service,
        /// loads the initial sensor data, and sets the binding context.
        /// </summary>
        public SensorManagementPage()
        {
            // This method is auto-generated and initializes the UI components
            // defined in the associated XAML file (SensorManagementPage.xaml).
            InitializeComponent();

            // Create a new instance of the SensorService to access sensor data.
            _sensorService = new SensorService();

            // Load the initial list of sensors when the page is created.
            LoadSensors();

            // Set the BindingContext of the page to this instance.
            // This allows the XAML elements to bind to the public properties
            // of this class, such as the 'Sensors' collection.
            BindingContext = this;
        }

        /// <summary>
        /// Loads the sensor data using the <see cref="SensorService"/> and
        /// populates the <see cref="Sensors"/> collection.
        /// </summary>
        private void LoadSensors()
        {
            // Call the GetMockSensors method of the SensorService to retrieve
            // the list of mock sensor data.
            Sensors = _sensorService.GetMockSensors();
            _sensorService.DetectAnomalies(Sensors);
            // The ObservableCollection will automatically notify the UI that
            // its content has changed, causing the CollectionView to update.
        }
    }
}
