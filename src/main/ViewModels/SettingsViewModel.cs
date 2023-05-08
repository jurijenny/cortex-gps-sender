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
        private ISettingsService settingsService;
        public SettingsViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        [ObservableProperty]
        private string clientSecret;

        [ObservableProperty]
        private string instantiatesGpsNeuronId;

        [RelayCommand]
        public async Task SaveAsync()
        {
            this.settingsService.ClientSecret = this.ClientSecret;
            this.settingsService.InstantiatesGpsNeuronId = this.InstantiatesGpsNeuronId;

            await Shell.Current.GoToAsync("..", true);
        }
    }
}