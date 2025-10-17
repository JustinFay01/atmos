using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Atmos.Application.Interfaces;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging;


namespace Atmos.Presentation.ViewModels
{
    public partial class SensorConnectionViewModel : ObservableObject
    {
        private readonly ISensorClient _sensorClient;
        private readonly ILogger<SensorConnectionViewModel> _logger;


        [ObservableProperty]
        public bool isInitializing;

        [ObservableProperty]
        private bool isConnected;

        public IAsyncRelayCommand ConnectCommand => new AsyncRelayCommand(ConnectAsync, () => !IsInitializing && !IsConnected);

        public SensorConnectionViewModel(ISensorClient sensorClient, ILogger<SensorConnectionViewModel> logger)
        {
            _sensorClient = sensorClient ?? throw new ArgumentNullException(nameof(sensorClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        private async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            IsInitializing = true;

            try
            {
                _logger.LogInformation("Attempting to connect to sensor client...");
                bool success = await _sensorClient.ConnectAsync(cancellationToken);
                IsConnected = success; 
                _logger.LogDebug("Sensor client connection status: {Status}", success ? "Connected" : "Failed to connect");
            }
            catch (Exception ex)
            {
                IsConnected = false;
                _logger.LogError(ex, "Error occurred while trying to connect to sensor client.");
            }
            finally
            {
                IsInitializing = false;
            }
        }

    }
}
