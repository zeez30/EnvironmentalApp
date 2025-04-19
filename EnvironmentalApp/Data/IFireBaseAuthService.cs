using System.Threading.Tasks;

namespace EnvironmentalApp.Data
{
    public interface IFirebaseAuthService
    {
        Task<bool> SignInWithEmailAndPasswordAsync(string email, string password);
        Task SignOutAsync();
        bool IsSignedIn();
    }
}
