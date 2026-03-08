using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.IO;
using sideQuest.Resources.layout;

namespace sideQuest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button StartBTN;
        Button clearBTN;
        private Database db = new Database();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            SetContentView(Resource.Layout.activity_main);
            StartBTN = FindViewById<Button>(Resource.Id.StartBTN);
            StartBTN.Click += StartBTN_Click;

            clearBTN = FindViewById<Button>(Resource.Id.ClearDB);
            clearBTN.Click += (sendet, e) =>
            {
                db.ClearDatabase();
                Toast.MakeText(this, "DATABASE CLEARED!", ToastLength.Short).Show();
            };
        }
        

        private void StartBTN_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(Home_Activity));
        }
    }
}