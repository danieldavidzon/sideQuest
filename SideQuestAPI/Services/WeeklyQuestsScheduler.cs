
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SideQuestAPI.Services
{
    public class WeeklyQuestScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WeeklyQuestScheduler> _logger;

        public WeeklyQuestScheduler(
            IServiceScopeFactory scopeFactory,
            ILogger<WeeklyQuestScheduler> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime nextMondayUtc = GetNextMondayUtc();
                TimeSpan delay = nextMondayUtc - DateTime.UtcNow;

                _logger.LogInformation(
                    "Next weekly quest refresh: {RefreshTime} UTC",
                    nextMondayUtc);

                await Task.Delay(delay, stoppingToken);

                using var scope = _scopeFactory.CreateScope();

                var questService = scope.ServiceProvider
                    .GetRequiredService<WeeklyQuestService>();

                try
                {
                    await questService.GetOrGenerateWeeklyQuestsAsync();

                    _logger.LogInformation(
                        "Weekly quests refreshed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex, "Failed to refresh weekly quests.");
                }
            }
        }

        private static DateTime GetNextMondayUtc()
        {
            TimeZoneInfo israelTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    OperatingSystem.IsWindows()
                        ? "Israel Standard Time"
                        : "Asia/Jerusalem");

            DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, israelTimeZone);

            int daysUntilMonday =
                ((int)DayOfWeek.Monday - (int)israelNow.DayOfWeek + 7) % 7;

            if (daysUntilMonday == 0)
                daysUntilMonday = 7;

            DateTime nextMondayIsrael = DateTime.SpecifyKind(
                israelNow.Date.AddDays(daysUntilMonday),
                DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(
                nextMondayIsrael, israelTimeZone);
        }
    }
}
