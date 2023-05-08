using ei8.Cortex.Gps.Sender.Views;

namespace ei8.Cortex.Gps.Sender;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }
}