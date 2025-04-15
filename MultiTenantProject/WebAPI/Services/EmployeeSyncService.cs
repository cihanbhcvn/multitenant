using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class EmployeeSyncService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<EmployeeSyncService> _logger;

        public EmployeeSyncService(IServiceProvider services, ILogger<EmployeeSyncService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Starting sync at {time}", DateTimeOffset.Now);

                try
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var lines = await File.ReadAllLinesAsync("sync_data.txt", stoppingToken);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(',');
                        var empId = int.Parse(parts[0]);
                        var deptId = int.Parse(parts[1]);

                        var exists = await db.EmployeeDepartments
                            .AnyAsync(ed => ed.EmployeeId == empId && ed.DepartmentId == deptId, stoppingToken);

                        if (!exists)
                        {
                            db.EmployeeDepartments.Add(new EmployeeDepartment
                            {
                                EmployeeId = empId,
                                DepartmentId = deptId
                            });
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Sync completed at {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during sync");
                }

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}
