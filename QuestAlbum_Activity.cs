using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sideQuest
{
    [Activity(Label = "QuestAlbum_Activity")]
    public class QuestAlbum_Activity : Activity
    {
        private Database db = new Database();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.QuestAlbum);

            int questId = Intent.GetIntExtra("QuestId", 0);
            string questName = Intent.GetStringExtra("QuestName");
            int questXP = Intent.GetIntExtra("QuestXP", 0);
            string startTime = Intent.GetStringExtra("StartTime");
            string EndTime = Intent.GetStringExtra("EndTime");

            TextView albumInfo = FindViewById<TextView>(Resource.Id.AlbumQuestInfo);
            albumInfo.Text = $"{questName}\nXP: {questXP}\n{startTime}-{EndTime}";

            GridView PhotosGrid = FindViewById<GridView>(Resource.Id.PhotosGrid);

            if (CheckSelfPermission(Android.Manifest.Permission.ReadMediaImages) != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.ReadMediaImages }, 1);
            }

            var appPhotos = db.GetPhotosForQuest(questId).Select(p => p.PhotoPath).ToList();
            var galleryPhotos = GetPhotosFromGallery(
                Intent.GetStringExtra("Date"),
                startTime,
                EndTime
            );

            // combine both lists without duplicates
            var allPhotos = appPhotos.Union(galleryPhotos).ToList();
            PhotosGrid.Adapter = new PhotoGridAdapter(this, allPhotos);
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 1 && grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
            {
                // reload the grid with gallery photos now that permission is granted
                var appPhotos = db.GetPhotosForQuest(Intent.GetIntExtra("QuestId", 0)).Select(p => p.PhotoPath).ToList();
                var galleryPhotos = GetPhotosFromGallery(Intent.GetStringExtra("Date"), Intent.GetStringExtra("StartTime"), Intent.GetStringExtra("EndTime"));
                var allPhotos = appPhotos.Union(galleryPhotos).ToList();
                FindViewById<GridView>(Resource.Id.PhotosGrid).Adapter = new PhotoGridAdapter(this, allPhotos);
            }
        }
        private List<string> GetPhotosFromGallery(string date, string startTime, string endTime)
        {
            List<string> photoPaths = new List<string>();

            // parse date and times
            var dateParts = date.Split('/');
            var startParts = startTime.Split(':');
            var endParts = endTime.Split(':');

            DateTime start = new DateTime(
                int.Parse(dateParts[2]),  // year
                int.Parse(dateParts[1]),  // month
                int.Parse(dateParts[0]),  // day
                int.Parse(startParts[0]), // hour
                int.Parse(startParts[1]), // minute
                0
            );

            DateTime end = new DateTime(
                int.Parse(dateParts[2]),
                int.Parse(dateParts[1]),
                int.Parse(dateParts[0]),
                int.Parse(endParts[0]),
                int.Parse(endParts[1]),
                0
            );

            // convert to Unix timestamps (what Android uses)
            long startUnix = ((DateTimeOffset)start).ToUnixTimeSeconds();
            long endUnix = ((DateTimeOffset)end).ToUnixTimeSeconds();

            // query the gallery
            Android.Net.Uri uri = MediaStore.Images.Media.ExternalContentUri;
            string[] projection = { MediaStore.Images.Media.InterfaceConsts.Data };
            string selection = $"{MediaStore.Images.Media.InterfaceConsts.DateTaken} >= ? AND {MediaStore.Images.Media.InterfaceConsts.DateTaken} <= ?";
            string[] selArgs = { (startUnix * 1000).ToString(), (endUnix * 1000).ToString() };

            var cursor = ContentResolver.Query(uri, projection, selection, selArgs, null);

            if (cursor != null && cursor.MoveToFirst())
            {
                do
                {
                    int colIndex = cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data);
                    photoPaths.Add(cursor.GetString(colIndex));
                }
                while (cursor.MoveToNext());
                cursor.Close();
            }

            return photoPaths;
        }

        
    }
}