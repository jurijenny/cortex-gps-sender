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

            this.oidcClientService.ClearOidcClient();
            var loginResult = await this.oidcClientService.GetOidcClient().LoginAsync(new LoginRequest());
            if (loginResult.IsError)
            {
                await Shell.Current.DisplayAlert("Error", loginResult.Error, "Ok");
                return;
            }

            this.tokenProviderService.AccessToken = loginResult.AccessToken;
            this.tokenProviderService.ExpiresAt = loginResult.AccessTokenExpiration;
            this.tokenProviderService.RefreshToken = loginResult.RefreshToken;

            await Shell.Current.GoToAsync($"{nameof(MainPage)}",true);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ShowSettingsAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(SettingsPage)}", true);
    }
    
}