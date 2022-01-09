using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Refit;
using SaCodeWhite.Api;
using SaCodeWhite.Api.Services;
using SaCodeWhite.Functions.Models;
using SaCodeWhite.Functions.Repositories;
using SaCodeWhite.Functions.Services;
using System;
using CloudTableStorageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount;

[assembly: FunctionsStartup(typeof(SaCodeWhite.Functions.Startup))]

namespace SaCodeWhite.Functions
{
    public class Startup : FunctionsStartup
    {
        public const string SaHealthBaseUrl = "https://www.sahealth.sa.gov.au/";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.AddRefitClient<IEmergencyDepartmentApi>(
                    new RefitSettings(new NewtonsoftJsonContentSerializer()))
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(SaHealthBaseUrl))
                .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(3, n => TimeSpan.FromSeconds(Math.Pow(2, n))));

            builder.Services.AddRefitClient<IAmbulanceServiceApi>(
                    new RefitSettings(new NewtonsoftJsonContentSerializer()))
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(SaHealthBaseUrl))
                .AddTransientHttpErrorPolicy(b => b.WaitAndRetryAsync(3, n => TimeSpan.FromSeconds(Math.Pow(2, n))));

            string getConnectionString(IServiceProvider serviceProvider)
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var storageConnString =
                    config["Values:AzureWebJobsStorage"] ??
                    config.GetConnectionString("StorageConnectionString") ??
                    Environment.GetEnvironmentVariable("AzureWebJobsStorage") ??
                    "UseDevelopmentStorage=true;";
                return storageConnString;
            }

            builder.Services.AddSingleton(serviceProvider =>
                CloudTableStorageAccount.Parse(getConnectionString(serviceProvider)).CreateCloudTableClient());

            builder.Services.AddOptions<NotificationHubOptions>()
                .Configure(configuration.GetSection("NotificationHub").Bind);

            builder.Services.AddSingleton<IBlobService>(sp => new BlobService(getConnectionString(sp)));
            builder.Services.AddSingleton<INotificationHubService, NotificationHubService>();

            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddSingleton<IHospitalService, HospitalService>();
            builder.Services.AddSingleton<IAmbulanceService, Api.Services.AmbulanceService>();
            builder.Services.AddSingleton<ITriageCategoryService, TriageCategoryService>();
            builder.Services.AddSingleton<IEmergencyDepartmentService, EmergencyDepartmentService>();

            builder.Services.AddSingleton<ITableRepository<DeviceInstallationEntity>, TableRepository<DeviceInstallationEntity>>();
        }
    }
}