using CommunityToolkit.Maui;
using ei8.Cortex.Diary.Nucleus.Client.In;
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

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainViewModel>();

        return builder.Build();
    }
}