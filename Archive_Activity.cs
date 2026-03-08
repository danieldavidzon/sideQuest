using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace sideQuest
{
    [Activity(Label = "Archive_Activity")]
    public class Archive_Activity : Activity
    {
        private Database db = new Database();
        private List<CompletedQuest> quests = new List<CompletedQuest>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Archive);

            CalendarView calender = FindViewById<CalendarView>(Resource.Id.ArchiveCalendar);
            ListView questlist = FindViewById<ListView>(Resource.Id.QuestsList);
            TextView questCount = FindViewById<TextView>(Resource.Id.QuestCount);

            calender.DateChange += (sender, e) =>
            {
                string selectedDate = $"{e.DayOfMonth}/{e.Month + 1}/{e.Year}";
               

                questCount.Text = quests.Count > 0
                    ? $"{quests.Count} quest(s) completed on this day"
                    : "No quests completed on this day";

                var questNames = quests.Select(q => q.QuestName + " (+" + q.XP + " XP)").ToList();
                questlist.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, questNames);

                quests = db.GetQuestsByDate(selectedDate);
            };

            questlist.ItemClick += (sender, e) =>
            {
                var selectedQuest = quests[e.Position];

                Intent album = new Intent(this, typeof(QuestAlbum_Activity));
                album.PutExtra("QuestId", selectedQuest.Id);
                album.PutExtra("QuestName", selectedQuest.QuestName);
                album.PutExtra("QuestXP", selectedQuest.XP);
                album.PutExtra("StartTime", selectedQuest.StartTime);
                album.PutExtra("EndTime", selectedQuest.EndTime);
                album.PutExtra("Date", selectedQuest.Date);
                StartActivity(album);
            };
        }
    }
}