using CommunityToolkit.Maui;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Gps.Sender.Services;
using ei8.Cortex.Gps.Sender.ViewModels;
using ei8.Cortex.Gps.Sender.ViewModels.Auth;
using ei8.Cortex.Gps.Sender.Views;
using IdentityModel.OidcClient;
using neurUL.Common.Http;

namespace ei8.Cortex.Gps.Sender;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IRequestProvider, RequestProvider>(sp =>
        {
            var rp = new RequestProvider();
            rp.SetHttpClientHandler(new HttpClientHandler());
            return rp;
        });
        builder.Services.AddSingleton<INeuronClient, HttpNeuronClient>();
        builder.Services.AddSingleton<ITerminalClient, HttpTerminalClient>();

        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<ILocationService, LocationService>();
		builder.Services.AddSingleton<LoginViewModel>();
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<Views.Auth.LoginPage>();
		builder.Services.AddSingleton<MainPage>();
		
		builder.Services.AddSingleton<HttpClient>(GetInsecureHttpClient());
        builder.Services.AddTransient<WebAuthenticatorBrowser>();
        
        builder.Services.AddTransient<OidcClient>(sp =>
		new OidcClient(new OidcClientOptions
		{
			Authority = "https://login.fibona.cc",
			ClientId = "mauimauefood.appclient",
			Scope = "openid profile avatarapi",
			RedirectUri = "mauimauefoodclient://",
			PostLogoutRedirectUri = "mauimauefoodclient://",
			ClientSecret = "SuperSecretPassword",
			HttpClientFactory = options => GetInsecureHttpClient(), 
			Browser = sp.GetRequiredService<WebAuthenticatorBrowser>()
        }));

        return builder.Build();
    }

    public static HttpClient GetInsecureHttpClient()
    {
        // #if ANDROID
        // TODO: var handler = new CustomAndroidMessageHandler();
        // #else
        var handler = new HttpClientHandler
        {
            // #endif
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
        };
        HttpClient client = new HttpClient(handler);
        return client;
    }
}