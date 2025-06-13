using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MathTutor.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace MathTutor.Infrastructure.Services
{
    public class DatabaseInitializationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseInitializationService> _logger;

        public DatabaseInitializationService(
            ApplicationDbContext context,
            ILogger<DatabaseInitializationService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Starting database initialization...");

                // Apply any pending migrations
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied successfully.");

                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();
                _logger.LogInformation("Database creation verified.");

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
} 