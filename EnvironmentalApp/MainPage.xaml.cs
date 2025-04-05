using Microsoft.Maui.Controls;
using EnvironmentalApp.ViewModels;
using System;

namespace EnvironmentalApp
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel; // Store the ViewModel instance

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadSensors();
        }

        private async void OnMapViewClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapPage());
        }

        private async void OnSensorManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SensorManagementPage());
        }

        private async void OnDataAnalysisClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DataAnalysisPage());
        }

        private async void OnReportsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReportsPage());
        }

        private async void OnUserManagementClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserManagementPage());
        }
    }
}