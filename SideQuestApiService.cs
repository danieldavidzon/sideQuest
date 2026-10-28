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
    }
}