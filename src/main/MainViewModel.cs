using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Gps.Sender.Models;

namespace ei8.Cortex.Gps.Sender
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly Services.LocationService _locationService;
        private readonly INeuronClient neuronClient;
        private readonly ITerminalClient terminalClient;

        [ObservableProperty]
        private bool _locationUpdatesEnabled;

        [ObservableProperty]
        private string instantiatesGpsNeuronId;

        [ObservableProperty]
        private string avatarUrl;

        public MainViewModel(INeuronClient neuronClient, ITerminalClient terminalClient)
        {
            Updates = new();
            _locationService = new();
            this.neuronClient = neuronClient;
            this.terminalClient = terminalClient;
        }

        public ObservableCollection<object> Updates { get; }
        
        [RelayCommand]
        public void ChangeLocationUpdates()
        {
            LocationUpdatesEnabled = !LocationUpdatesEnabled;
            if (LocationUpdatesEnabled)
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
            _locationService.LocationChanged += LocationService_LocationChanged;
            _locationService.StatusChanged += LocationService_StatusChanged;
            _locationService.Initialize();
        }

        public void StopLocationUpdates()
        {
            _locationService.Stop();
            _locationService.LocationChanged -= LocationService_LocationChanged;
            _locationService.StatusChanged -= LocationService_StatusChanged;
        }

        private void LocationService_StatusChanged(object sender, string e)
        {
            Updates.Add(e);
        }
            
        private void LocationService_LocationChanged(object sender, Models.LocationModel e)
        {
            Updates.Add(e);            
        }
    }
}

