using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sideQuest.Resources.layout
{
    [Activity(Label = "Home_Activity")]
    public class Home_Activity : Activity
    {
        private Database db = new Database();
        private List<CompletedQuest> Quests = new List<CompletedQuest>();
        Button ArchiveBTN, SideQuestsBTN;
        TextView XPCounter;
        int XP = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.Home);
            ArchiveBTN = FindViewById<Button>(Resource.Id.ArchiveBTN);
            SideQuestsBTN = FindViewById<Button>(Resource.Id.SideQuestsBTN);
            XPCounter = FindViewById<TextView>(Resource.Id.XPCounter);
            XP += Intent.GetIntExtra("XP", 0);
            XPCounter.Text = $"XP: {XP.ToString()}";
            


            SideQuestsBTN.Click += SideQuestsBTN_Click;
            ArchiveBTN.Click += ArchiveBTN_Click;
        }

        private void ArchiveBTN_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(Archive_Activity));
        }

        private void SideQuestsBTN_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SideQuests_Activity));
        }
    }
}