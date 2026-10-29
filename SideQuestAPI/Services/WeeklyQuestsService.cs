
using Microsoft.EntityFrameworkCore;
using SideQuestAPI.Data;
using SideQuestAPI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SideQuestAPI.Services
{
    public class WeeklyQuestService
    {
        private readonly SideQuestDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public WeeklyQuestService(
            SideQuestDbContext db,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<WeeklyQuest>> GetOrGenerateWeeklyQuestsAsync()
        {
            DateTime weekStart = GetCurrentWeekStart();

            var quests = await _db.WeeklyQuests
                .Where(q => q.WeekStartUtc == weekStart && q.IsActive)
                .ToListAsync();

            if (quests.Count > 0)
                return quests;

            var generatedQuests = await GenerateQuestsWithAIAsync();

            // Deactivate previous quests.
            var oldQuests = await _db.WeeklyQuests
                .Where(q => q.IsActive)
                .ToListAsync();

            foreach (var oldQuest in oldQuests)
                oldQuest.IsActive = false;

            // Save the new batch.
            foreach (var quest in generatedQuests)
            {
                _db.WeeklyQuests.Add(new WeeklyQuest
                {
                    QuestId = string.IsNullOrWhiteSpace(quest.id)
                        ? Guid.NewGuid().ToString()
                        : quest.id,
                    Title = quest.title,
                    Description = quest.description,
                    XpReward = quest.xp_reward,
                    PhotoCriteria = quest.photo_criteria,
                    WeekStartUtc = weekStart,
                    IsActive = true
                });
            }

            await _db.SaveChangesAsync();

            return await _db.WeeklyQuests
                .Where(q => q.WeekStartUtc == weekStart && q.IsActive)
                .ToListAsync();
        }

        // Monday at 00:00 in Israel.
        public static DateTime GetCurrentWeekStart()
        {
            TimeZoneInfo israelTimeZone;

            try
            {
                israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                    OperatingSystem.IsWindows()
                        ? "Israel Standard Time"
                        : "Asia/Jerusalem");
            }
            catch
            {
                israelTimeZone = TimeZoneInfo.Utc;
            }

            DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, israelTimeZone);

            DateTime monday = israelNow.Date.AddDays(
                -(((int)israelNow.DayOfWeek + 6) % 7));

            return TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(monday, DateTimeKind.Unspecified),
                israelTimeZone);
        }

        public static DateTime GetNextRerollUtc()
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


        private async Task<List<GeneratedQuest>> GenerateQuestsWithAIAsync()
        {
            string? apiKey = _configuration["OpenRouter:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "OpenRouter API key is not configured.");

            var payload = new
            {
                model = "nvidia/nemotron-3.5-super-vl:free",
                response_format = new { type = "json_object" },
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Return only valid JSON."
                    },
                    new
                    {
                        role = "user",
                        content =
                            "Generate 5 diverse real-life SideQuest missions. " +
                            "Return a JSON object with a 'quests' array. " +
                            "Each quest must have id, title, description, " +
                            "xp_reward (integer), and photo_criteria."
                    }
                }
            };

            string json = JsonSerializer.Serialize(payload);
            var client = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://openrouter.ai/api/v1/chat/completions");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            request.Content = new StringContent(
                json, Encoding.UTF8, "application/json");

            using var response = await client.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(
                    $"OpenRouter error {(int)response.StatusCode}: {body}");

            using var document = JsonDocument.Parse(body);

            string? content = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("OpenRouter returned empty content.");

            using var questDocument = JsonDocument.Parse(content);

            if (!questDocument.RootElement.TryGetProperty(
                "quests", out var questsElement))
                throw new Exception("AI response has no quests array.");

            var quests = JsonSerializer.Deserialize<List<GeneratedQuest>>(
                questsElement.GetRawText());

            if (quests == null || quests.Count == 0)
                throw new Exception("AI returned no quests.");

            return quests;
        }

        private class GeneratedQuest
        {
            public string id { get; set; } = Guid.NewGuid().ToString();
            public string title { get; set; } = "";
            public string description { get; set; } = "";
            public int xp_reward { get; set; }
            public string photo_criteria { get; set; } = "";
        }
    }
}
