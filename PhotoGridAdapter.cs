using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace sideQuest
{
    public class PhotoGridAdapter : BaseAdapter<string>
    {
        private Context context;
        private List<string> photoPaths;

        public PhotoGridAdapter(Context context, List<string> photoPaths)
        {
            this.context = context;
            this.photoPaths = photoPaths;
        }

        public override string this[int position] => photoPaths[position];
        public override int Count => photoPaths.Count;
        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(250, 250);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            }
            else
            {
                imageView = (ImageView)convertView;
            }

            // load the photo from the saved path
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPaths[position]);
            imageView.SetImageBitmap(bitmap);

            return imageView;
        }
    }
}