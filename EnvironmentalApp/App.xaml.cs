using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using EnvironmentalApp.Services;
using EnvironmentalApp;
using System;

namespace EnvironmentalApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            // This method is called when the application starts.

            // Resolve the DatabaseService from the dependency injection container
            var databaseService = Services.GetService<DatabaseService>();

            if (databaseService != null)
            {
                await databaseService.InitializeDatabaseAsync();
                Console.WriteLine("Database initialization completed."); 
            }
            else
            {
                Console.WriteLine("Error: DatabaseService not found in DI container.");
            }

            base.OnStart();
        }
    }
}