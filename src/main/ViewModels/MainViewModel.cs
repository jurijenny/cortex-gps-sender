using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Gps.Sender.Models;
using IdentityModel.OidcClient;
using IdentityModel.Client;
using System.Text.Json;

namespace ei8.Cortex.Gps.Sender.ViewModels
{
    [QueryProperty("Token", "Token")]
    public partial class MainViewModel : ViewModelBase
    {
        private readonly Services.LocationService locationService;
        private readonly INeuronClient neuronClient;
        private readonly ITerminalClient terminalClient;
        protected readonly OidcClient oidcClient;
        protected readonly HttpClient httpClient;
        protected IConnectivity connectivity;

        [ObservableProperty] 
        private string token;

        [ObservableProperty]
        private bool _locationUpdatesEnabled;

        [ObservableProperty]
        private string instantiatesGpsNeuronId;

        [ObservableProperty]
        private string avatarUrl;

        public MainViewModel(INeuronClient neuronClient, ITerminalClient terminalClient, OidcClient client, HttpClient httpclient, IConnectivity connectivity)
        {
            this.oidcClient = client;
            this.httpClient = httpclient;
            this.connectivity = connectivity;
            Updates = new();
            this.locationService = new();
            this.neuronClient = neuronClient;
            this.terminalClient = terminalClient;
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
        public void UploadLastLocation()
        {
            var o = this.Updates.Last() as LocationModel;

            if (o != null)
            {
                var neuronId = Guid.NewGuid().ToString();
                var instantiatesGps = this.InstantiatesGpsNeuronId; 
                var avatarUrl = this.AvatarUrl; 
                string regionId = null;
                try
                {
                    var task = Task.Run(async () => await this.neuronClient.CreateNeuron(
                        avatarUrl,
                        neuronId.ToString(),
                        o.Latitude + ", " + o.Longitude,
                        regionId,
                        string.Empty,
                        string.Empty
                        ));
                    task.GetAwaiter().GetResult();
                    task = Task.Run(async () => await this.terminalClient.CreateTerminal(
                        avatarUrl,
                        Guid.NewGuid().ToString(),
                        neuronId,
                        instantiatesGps,
                        neurUL.Cortex.Common.NeurotransmitterEffect.Excite,
                        1f,
                        string.Empty,
                        String.Empty
                        ));
                    task.GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var exc = ex;
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
                var loginResult = await this.oidcClient.LogoutAsync(new LogoutRequest());
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
                this.httpClient.SetBearerToken(this.token);
                var response = await this.httpClient.GetAsync("https://10.0.2.2:6001/identity");
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

