using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ei8.Cortex.Gps.Sender.Services;
using ei8.Cortex.Gps.Sender.Views;
using IdentityModel.OidcClient;

namespace ei8.Cortex.Gps.Sender.ViewModels.Auth;

public partial class LoginViewModel : ViewModelBase
{
    private IUrlService urlService;
    private IOidcClientService oidcClientService;
    private IConnectivity connectivity;
    private ITokenProviderService tokenProviderService;
    public LoginViewModel(IUrlService urlService, IOidcClientService oidcClientService, IConnectivity connectivity, ITokenProviderService tokenProviderService)
    {
        this.urlService = urlService;
        this.oidcClientService = oidcClientService;
        this.connectivity = connectivity;
        this.tokenProviderService = tokenProviderService;
    }

    [ObservableProperty]
    private string avatarUrl;

    partial void OnAvatarUrlChanged(string value)
    {
        this.urlService.AvatarUrl = value;
    }

    [RelayCommand]
    async Task LoginAsync()
    {
        if (IsBusy)
            return;
        
        try
        {
            if(connectivity.NetworkAccess is not NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Internet Offline", "Please check your internet connection", "Ok");
                return;
            }
            
            var loginResult = await oidcClientService.GetOidcClient().LoginAsync(new LoginRequest());
            if (loginResult.IsError)
                return;

            this.tokenProviderService.AccessToken = loginResult.AccessToken;
            this.tokenProviderService.ExpiresAt = loginResult.AccessTokenExpiration;
            this.tokenProviderService.RefreshToken = loginResult.RefreshToken;

            await Shell.Current.DisplayAlert("Login Result", "Access Token is:\n\n" + this.tokenProviderService.AccessToken, "Close");
            await Shell.Current.GoToAsync($"{nameof(MainPage)}",true,
                new Dictionary<string, object>
                {
                    {"Token", this.tokenProviderService.AccessToken }
                });
            
            // Application.Current.MainPage = new MainPage();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "ok");
        }
        finally
        {
            IsBusy = false;

        }
    }
    
}