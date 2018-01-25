
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.IO;
using SQLite;

namespace RiverLevelApp
{

    public class StationData
    {
        public StationDetail Detail { get; set; }


        public float? Change { get; set; }

        public string NotificationText => $@"River level at {Station.town} as of {LatestLevel.dateTime.ToString("HH:mm")}: {LatestLevel.value*100}cm" + (Change != null ? $" ({(Change*100f)?.ToString("{0:+0.0;-0.0;0.0}")})" : "");

        public string ButtonText =>  $@"{Station.town} @ {LatestLevel.dateTime.ToString("HH:mm")}  :  {LatestLevel.value * 100}cm" + (Change != null ? $" ({(Change * 100f)?.ToString("{0:+0.0;-0.0;0.0}")})" : "");


        public Level LatestLevel => Station.measures.latestReading;

        public StationDetail.Items Station => Detail.items;

    }


    public class StationButton : Button
    {
        public StationData StationData { get; set; }

    }

    public class Settings
    {
        public string River { get; set; }
    }


    public partial class MainPage : ContentPage
	{

        private List<Stations.Item> _stations;
        private List<StationData> data = new List<StationData>();
        public string _river;

        public void StationClicked(object sender, EventArgs e)
        {
            var f = new StationPage((sender as StationButton).StationData);
            Application.Current.MainPage.Navigation.PushModalAsync(f);
        }

        public MainPage()
		{
			InitializeComponent();
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"RiverLevel.db3");

            var db = new SQLiteConnection(dbPath);
            db.CreateTable<Settings>();
            if (!db.Table<Settings>().Any())
            {
                db.Insert(new Settings() { River = "" });
            }
            _river = db.Table<Settings>().First().River;
            header.Text = "River Levels - " + _river;
            //var serviceConnnection = new RiverLevelApp.RiverLevelServiceConnection();

            Task.Run(async () =>
            {
                int retries = 5;
                while (true)
                {

                    Device.BeginInvokeOnMainThread(() =>
                    {

                        status.Text = "Loading stations...";
                    });
                    try
                    {
                        _stations = await GetStations();
                        await GetLevels();
                        break;
                    }
                    catch (Exception ex) when (retries<5)
                    {
                        retries++;
                        ReportError(ex);
                        Thread.Sleep(5000);

                    } catch (Exception ex)
                    {
                        ReportError(ex);
                    }

                }
            });



            Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                // Recude hits as it tends to happen around 23 minutes after. TODO: increase this in flood situation
                if (DateTime.Now.Minute % 15 == 9)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            _stations = _stations ?? await GetStations();
                            await GetLevels();
                        }
                        catch (Exception ex)
                        {
                            ReportError(ex);
                        }

                    });
                }
                return true;
            });

        }


        private void ReportError(Exception ex)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                status.Text = $"Error: " + ex.Message;
            });
 
        }

        private async Task<List<Stations.Item>> GetStations()
        {
            return (await Stations.GetAsync(_river)).items.ToList();
        }


        private async Task GetLevels()
        {
            if (_stations == null) return;
            Device.BeginInvokeOnMainThread(() =>
            {                
                status.Text = "Loading data...";
            });


            List<StationData> newStationData = new List<StationData>();

            foreach (var s in _stations)
            {
                var r = await StationDetail.GetAsync(s.stationReference);




                var newdata = new StationData { Detail = r };

                var olddata = data?.OrderByDescending(x=>x.LatestLevel.dateTime).FirstOrDefault(x => x.Station.stationReference == newdata.Station.stationReference && newdata.LatestLevel.dateTime != x.LatestLevel.dateTime);
                if (olddata != null)
                {
                    newdata.Change = newdata.LatestLevel.value - olddata.LatestLevel.value;             
                }
                newStationData.Add(newdata);        
            }

            data = newStationData;



            if (data.Count>_stations.Count())
            {
                data.Clear();
                throw new Exception("Too much station data detected");
            }


            if (data.Any(x => x.Change != null))
            {
                DependencyService.Get<INotification>().SendNotification("River Levels", string.Join("\r\n", data.Select(x => x.NotificationText)));
            }

            Device.BeginInvokeOnMainThread(() =>
            {

   
                datacontainer.Children.Clear();
                status.Text = $"Data loaded at {DateTime.Now.ToString("HH:mm")}";
                foreach (var d in data)
                {
                    
                    var b = new StationButton() { Text = d.ButtonText, StationData = d };
                    if (d.LatestLevel.value > d.Station.stageScale.typicalRangeHigh)
                    {
                        b.BackgroundColor = Color.Salmon;
                    }
                    b.Clicked += StationClicked;
                    datacontainer.Children.Add(b);
                }

            });
        }
	}


}
