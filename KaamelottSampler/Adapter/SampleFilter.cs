using System;
using System.Collections.Generic;
using System.Linq;
using Android.Widget;
using Java.Lang;
using KaamelottSampler.Adapter;
using KaamelottSampler.Models;

namespace KaamelottSampler.Adapter
{
    public class SampleFilter : Filter
    {
        private readonly SampleAdapter myAdapter;

        public SampleFilter(SampleAdapter adapter)
        {
            myAdapter = adapter;
        }
        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var returnedObj = new FilterResults();
            var results = new List<Sample>();

            if (myAdapter.OriginalData == null)
                myAdapter.OriginalData = myAdapter.Items;

            if (constraint == null) return returnedObj;

            if (myAdapter.OriginalData != null && myAdapter.OriginalData.Any())
            {
                var filter =  constraint.ToString().ToLower().RemoveDiacritics();
                results.AddRange(
                    myAdapter.OriginalData.Where(x => x.Title.ToLower().RemoveDiacritics().Contains(filter) ||
                                                x.Character.ToLower().RemoveDiacritics().Contains(filter)));
            }

            returnedObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
            returnedObj.Count = results.Count;

            constraint.Dispose();

            return returnedObj;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            using (var values = results.Values)
            {
                myAdapter.Items = values.ToArray<Java.Lang.Object>().Select(r => r.ToNetObject<Sample>()).ToList();
            }

            myAdapter.NotifyDataSetChanged();

            constraint.Dispose();
            results.Dispose();
        }
    }
}