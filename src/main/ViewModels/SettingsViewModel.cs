using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ei8.Cortex.Gps.Sender.Services;
using ei8.Cortex.Gps.Sender.Views.Auth;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Gps.Sender.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService settingsService;

        [ObservableProperty]
        private string clientSecret;

        [ObservableProperty]
        private string instantiatesGpsNeuronId;

        [ObservableProperty]
        private DateTime startTime;

        [ObservableProperty]
        private DateTime endTime;

        public SettingsViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            this.startTime = DateTime.Now;
            // FIXME: Fix this later
            this.endTime = DateTime.Now.AddMinutes(2);
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            this.settingsService.ClientSecret = this.ClientSecret;
            this.settingsService.InstantiatesGpsNeuronId = this.InstantiatesGpsNeuronId;
            this.settingsService.StartTime = this.StartTime;
            this.settingsService.EndTime = this.EndTime;
            await Shell.Current.GoToAsync("..", true);
        }
    }
}