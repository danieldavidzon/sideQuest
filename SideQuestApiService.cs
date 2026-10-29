using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sideQuest
{
    public class SideQuestApiService
    {
        private static readonly HttpClient client = new HttpClient();

        // Replace PORT with your backend's HTTP port.
        private const string BaseUrl = "http://192.168.167.25:5094";

        public async Task<string> GenerateWeeklyQuestsAsync()
        {
            try
            {
                string url = $"{BaseUrl}/api/ai/quests";

                HttpResponseMessage response =
                    await client.GetAsync(url);
                    
                string responseBody =
                    await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Backend error: {(int)response.StatusCode} - {responseBody}");
                }

                return responseBody;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"SideQuest API error: {ex}");

                throw;
            }
        }

        public async Task<DateTime> GetNextRerollAsync()
        {
            string url = $"{BaseUrl}/api/ai/weekly-status";

            HttpResponseMessage response = await client.GetAsync(url);

            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Backend error: {(int)response.StatusCode} - {responseBody}");
            }

            var result = Newtonsoft.Json.JsonConvert
                .DeserializeObject<WeeklyStatusResponse>(responseBody);

            if (result == null || string.IsNullOrWhiteSpace(result.NextRerollUtc))
                throw new Exception("Invalid weekly status response.");

            return DateTime.Parse(
                result.NextRerollUtc,
                null,
                System.Globalization.DateTimeStyles.AdjustToUniversal);
        }

    }
    public class WeeklyStatusResponse
    {
        [Newtonsoft.Json.JsonProperty("nextRerollUtc")]
        public string NextRerollUtc { get; set; }
    }
}