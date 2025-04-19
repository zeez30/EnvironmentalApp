using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace EnvironmentalApp.Data
{
    public class FirebaseAuthService
    {
        private readonly Dictionary<string, string> _validUsers = new Dictionary<string, string>
        {
            { "admin@example.com", "admin123" },
            { "user@example.com", "user123" }
        };

        private readonly Dictionary<string, string> _userRoles = new Dictionary<string, string>
        {
                { "admin@example.com", "admin" },
                { "user@example.com", "user" }
        };

        public string CurrentUserRole { get; private set; }

        // This method can be used to validate email and password against the predefined list
        public Task<bool> ValidateUserAsync(string email, string password)
        {
            // Check if the email exists and the password matches
            if (_validUsers.ContainsKey(email) && _validUsers[email] == password)
            {
                CurrentUserRole = GetUserRole(email);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private readonly FirebaseAuthProvider _authProvider;
        private FirebaseAuthLink _authLink;  // Store the auth link after login

        public FirebaseAuthService()
        {
            var firebaseConfig = new FirebaseConfig("AIzaSyA2g6pKvKuszH5pKvQGUdMIaHsGJqXtiN0");  // Replace with your Firebase API Key
            _authProvider = new FirebaseAuthProvider(firebaseConfig);


        }

        // Sign In user with Email and Password
        public async Task<bool> SignInAsync(string email, string password)
        {
            if (_validUsers.ContainsKey(email) && _validUsers[email] == password)
            {
                // Assign the role based on email
                CurrentUserRole = GetUserRole(email); // Update the role after login

                return true; // Return true if valid credentials
            }
            return false; // Invalid credentials
        }

        private string GetUserRole(string email)
        {
            // Manually return role for this email (fake role assignment)
            if (_userRoles.ContainsKey(email))
            {
                var test = _validUsers[email];
                return _userRoles[email];
            }
            return "user";  // Default to 'user' if no role is found
        }

        // Sign Out user
        public void SignOut()
        {
            try
            {
                _authLink = null;  // Clear the authentication link
                // No need for a specific method call in FirebaseAuthProvider to sign out
            }
            catch (Exception)
            {
                // Handle any errors that occur during sign-out
                throw new InvalidOperationException("Failed to sign out");
            }
        }

        // Check if the user is signed in
        public bool IsSignedIn()
        {
            return _authLink != null;  // If the authLink exists, user is signed in
        }
    }
}
