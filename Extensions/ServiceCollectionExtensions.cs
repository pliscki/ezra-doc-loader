using Ezra.Scribe.Core.CloudStorage;
using Ezra.Scribe.Core.CloudStorage.GoogleDrive;
using Ezra.Scribe.Infrastructure.GoogleDrive;
using Ezra.Scribe.Infrastructure.CloudStorage;

namespace Ezra.Scribe.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEzraScribe(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.Configure<GoogleDriveSettings>(configuration.GetSection(nameof(GoogleDriveSettings)));
            services.AddScoped<IGoogleDriveAuthService, GoogleDriveAuthService>();
            services.AddScoped<ICloudStorageService, GoogleDriveStorageService>();
            services.AddMemoryCache();
            services.AddSingleton<IUserTokenCache, UserTokenCache>();

            if (environment.IsDevelopment())
            {
                services.AddSingleton<IUserTokenStore, LocalUserTokenStore>();
            }
            else
            {
                // Register your production IUserTokenStore implementation here
                // services.AddSingleton<IUserTokenStore, ProductionUserTokenStore>();
            }

            return services;
        }
    }
}
