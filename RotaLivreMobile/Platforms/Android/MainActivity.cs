using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;

namespace RotaLivreMobile;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    Exported = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                           ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
                           ConfigChanges.SmallestScreenSize | ConfigChanges.Density
)]

[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[]
    {
        Intent.CategoryDefault,
        Intent.CategoryBrowsable
    },
    DataScheme = "rotalivre",
    DataHost = "grupo"
)]

public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        HandleIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);

        HandleIntent(intent);
    }

    private void HandleIntent(Intent? intent)
    {
        var data = intent?.Data;

        if (data != null)
        {
            var uri = new Uri(data.ToString());

            App.Current?.SendOnAppLinkRequestReceived(uri);
        }
    }
}