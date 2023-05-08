using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Gps.Sender.Models;
using IdentityModel.OidcClient;
using IdentityModel.Client;
using System.Text.Json;
using ei8.Cortex.Gps.Sender.Services;

namespace ei8.Cortex.Gps.Sender.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;
        private readonly IUrlService urlService;
        private readonly ILocationService locationService;
        private readonly INeuronClient neuronClient;
        private readonly ITerminalClient terminalClient;
        protected readonly IOidcClientService oidcClientService;
        protected readonly HttpClient httpClient;
        protected IConnectivity connectivity;
        private readonly ITokenProviderService tokenProviderService;

        [ObservableProperty]
        private bool _locationUpdatesEnabled;

        public MainViewModel(ISettingsService settingsService, IUrlService urlService, ILocationService locationService, INeuronClient neuronClient, ITerminalClient terminalClient, IOidcClientService oidcClientService, HttpClient httpclient, IConnectivity connectivity, ITokenProviderService tokenProviderService)
        {
            this.settingsService = settingsService;
            this.urlService = urlService;
            this.oidcClientService = oidcClientService;
            this.httpClient = httpclient;
            this.connectivity = connectivity;
            Updates = new();
            this.locationService = locationService;
            this.neuronClient = neuronClient;
            this.terminalClient = terminalClient;
            this.tokenProviderService = tokenProviderService;
        }

        public ObservableCollection<object> Updates { get; }
        
        [RelayCommand]
        public void ChangeLocationUpdates()
        {
            this.LocationUpdatesEnabled = !this.LocationUpdatesEnabled;
            if (this.LocationUpdatesEnabled)
                StartLocationUpdates();
            else
                StopLocationUpdates();
        }

        [RelayCommand]
        public async Task UploadLastLocationAsync()
        {
            var o = this.Updates.Last() as LocationModel;

            if (o != null)
            {
                var neuronId = Guid.NewGuid().ToString();
                string regionId = null;
                try
                {
                    var task = Task.Run(async () => await this.neuronClient.CreateNeuron(
                        this.urlService.AvatarUrl + "/",
                        neuronId.ToString(),
                        o.Latitude + ", " + o.Longitude,
                        regionId,
                        string.Empty,
                        this.tokenProviderService.AccessToken
                        ));
                    task.GetAwaiter().GetResult();
                    task = Task.Run(async () => await this.terminalClient.CreateTerminal(
                        this.urlService.AvatarUrl + "/",
                        Guid.NewGuid().ToString(),
                        neuronId,
                        this.settingsService.InstantiatesGpsNeuronId,
                        neurUL.Cortex.Common.NeurotransmitterEffect.Excite,
                        1f,
                        string.Empty,
                        this.tokenProviderService.AccessToken
                        ));
                    task.GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", ex.ToString(), "Ok");
                }
            }
        }

        public void StartLocationUpdates()
        {
            this.locationService.LocationChanged += LocationService_LocationChanged;
            this.locationService.StatusChanged += LocationService_StatusChanged;
            this.locationService.Initialize();
        }

        public void StopLocationUpdates()
        {
            this.locationService.Stop();
            this.locationService.LocationChanged -= LocationService_LocationChanged;
            this.locationService.StatusChanged -= LocationService_StatusChanged;
        }

        private void LocationService_StatusChanged(object sender, string e)
        {
            Updates.Add(e);
        }
            
        private void LocationService_LocationChanged(object sender, Models.LocationModel e)
        {
            Updates.Add(e);            
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            try
            {
                var loginResult = await this.oidcClientService.GetOidcClient().LogoutAsync(new LogoutRequest());
                await Shell.Current.DisplayAlert("Result", "Success", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }

        [RelayCommand]
        async Task CallApiAsync()
        {
            if (IsBusy)
                return;
            try
            {
                if (this.connectivity.NetworkAccess is not NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("Internet Offline", "Check sua internet e tente novamente!", "Ok");
                    return;
                }

                IsBusy = true;
                this.httpClient.SetBearerToken(this.tokenProviderService.AccessToken);
                var response = await this.httpClient.GetAsync("https://192.168.1.110:6001/identity");
                if (!response.IsSuccessStatusCode)
                    await Shell.Current.DisplayAlert("Api Error", $"{response.StatusCode}", "ok");


                var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
                var formatted = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
                await Shell.Current.DisplayAlert("Token Claims", formatted, "ok");
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
}

