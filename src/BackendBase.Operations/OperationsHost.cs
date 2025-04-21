using Azure.Identity;
using BackendBase.Core;
using BackendBase.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackendBase.Operations;
public static class OperationsHost
{
    public static IHost Create(CommonOptions commonOptions)
    {
        return Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                var configuration = config.Build();
                var appConfigEndpoint = configuration.GetValue<string>("AppConfigEndpoint");
                var useAzureAppConfiguration = configuration.GetValue<string>("UseAzureAppConfiguration");

                if (useAzureAppConfiguration == "true" && !string.IsNullOrEmpty(appConfigEndpoint))
                {
                    config.AddAzureAppConfiguration(options =>
                    {
                        var defaultAzureCredential = new DefaultAzureCredential();

                        options
                            .Connect(new Uri(appConfigEndpoint), defaultAzureCredential)
                            .Select(KeyFilter.Any, LabelFilter.Null)
                            .Select(KeyFilter.Any, commonOptions.Environment)
                            .ConfigureRefresh(refreshOptions => refreshOptions.Register("SentinelKey", refreshAll: true));

                        options.ConfigureKeyVault(kv =>
                        {
                            kv.SetCredential(defaultAzureCredential);
                        });
                    });
                }

                if (commonOptions.Environment == Environments.Local)
                {
                    config.AddJsonFile($"appsettings.local.json", optional: true, reloadOnChange: true);
                }
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                });
            })
            .ConfigureServices((context, services) =>
            {
                var config = context.Configuration;
                services.AddInfrastructure(config);
                services.AddCore();
            })
            .Build();
    }
}
