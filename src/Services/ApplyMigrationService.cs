using System;
using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Services; // đảm bảo rằng SeedService nằm trong namespace này
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace src.Services
{
    public class ApplyMigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApplyMigrationService> _logger;

        public ApplyMigrationService(IServiceProvider serviceProvider, ILogger<ApplyMigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

            try
            {
                _logger.LogInformation("Checking for pending migrations...");
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying pending migrations...");
                    await dbContext.Database.MigrateAsync(cancellationToken);
                    _logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations found.");
                }

                // Gọi SeedService để seed dữ liệu
                _logger.LogInformation("Seeding data...");
                var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
                await seedService.SeedAsync();
                _logger.LogInformation("Data seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying migrations and seeding data");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
