using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using EnvironmentalApp.Data;

namespace EnvironmentalApp;

public partial class UserManagementPage : ContentPage
{
    private readonly FirebaseAuthService _authService;
    public ObservableCollection<UserInfo> Users { get; set; }

    /// Constructor for UserManagementPage.
    public UserManagementPage(FirebaseAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        Users = new ObservableCollection<UserInfo>(
            _authService.GetAllUsersWithRoles()
        );

        BindingContext = this;
    }

    /// Changes the role of a selected user.
    private async void OnRoleChanged(object sender, EventArgs e)
    {
        if (userPicker.SelectedItem is UserInfo selectedUser && rolePicker.SelectedItem is string newRole)
        {
            bool updated = _authService.ChangeUserRole(selectedUser.Email, newRole);
            if (updated)
            {
                selectedUser.Role = newRole;
                await DisplayAlert("Success", $"{selectedUser.Email}'s role changed to {newRole}", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to change role.", "OK");
            }
        }
    }
}
