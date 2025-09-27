using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atmos.Application.Interfaces;

using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Atmos.Presentation.Features.Dashboard.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {

        private readonly ISensorClient _sensorClient;

        public DashboardViewModel(ISensorClient sensorClient)
        {
            _sensorClient = sensorClient;


            DisconnectCommand = new AsyncRelayCommand(DisconnectAsync);

           _sensorClient.ConnectAsync(default).ContinueWith(t => UpdateStatus(), TaskScheduler.FromCurrentSynchronizationContext());
        }


        [ObservableProperty]
        private string status = "Disconnected";
        public bool IsConnected => _sensorClient.IsConnected;


        public IAsyncRelayCommand DisconnectCommand { get; }

        private async Task DisconnectAsync()
        {
            await _sensorClient.DisconnectAsync(default);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            Status = _sensorClient.IsConnected ? "Connected" : "Disconnected";
        }
    }
}
