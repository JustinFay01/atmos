using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Atmos.Application.Extensions;
using Atmos.Presentation.Features.Dashboard;
using Atmos.Presentation.ViewModels;

using Infrastructure.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Atmos.Presentation.AppConfig
{
    internal class AppHost
    {
        private static IServiceProvider? _serviceProvider;

        public static IServiceProvider Provider => _serviceProvider ??= CreateServiceProvider();

        public static T GetRequiredService<T>() where T : notnull => Provider.GetRequiredService<T>();

        public static void Init()
        {
            _serviceProvider = CreateServiceProvider();
        }

        private static IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            return configuration;
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddScoped<SensorConnectionViewModel>();
            services.AddScoped<DashboardViewModel>();
        }

        private static IServiceProvider CreateServiceProvider()
        {
            if (_serviceProvider != null)
            {
                return _serviceProvider;
            }

            var services = new ServiceCollection();
            var config = BuildConfiguration();
            services.AddSingleton(config);

            services.AddLogging();
            services.UseAtmosApplicationServices();
            services.UseAtmosInfrastructure(config);

            RegisterViewModels(services);


            return services.BuildServiceProvider();
        }

    }
}
