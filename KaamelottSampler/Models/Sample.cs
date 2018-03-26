using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace KaamelottSampler.Models
{
    public class Sample
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonProperty("episode")]
        public string Episode { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }

        [JsonIgnore]
        public string ImageCharacter
        {
            get
            {
                return Character.ToLower().Trim()
                    .Replace("é", "e")
                    .Replace("è", "e")
                    .Replace("ê", "e")
                    .Replace("î", "i")
                    .Replace(@"'", "");

            }
        }

        public override string ToString()
        {
            return Title + " - " + Character + " - " + Episode;
        }

        internal object ToJavaObject()
        {
            throw new NotImplementedException();
        }
    }
}
