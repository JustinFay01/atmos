using System.Configuration;
using System.Data;
using System.Windows;

using Atmos.Application.Interfaces;
using Atmos.Presentation.Features.Dashboard.ViewModels;

using Infrastructure.Hardware;

using Microsoft.Extensions.DependencyInjection;

namespace Atmos.Presentation
{

    public partial class App : System.Windows.Application
    {
        public static IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            RegisterServices(services);
            RegisterViewModels(services);

            Services = services.BuildServiceProvider();

            base.OnStartup(e);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ISensorClient, MockSensorClient>();
        }

        private void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<DashboardViewModel>();
        }
    }

}
