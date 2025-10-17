using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Atmos.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Logging;

namespace Atmos.Presentation.Features.Startup
{
    public partial class StartupViewModel : ObservableObject
    {
        private readonly SensorConnectionViewModel _connectionModel;
        private readonly ILogger<StartupViewModel> _logger; 

        [ObservableProperty]
        public bool isLoading;

        [ObservableProperty]
        public bool isConnected;

        public StartupViewModel(ILogger<StartupViewModel> logger, SensorConnectionViewModel connectionModel)
        {
            _connectionModel = connectionModel ?? throw new ArgumentNullException(nameof(connectionModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        [RelayCommand]
        public async Task InitalizeAsync(CancellationToken cancellationToken = default)
        {
            IsLoading = true;
            await _connectionModel.ConnectCommand.ExecuteAsync(cancellationToken);
            IsConnected = _connectionModel.IsConnected;
            IsLoading = false;
        }


    }
}
