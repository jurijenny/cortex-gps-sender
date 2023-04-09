using ei8.Cortex.Gps.Sender.ViewModels.Auth;

namespace ei8.Cortex.Gps.Sender.Views.Auth;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel loginviewmodel)
	{
		InitializeComponent();
		BindingContext = loginviewmodel;
	}
}
