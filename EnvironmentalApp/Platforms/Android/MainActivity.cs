using Android.App;
using Android.OS;
using Firebase;

namespace EnvironmentalApp
{
    [Activity(Label = "EnvironmentalApp", MainLauncher = true)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Init Firebase using native context
            if (FirebaseApp.InitializeApp(this) == null)
            {
                FirebaseApp.InitializeApp(this);
            }
        }
    }
}
