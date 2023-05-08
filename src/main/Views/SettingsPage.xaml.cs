using ei8.Cortex.Gps.Sender.ViewModels;

namespace ei8.Cortex.Gps.Sender.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();
		this.BindingContext = settingsViewModel;
	}
}