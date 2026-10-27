using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SideQuestAPI.Controllers
{

    using Microsoft.AspNetCore.Mvc;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;

    namespace SideQuest.API.Controllers
    {
        [ApiController]
        [Route("api/ai")]
        public class AIController : ControllerBase
        {
            private readonly IConfiguration _configuration;
            private readonly IHttpClientFactory _httpClientFactory;
            public AIController(
                IConfiguration configuration,
                IHttpClientFactory httpClientFactory)
            {
                _configuration = configuration;
                _httpClientFactory = httpClientFactory;
            }

            [HttpGet("quests")]
            public async Task<IActionResult> GenerateQuests()
            {
                string? apiKey =
                    _configuration["OpenRouter:ApiKey"];

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    return StatusCode(500,
                        "OpenRouter API key is not configured.");
                }

                var client = _httpClientFactory.CreateClient();

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

                var json = JsonSerializer.Serialize(payload);

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://openrouter.ai/api/v1/chat/completions");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                request.Content = new StringContent(
                    json, Encoding.UTF8, "application/json");

                using var response = await client.SendAsync(request);

                var responseBody =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        "OpenRouter request failed: " + responseBody);
                }

                using var document = JsonDocument.Parse(responseBody);

                var content = document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrWhiteSpace(content))
                {
                    return StatusCode(502,
                        "OpenRouter returned an empty response.");
                }

                // Return the generated JSON as JSON, not as a quoted string.
                using var questDocument = JsonDocument.Parse(content);

                return new JsonResult(questDocument.RootElement.Clone());
            }
        }
    }

}
