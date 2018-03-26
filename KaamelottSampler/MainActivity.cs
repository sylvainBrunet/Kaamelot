using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using KaamelottSampler.DAL;
using KaamelottSampler.Models;
using System.Collections.Generic;
using Android.Media;
using KaamelottSampler.Adapter;
using System.Linq;
using System;

namespace KaamelottSampler
{
    [Activity(Label = "KaamelottSampler", MainLauncher = true)]
    public class MainActivity : Activity
    {
        ListView mylist;
        EditText myTextviewsearch;
        List<Sample> listesamplefull;
        List<Sample> listesamplecurrent;
        MediaPlayer mediaPlayer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //On récupère les éléments pour les manipuler
            mylist = FindViewById<ListView>(Resource.Id.main_listView_Samples);
            myTextviewsearch = FindViewById<EditText>(Resource.Id.main_editText_search);
            

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Prepared += (s, e) =>
            {
                mediaPlayer.Start();
            };

            LoadData();
            myTextviewsearch.TextChanged += MyTextviewsearch_TextChanged;
            mylist.ItemClick += Mylist_ItemClick;
            

        }

        protected override void OnResume()
        {
            base.OnResume();
            Task.Factory.StartNew(async () =>
            {
                //On charge les données
                await LoadData().ContinueWith(t =>
                {
                    AndroidHUD.AndHUD.Shared.Dismiss();
                });
            });
        }

        private async void MyTextviewsearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            //méthode CRADE !!!
            await LoadFilteredData(e.Text.ToString());
        }

        private async Task LoadData()
        {
            AndroidHUD.AndHUD.Shared.Show(this, "Loading...", -1, AndroidHUD.MaskType.Black, null, null, true, null);
            listesamplefull = await Datastore.GetSamplesFromWebAsync(this); 
            //listesamplefull = await Datastore.GetSamplesFromAssetsAsync(this, "sounds.json");
            await LoadFilteredData("");
        }

        private async Task LoadFilteredData(string filter)
        {
            listesamplecurrent = listesamplefull;
            if (!String.IsNullOrWhiteSpace(filter))
            {
                listesamplecurrent = listesamplefull.Where(x => x.Title.Contains(filter)).ToList();
                //On crée notre CustomAdapter
                SampleAdapter myadapter = new SampleAdapter(this, listesamplecurrent);
                //On assigne notre customAdapter à notre listview
                mylist.Adapter = myadapter;
            }
            else
            {
                SampleAdapter myadapter = new SampleAdapter(this, listesamplecurrent);
                //On assigne notre customAdapter à notre listview
                mylist.Adapter = myadapter;
            }


        }

        private void Mylist_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Sample clikedsample = listesamplecurrent[e.Position];
            Task.Factory.StartNew(async () =>
            {
                await PlayAudioFileAsync(clikedsample.File);
            });
        }

        public async Task PlayAudioFileAsync(string fileName)
        {
            await Task.Factory.StartNew(() =>
            {
                
                var fd = this.ApplicationContext.Assets.OpenFd(fileName);
                if (mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Stop();
                }
                mediaPlayer.Reset();
                mediaPlayer.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                mediaPlayer.Prepare();
            });
        }
    }
}

