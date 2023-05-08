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
        builder.Services.AddSingleton<IUrlService, UrlService>();
        builder.Services.AddSingleton<ITokenProviderService, TokenProviderService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<Views.Auth.LoginPage>();
        builder.Services.AddSingleton<SettingsPage>();
		builder.Services.AddSingleton<MainPage>();
		
		builder.Services.AddSingleton<HttpClient>(OidcClientService.GetInsecureHttpClient());
        builder.Services.AddTransient<WebAuthenticatorBrowser>();
        builder.Services.AddTransient<IOidcClientService, OidcClientService>();

        return builder.Build();
    }
}