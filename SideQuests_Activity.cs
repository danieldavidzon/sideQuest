using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace sideQuest
{
    [Activity(Label = "SideQuests_Activity")]
    public class SideQuests_Activity : Activity
    {
        private Database db = new Database();
        private OpenRouterService aiService = new OpenRouterService();
        private LinearLayout layout;

        private List<sideQuestsList> sideQuestsList = new List<sideQuestsList>
        {
            new sideQuestsList("SideQuest1","example explanation",500,50),
            new sideQuestsList("SideQuest2","example explanation",500,50),
            new sideQuestsList("SideQuest3","example explanation",500,50),
            new sideQuestsList("SideQuest4","example explanation",500,50)
        };
        //protected override void OnCreate(Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);
        //    SetContentView(Resource.Layout.SideQuests);

        //    LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.sideQuestLayout);

        //    foreach (sideQuestsList quest in   sideQuestsList)
        //    {
        //        bool isComplete = db.IsQuestCompleted(quest.GetName());
        //        if (!isComplete)
        //        {
        //            Button SideQuestBTN = new Button(this);
        //            SideQuestBTN.Text = (quest.GetName() + "\nExplanation: " + quest.GetExplanation() + "\nXP: " + quest.GetXP());
        //            SideQuestBTN.Tag = quest.GetName();
        //            SideQuestBTN.TextAlignment = TextAlignment.TextStart;
        //            SideQuestBTN.Click += (sender, e) =>
        //            {
        //                Button btn = sender as Button;
        //                Intent complete = new Intent(this, typeof(completeSideQuests));
        //                complete.PutExtra("QuestName", quest.GetName());
        //                complete.PutExtra("QuestExplanation", quest.GetExplanation());
        //                complete.PutExtra("QuestXP", quest.GetXP());
        //                StartActivity(complete);
        //            };
        //            layout.AddView(SideQuestBTN);
        //        }
        //    }
        //}
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SideQuests);

            layout = FindViewById<LinearLayout>(
                Resource.Id.sideQuestLayout);

            TextView loadingText = new TextView(this)
            {
                Text = "Loading quests...",
                TextSize = 22,
                Gravity = GravityFlags.Center,
                LayoutParameters = new LinearLayout.LayoutParams(
        LinearLayout.LayoutParams.MatchParent,
        LinearLayout.LayoutParams.MatchParent)
            };

            loadingText.SetTextColor(Android.Graphics.Color.White);
            loadingText.SetBackgroundColor(Android.Graphics.Color.Black);

            layout.AddView(loadingText);

            await LoadQuestsAsync(loadingText);
        }
        private async Task LoadQuestsAsync(TextView loadingText)
        {
            try
            {
                string json = await aiService.GenerateWeeklyQuestsAsync();

                if (string.IsNullOrWhiteSpace(json))
                {
                    loadingText.Text = "Failed to load quests.";
                    return;
                }

                var result = JsonConvert.DeserializeObject<QuestResponse>(json);

                if (result?.Quests == null || result.Quests.Count == 0)
                {
                    loadingText.Text = "No quests were returned.";
                    return;
                }

                layout.RemoveAllViews();

                foreach (var quest in result.Quests)
                {
                    bool isComplete = db.IsQuestCompleted(quest.Id);

                    if (isComplete)
                        continue;

                    Button questButton = new Button(this);

                    questButton.Text =
                        $"{quest.Title}\n{quest.Description}\nXP: {quest.XpReward}";

                    questButton.Tag = quest.Id;

                    questButton.TextAlignment = TextAlignment.TextStart;

                    questButton.Click += (sender, e) =>
                    {
                        Intent complete = new Intent(
                            this, typeof(completeSideQuests));

                        complete.PutExtra("QuestName", quest.Title);
                        complete.PutExtra("QuestExplanation", quest.Description);
                        complete.PutExtra("QuestXP", quest.XpReward);

                        StartActivity(complete);
                    };

                    layout.AddView(questButton);
                }
            }
            catch (Exception ex)
            {
                loadingText.Text = "Error loading quests: " + ex.Message;
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


    }
}