using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace EnvironmentalApp.Data
{
    public class FirebaseAuthService
    {

        /// <summary>
        /// Dictionary to hold email and password
        /// </summary>
        private readonly Dictionary<string, string> _validUsers = new Dictionary<string, string>
        {
            { "admin@example.com", "admin123" },
            { "user@example.com", "user123" }
        };

        /// <summary>
        /// Dictionary to hold email and role
        /// </summary>
        private readonly Dictionary<string, string> _userRoles = new Dictionary<string, string>
        {
                { "admin@example.com", "admin" },
                { "user@example.com", "user" }
        };

        public string CurrentUserRole { get; private set; }

        /// <summary>
        /// Validate user credentials against the dictionary
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns></returns>
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

        /// <summary>
        ///  Get the role of the user based on the email
        /// </summary>
        /// <param name="email">user's email</param>
        /// <returns>_userRoles</returns>
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

        /// <summary>
        /// Signs out the user by clearing the auth link.
        /// </summary>
        /// <exception cref="InvalidOperationException">Failed to sign out</exception>
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

        /// <summary>
        /// Checks if user is signed in by checking if the auth link exists.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool IsSignedIn()
        {
            return _authLink != null;  // If the authLink exists, user is signed in
        }

        /// <summary>
        /// Change the role of a user based on their email.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="newRole">New Role to be assigned</param>
        /// <returns>True if email has been changed</returns>
        public bool ChangeUserRole(string email, string newRole)
        {
            if (_userRoles.ContainsKey(email))
            {
                _userRoles[email] = newRole;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fetch all users with their roles.
        /// </summary>
        /// <returns>all users and their roles</returns>
        public List<UserInfo> GetAllUsersWithRoles()
        {
            return _userRoles.Select(kvp => new UserInfo { Email = kvp.Key, Role = kvp.Value }).ToList();
        }

    }
}
