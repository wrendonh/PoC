using Crushbank.Common.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersistingPoC.Interfaces;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static PersistingPoC.Entities.Enums;

namespace PersistingPoC
{
    public class Worker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private static int ServiceCount = 0;

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
                    await StartIntegration(configuration);
                    ServiceCount++;
                    await Task.Delay(15000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "There was a problem processing the integration");
                    throw;
                }
            }
        }

        private async Task StartIntegration(IConfiguration configuration)
        {   
            var integrationType = configuration.GetValue<int>("IntegrationConfiguration:IntegrationType");
            var backDaysToStartProcess = configuration.GetValue<int>("IntegrationConfiguration:BackDaysToStartProcess");

            var companiesSection = configuration.GetSection("IntegrationConfiguration:CompaniesToProcess");
            var companiesToProcess = companiesSection.AsEnumerable().Select(x => Convert.ToInt32(x.Value)).Where(x => x.ToString() != "0").Reverse().ToArray();

            var taskTypesSection = configuration.GetSection("IntegrationConfiguration:TaskTypesToProcess");
            var taskTypesToProcess = taskTypesSection.AsEnumerable().Select(x => Convert.ToInt32(x.Value)).Where(x => x.ToString() != "0").Reverse().ToArray();

            var securityKey = configuration.GetValue<string>("SecurityConfiguration:SecurityKey");
            var initialVector = configuration.GetValue<string>("SecurityConfiguration:InitialVector");

            CryptographicUtility.SecurityKey = securityKey;
            CryptographicUtility.InitialVector = initialVector;

            var connectWiseIntegration = _serviceProvider.GetRequiredService<IConnectWiseTicketIntegration>();
            var taskProcesses = await connectWiseIntegration.ConfigureIntegration((IntegrationTypes)integrationType, companiesToProcess, taskTypesToProcess, ServiceCount == 0, backDaysToStartProcess);

            foreach (var taskProcess in taskProcesses)
            {
                await connectWiseIntegration.ExecuteProcesses(taskProcess);
            }
        }
    }
}
