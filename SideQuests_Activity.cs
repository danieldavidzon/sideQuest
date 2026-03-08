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

namespace sideQuest
{
    [Activity(Label = "SideQuests_Activity")]
    public class SideQuests_Activity : Activity
    {
        private Database db = new Database();

        private List<sideQuestsList> sideQuestsList = new List<sideQuestsList>
        {
            new sideQuestsList("SideQuest1","example explanation",500),
            new sideQuestsList("SideQuest2","example explanation",500),
            new sideQuestsList("SideQuest3","example explanation",500),
            new sideQuestsList("SideQuest4","example explanation",500)
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SideQuests);

            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.sideQuestLayout);

            foreach (sideQuestsList quest in   sideQuestsList)
            {
                bool isComplete = db.IsQuestCompleted(quest.GetName());
                if (!isComplete)
                {
                    Button SideQuestBTN = new Button(this);
                    SideQuestBTN.Text = (quest.GetName() + "\nExplanation: " + quest.GetExplanation() + "\nXP: " + quest.GetXP());
                    SideQuestBTN.Tag = quest.GetName();
                    SideQuestBTN.TextAlignment = TextAlignment.TextStart;
                    SideQuestBTN.Click += (sender, e) =>
                    {
                        Button btn = sender as Button;
                        Intent complete = new Intent(this, typeof(completeSideQuests));
                        complete.PutExtra("QuestName", quest.GetName());
                        complete.PutExtra("QuestExplanation", quest.GetExplanation());
                        complete.PutExtra("QuestXP", quest.GetXP());
                        StartActivity(complete);
                    };
                    layout.AddView(SideQuestBTN);
                }
            }
        }

        
    }
}