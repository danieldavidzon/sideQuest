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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace sideQuest
{
        public class QuestResponse
        {
            [JsonProperty("quests")]
            public List<AIQuest> Quests { get; set; }
        }

        public class AIQuest
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("xp_reward")]
            public int XpReward { get; set; }

            [JsonProperty("photo_criteria")]
            public string PhotoCriteria { get; set; }
        }
    }

    
