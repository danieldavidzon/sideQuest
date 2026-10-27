using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sideQuest
{
    public class OpenRouterResponse
    {
        [JsonProperty("choices")]
        public List<OpenRouterChoice> Choices { get; set; }
    }

    public class OpenRouterChoice
    {
        [JsonProperty("message")]
        public OpenRouterMessage Message { get; set; }
    }

    public class OpenRouterMessage
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}