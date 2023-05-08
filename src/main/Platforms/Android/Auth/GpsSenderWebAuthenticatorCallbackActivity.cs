using Android.App;
using Android.Content;
using Android.Content.PM;

namespace ei8.Cortex.Gps.Sender.Platforms.Android.Auth;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = CALLBACK_SCHEME)]
public class GpsSenderWebAuthenticatorCallbackActivity : WebAuthenticatorCallbackActivity
{
    const string CALLBACK_SCHEME = "ei8cortexgpssender";
}