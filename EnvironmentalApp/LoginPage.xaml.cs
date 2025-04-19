using EnvironmentalApp.Data;
using Microsoft.Maui.Controls;
using System;

namespace EnvironmentalApp
{
    public partial class LoginPage : ContentPage
    {
        private FirebaseAuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = new FirebaseAuthService();  // Initialize the authentication service
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var email = emailEntry.Text;
            var password = passwordEntry.Text;  

            var success = await _authService.ValidateUserAsync(email, password);

            if (success)
            {
                // Redirect to Home or Admin page depending on the role
                await DisplayAlert("Success", "User signed in successfully!", "OK");
                // Navigate to the home page:
                await Navigation.PushAsync(new MainPage(_authService));
            }
            else
            {
                await DisplayAlert("Error", "Invalid login credentials.", "OK");
            }
        }
    }
}
