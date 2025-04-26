using Microsoft.Maui.Controls;


namespace EnvironmentalApp
{

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Set the MainPage to be the LoginPage
            MainPage = new NavigationPage(new LoginPage());  // Use NavigationPage to allow page navigation
        }
    }
}
