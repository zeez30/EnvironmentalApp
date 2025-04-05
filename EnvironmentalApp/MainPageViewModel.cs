using EnvironmentalApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel; 
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using EnvironmentalApp.Services;
using System.Threading.Tasks;
using EnvironmentalApp.Data;

namespace EnvironmentalApp.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        private readonly AppDbContext _dbContext;
        private readonly DatabaseService _databaseService;
        private readonly ILogger<MainPage> _logger;

        public ObservableCollection<AirQualityData> AirQualityData { get; set; } = new ObservableCollection<AirQualityData>();
        public ObservableCollection<Sensor> Sensors { get; set; } = new ObservableCollection<Sensor>(); // Initialize
        public ObservableCollection<Alert> RecentAlerts { get; set; } = new ObservableCollection<Alert>();

        public ICommand ImportDataCommand { get; private set; }

        public MainPageViewModel(AppDbContext dbContext, DatabaseService databaseService, ILogger<MainPage> logger)
        {
            _dbContext = dbContext;
            _databaseService = databaseService;
            _logger = logger;

            ImportDataCommand = new Command(async () => await ExecuteImportDataCommand());

            //LoadSensors(); // Load sensors when the ViewModel is created
            InitializeDataAsync();
        }

        private async Task ExecuteImportDataCommand()
        {
            try
            {
                await _databaseService.ImportExcelDataAsync(); // Await the import process
                // Optional: Refresh the sensor list after importing data
                await LoadSensors();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing or fetching data from Excel");
                // Log the error or handle it appropriately, possibly displaying an alert to the user
            }
        }

        private async Task InitializeDataAsync()
        {
            RecentAlerts = new ObservableCollection<Alert>
            {
                new Alert { AlertMessage = "High air pollution detected at sensor 123", AlertTime = DateTime.Now.AddHours(-1) },
                new Alert { AlertMessage = "Water pH level exceeded threshold at sensor 456", AlertTime = DateTime.Now.AddHours(-2) },
                new Alert { AlertMessage = "Sensor 789 offline. Please check.", AlertTime = DateTime.Now.AddHours(-3) }
            };
            OnPropertyChanged(nameof(RecentAlerts));
        }

        public async Task LoadSensors()
        {
            var sensors = await _dbContext.Sensors.ToListAsync();
            Sensors.Clear();
            foreach (var sensor in sensors)
            {
                Sensors.Add(sensor);
            }
            OnPropertyChanged(nameof(Sensors));
        }
    }
}