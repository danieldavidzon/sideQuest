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
using Android.Provider;
using Android.Net;
using Java.IO;
using AndroidX.Core.Content;

namespace sideQuest
{
    [Activity(Label = "completeSideQuests")]
    public class completeSideQuests : Activity
    {
        private string SelectedQuestName, SelectedQuestExplanation;
        private int SelectedQuestXP;

        private Button date;
        private Button start;
        private Button end;
        private Database db = new Database();

        private Android.Net.Uri photoUri;
        private static readonly int CameraRequestCode = 1;

        private string lastPhotoPath;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.compleSideQuests);

            SelectedQuestName = Intent.GetStringExtra("QuestName");
            SelectedQuestExplanation = Intent.GetStringExtra("QuestExplanation");
            SelectedQuestXP = Intent.GetIntExtra("QuestXP",0);

            TextView QuestText = FindViewById<TextView>(Resource.Id.QuestText);
            QuestText.Text = SelectedQuestName+"\nExplanation: "+SelectedQuestExplanation+"\nXP: "+SelectedQuestXP;


            date = FindViewById<Button>(Resource.Id.DateBTN);
            date.Click += (sender, e) =>
            {
                new DatePickerDialog(this, (sender, args) =>
                {
                    date.Text = $"{args.Date.Day}/{args.Date.Month}/{args.Date.Year}";
                }, DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day).Show();
            };
            start = FindViewById<Button>(Resource.Id.StartTimePicker);
            end = FindViewById<Button>(Resource.Id.EndTimePicker);

            start.Click += (sender, e) =>
            {
                new TimePickerDialog(this, (sender, args) =>
                {
                    start.Text = $"{args.HourOfDay}:{args.Minute:D2}";
                }, DateTime.Now.Hour, DateTime.Now.Minute, true).Show();
            };
            end.Click += (sender, e) =>
            {
                new TimePickerDialog (this, (sender, args) =>
                {
                    end.Text = $"{args.HourOfDay}:{args.Minute:D2}";
                },DateTime.Now.Hour, DateTime.Now.Minute, true).Show();
            };

            Button cameraBtn = FindViewById<Button>(Resource.Id.CameraBtn);
            cameraBtn.Click += async (sender, e) =>
            {
                // request camera permission at runtime
                if (CheckSelfPermission(Android.Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                {
                    RequestPermissions(new string[] { Android.Manifest.Permission.Camera }, 0);
                    return;
                }

                Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(cameraIntent, CameraRequestCode);
            };

            Button completeQuest = FindViewById<Button>(Resource.Id.CompleteQuest);
            completeQuest.Click += CompleteQuest_Click;
        }

        private void CompleteQuest_Click(object sender, EventArgs e)
        {
            string SaveDate = date.Text;
            string SaveStart = start.Text;
            string SaveEnd = end.Text;

            CompletedQuest quest = new CompletedQuest
            {
                QuestName = SelectedQuestName,
                XP = SelectedQuestXP,
                Date = SaveDate,
                StartTime = SaveStart,
                EndTime = SaveEnd,
                Saved = 1
            };
            db.SaveQuest(quest);

            // save the photo linked to this quest if one was taken
            if (lastPhotoPath != null)
            {
                db.SavePhoto(quest.Id, lastPhotoPath);
            }

            Intent archive = new Intent(this, typeof (Archive_Activity));
            StartActivity(archive);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == CameraRequestCode && resultCode == Result.Ok)
            {
                Android.Graphics.Bitmap photo = (Android.Graphics.Bitmap)data.Extras.Get("data");

                string photoPath = System.IO.Path.Combine(
                    GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).AbsolutePath,
                    $"quest_{DateTime.Now.Ticks}.jpg"
                );

                using (var stream = new System.IO.FileStream(photoPath, System.IO.FileMode.Create))
                {
                    photo.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
                }

                lastPhotoPath = photoPath;
                Toast.MakeText(this, "Photo saved!", ToastLength.Short).Show();
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
            {
                // permission granted, open camera
                Intent cameraIntent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(cameraIntent, CameraRequestCode);
            }
            else
            {
                Toast.MakeText(this, "Camera permission is required!", ToastLength.Short).Show();
            }
        }
    }
}