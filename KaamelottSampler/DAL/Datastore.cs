using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using KaamelottSampler.Models;
using Newtonsoft.Json;

namespace KaamelottSampler.DAL
{
    public class Datastore
    {
        public async static Task<List<Sample>> GetSamplesFromAssetsAsync(Context context, string filename)
        {
            string content = "";
            using (StreamReader sr = new StreamReader(context.Assets.Open(filename)))
            {
                content = await sr.ReadToEndAsync();
            }
            return JsonConvert.DeserializeObject<List<Sample>>(content);

        }

        public async static Task<List<Sample>> GetSamplesFromWebAsync(Context context)
        {
            string content = "";
            using (HttpClient client = new HttpClient())
            {
                content = await client.GetStringAsync(@"http://www.la-mobile-it.com/wp-content/uploads/2017/05/sounds.txt");
            }
            var result = await Task.Factory.StartNew(() =>
            {
                List<Sample> tmp = JsonConvert.DeserializeObject<List<Sample>>(content);
                return tmp;
            });
            return result;


        }
    }
}