using System;
using System.Threading.Tasks;
using Android.Gms.Tasks;
using Firebase.Auth;
using EnvironmentalApp.Data;
using Application = Android.App.Application;
using GmsTask = Android.Gms.Tasks.Task;
using Task = System.Threading.Tasks.Task;

namespace EnvironmentalApp.Platforms.Android
{
    public class FirebaseAuthService : Java.Lang.Object, IFirebaseAuthService, IOnCompleteListener
    {
        private TaskCompletionSource<bool> _tcs;

        public Task<bool> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            _tcs = new TaskCompletionSource<bool>();
            FirebaseAuth.Instance.SignInWithEmailAndPassword(email, password)
                .AddOnCompleteListener(this);
            return _tcs.Task;
        }

        public void OnComplete(GmsTask task)
        {
            if (task.IsSuccessful)
                _tcs.TrySetResult(true);
            else
                _tcs.TrySetResult(false);
        }

        public Task SignOutAsync()
        {
            FirebaseAuth.Instance.SignOut();
            return Task.CompletedTask;
        }

        public bool IsSignedIn()
        {
            return FirebaseAuth.Instance.CurrentUser != null;
        }
    }
}
