using System;
using Java.Lang;
using KaamelottSampler.Models;
using SearchViewSample;
using Object = Java.Lang.Object;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;

namespace KaamelottSampler.Adapter
{
    public class SampleAdapter : BaseAdapter<Sample>, IFilterable
    {
        List<Sample> originalData;
        List<Sample> items;
        Activity context;

        public SampleAdapter(Activity context, List<Sample> items)
       : base()
        {
            this.context = context;
            this.items = items;
            Filter = new SampleFilter(this);

        }
        public List<Sample> Items
        {
            get => items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Sample this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }

        public Filter Filter { get; private set; }
        public override void NotifyDataSetChanged()
        {
            // If you are using cool stuff like sections
            // remember to update the indices here!
            base.NotifyDataSetChanged();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.MyCustomRow, null);
            view.FindViewById<TextView>(Resource.Id.Textview_Title).Text = item.Title;
            view.FindViewById<TextView>(Resource.Id.Textview_Episode).Text = item.Episode;
            //On essaye de rechercher l'image dans les drawable
            int rescharid = context.Resources.GetIdentifier(item.ImageCharacter, "drawable", context.PackageName);
            if (rescharid != 0)
            {
                view.FindViewById<ImageView>(Resource.Id.Imageview_character).SetImageResource(rescharid);
            }
            else
            {
                view.FindViewById<ImageView>(Resource.Id.Imageview_character).SetImageResource(Resource.Drawable.profile_generic);
            }
            return view;
        }

        private class SampleFilter : Filter
        {
            private readonly SampleAdapter _adapter;
            public SampleFilter(SampleAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<Sample>();
                if (_adapter.originalData == null)
                    _adapter.originalData = _adapter.items;

                if (constraint == null) return returnObj;

                if (_adapter.originalData != null && _adapter.originalData.Any())
                {
                    // Compare constraint to all names lowercased. 
                    // It they are contained they are added to results.
                    results.AddRange(
                        _adapter.originalData.Where(
                            sample => sample.Title.ToLower().Contains(constraint.ToString())));
                }

                // Nasty piece of .NET to Java wrapping, be careful with this!
                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                    _adapter.items = values.ToArray<Object>()
                        .Select(r => r.ToNetObject<Sample>()).ToList();

                _adapter.NotifyDataSetChanged();

                // Don't do this and see GREF counts rising
                constraint.Dispose();
                results.Dispose();
            }
        }



        public static implicit operator SimpleAdapter(SampleAdapter v)
        {
            throw new NotImplementedException();
        }
    }
}