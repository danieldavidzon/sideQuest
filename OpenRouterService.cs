//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using System;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using System.Collections.Generic;

//namespace sideQuest
//{
//    public class OpenRouterService
//    {
//        private static readonly HttpClient client = new HttpClient();
//        private const string BaseUrl = "https://openrouter.ai/api/v1/chat/completions";
//        private const string ModelName = "nvidia/nemotron-3.5-super-vl:free";

//        public async Task<string> GenerateWeeklyQuestsAsync()
//        {
//            var requestPayload = new
//            {
//                model = ModelName,
//                response_format = new { type = "json_object" },
//                messages = new[]
//                {
//                new { role = "system", content = "You are a backend game engine module. You must respond strictly in valid JSON format." },
//                new { role = "user", content = "Generate a list of 5 diverse weekly quests designed to push users out of their comfort zone. Return a JSON object containing a 'quests' array. Each object must include: 'id' (string), 'title' (string), 'description' (string), 'xp_reward' (int), and 'photo_criteria' (string)." }
//            }
//            };

//            return await SendRequestAsync(requestPayload);
//        }

//        public async Task<string> VerifyQuestPhotoAsync(string imageUrl, string questCriteria)
//        {
//            var requestPayload = new
//            {
//                model = ModelName,
//                response_format = new { type = "json_object" },
//                messages = new[]
//                {
//                new {
//                    role = "user",
//                    content = new object[]
//                    {
//                        new { type = "text", text = $"Act as a strict quest verification judge. The user submitted this photo to complete a quest. Criteria to pass: '{questCriteria}'. Return a JSON object with fields: 'verified' (bool) and 'reason' (string)." },
//                        new { type = "image_url", image_url = new { url = imageUrl } }
//                    }
//                }
//            }
//            };

//            return await SendRequestAsync(requestPayload);
//        }

//        public async Task<string> RotateWeeklyShopAsync(string masterItemListJson, string pastRotationsJson)
//        {
//            var requestPayload = new
//            {
//                model = ModelName,
//                response_format = new { type = "json_object" },
//                messages = new[]
//                {
//                new { role = "user", content = $"Master Items: {masterItemListJson}\nHistory: {pastRotationsJson}\nGenerate a fresh 5-slot weekly boost shop rotation. Return a JSON object with a 'shop_slots' array containing item names, descriptions, and current prices." }
//            }
//            };

//            return await SendRequestAsync(requestPayload);
//        }

//        private async Task<string> SendRequestAsync(object payload)
//        {
//            try
//            {
//                var jsonString = JsonConvert.SerializeObject(payload);
//                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

//                var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl);
//                request.Headers.Add("Authorization", $"Bearer {ApiKey}");
//                request.Content = content;

//                var response = await client.SendAsync(request);
//                var responseString = await response.Content.ReadAsStringAsync();

//                if (!response.IsSuccessStatusCode)
//                {
//                    throw new Exception($"API Error: {response.StatusCode} - {responseString}");
//                }

//                // You can parse this responseString to extract the model's actual JSON reply text

//                var result =
//                    JsonConvert.DeserializeObject<OpenRouterResponse>(responseString);

//                if (result?.Choices == null || result.Choices.Count == 0)
//                {
//                    throw new Exception("The API returned no choices.");
//                }

//                string modelReply = result.Choices[0].Message?.Content;

//                if (string.IsNullOrWhiteSpace(modelReply))
//                {
//                    throw new Exception("The API returned an empty response.");
//                }

//                return modelReply;

//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Error reaching OpenRouter: {ex.Message}");
//                return null;
//            }
//        }
//    }
//}