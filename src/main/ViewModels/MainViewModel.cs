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
using System.Reflection;
using System.Diagnostics;

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
        private Timer timer;
        private const int interval = 10000; // FIXME: It need to be able to set interval on settingsPage

        public ObservableCollection<object> Updates { get; }
        public SettingsViewModel settingsViewModel;

        [ObservableProperty]
        private bool _locationUpdatesEnabled;

        public MainViewModel(ISettingsService settingsService,
                            IUrlService urlService,
                            ILocationService locationService,
                            INeuronClient neuronClient,
                            ITerminalClient terminalClient,
                            IOidcClientService oidcClientService,
                            HttpClient httpclient,
                            IConnectivity connectivity,
                            ITokenProviderService tokenProviderService)

        {
            this.settingsService = settingsService;
            this.urlService = urlService;
            this.oidcClientService = oidcClientService;
            this.httpClient = httpclient;
            this.connectivity = connectivity;
            this.locationService = locationService;
            this.neuronClient = neuronClient;
            this.terminalClient = terminalClient;
            this.tokenProviderService = tokenProviderService;
            Updates = new();
        }

        [RelayCommand]
        public void ChangeLocationUpdates()
        {
            this.LocationUpdatesEnabled = !this.LocationUpdatesEnabled;
            if (this.LocationUpdatesEnabled)
                this.StartLocationUpdates();
            else
                this.StopLocationUpdates();
        }

        public bool IsCurrentTimeInRange()
        {
            DateTime currentTime = DateTime.Now;
            return currentTime >= this.settingsService.StartTime && currentTime <= this.settingsService.EndTime;
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
        public async Task StartTimerAsync()
        {
            timer = new Timer(Timer_tick, null, 0, interval);
        }

        public async void Timer_tick(object state)
        {
            try
            {
                if (IsCurrentTimeInRange())
                {
                    await this.UploadLastLocationCoreAsync();
                }
            }
            catch (TargetInvocationException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task UploadLastLocationCoreAsync()
        {
            var o = this.Updates.Last() as LocationModel;

            if (o != null)
            {
                var neuronId = Guid.NewGuid().ToString();
                string regionId = null;
                try
                {
                    await this.neuronClient.CreateNeuron(
                        this.urlService.AvatarUrl + "/",
                        neuronId.ToString(),
                        o.Latitude + ", " + o.Longitude,
                        regionId,
                        string.Empty,
                        this.tokenProviderService.AccessToken
                        );

                    await this.terminalClient.CreateTerminal(
                        this.urlService.AvatarUrl + "/",
                        Guid.NewGuid().ToString(),
                        neuronId,
                        this.settingsService.InstantiatesGpsNeuronId,
                        neurUL.Cortex.Common.NeurotransmitterEffect.Excite,
                        1f,
                        string.Empty,
                        this.tokenProviderService.AccessToken
                        );

                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", ex.ToString(), "Ok");
                }
            }
        }
        private void LocationService_LocationChanged(object sender, Models.LocationModel e)
        {
            Updates.Add(e);
        }

        private void LocationService_StatusChanged(object sender, string e)
        {
            Updates.Add(e);
        }

        public void StartLocationUpdates()
        {
            this.locationService.LocationChanged += this.LocationService_LocationChanged;
            this.locationService.StatusChanged += this.LocationService_StatusChanged;
            this.locationService.Initialize();
        }

        public void StopLocationUpdates()
        {
            this.locationService.Stop();
            this.locationService.LocationChanged -= this.LocationService_LocationChanged;
            this.locationService.StatusChanged -= this.LocationService_StatusChanged;
        }
    }
}

