using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using RiverLevelApp.ViewModel;
using SQLite;
using Xamarin.Forms;

namespace RiverLevelApp.ViewModels
{
    internal class MainPageViewModel : INotifyPropertyChanged
    {
        private List<Stations.Item> _stationsRawData;
        public ObservableCollection<Station> Stations { get; set; }

        private string _river;
        public string Header { get; set; }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status!=value) OnPropertyChanged("Status");
                _status = value;
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value) OnPropertyChanged("IsBusy");
                _isBusy = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MainPageViewModel()
        {
            SetRiverFromDatabase();
            //var serviceConnnection = new RiverLevelApp.RiverLevelServiceConnection();
        }

        private void SetRiverFromDatabase()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "RiverLevel.db3");
            var db = new SQLiteConnection(dbPath);
            db.CreateTable<Settings>();
            if (!db.Table<Settings>().Any())
            {
                db.Insert(new Settings() {River = ""});
            }

            _river = db.Table<Settings>().First().River;
            Header = "River Levels For " + _river;
        }

        public async Task GetData()
        {
            int retries = 1;
            while (true)
            {
                Status = $"Loading stations (attempt {retries})...";
                try
                {
                    _stationsRawData = await GetStationsRawData();
                    await UpdateStationAsync();
                    break;
                }
                catch (Exception ex) when (retries < 5)
                {
                    retries++;
                    ReportError(ex);
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                }
            }
        }


        private void ReportError(Exception ex)
        {
                Status = $"Error: " + ex.Message;
        }

        private async Task<List<Stations.Item>> GetStationsRawData()
        {
            var t = await RiverLevelApp.Stations.GetAsync(_river);
            return t.items.ToList();
        }


        private async Task UpdateStationAsync()
        {
            if (_stationsRawData == null) return;
            Status = "Loading levels data...";

            ObservableCollection<Station> newStationData = new ObservableCollection<Station>();

            foreach (var s in _stationsRawData)
            {
                // Reget the station
                var r = await StationDetailRaw.GetAsync(s.stationReference);




                var newdata = new Station { Detail = r };

                var olddata = Stations?.OrderByDescending(x => x.LatestLevel.dateTimeTimeZone).FirstOrDefault(x => x.StationDetails.stationReference == newdata.StationDetails.stationReference && newdata.LatestLevel.dateTimeTimeZone != x.LatestLevel.dateTimeTimeZone);
                if (olddata != null)
                {
                    newdata.Change = newdata.LatestLevel.value - olddata.LatestLevel.value;
                }
                newStationData.Add(newdata);
            }

            Stations = newStationData;

            


            if (Stations.Count > _stationsRawData.Count())
            {
                Stations.Clear();
                throw new Exception("Too much station data detected");
            }


            if (Stations.Any(x => x.Change != null))
            {
                DependencyService.Get<INotification>().SendNotification("River Levels", string.Join("\r\n", Stations.Select(x => x.NotificationText)));
            }


            Status = $"Data loaded at {DateTime.Now.ToString("HH:mm")}";

        }


        public static async Task<MainPageViewModel> CreateAsync()
        {
            var m=new MainPageViewModel();
            await m.GetData();
            return m;
        }

        public ICommand RefreshCommand => new MainPageRefresh(this);
    }

    internal class MainPageRefresh : ICommand
    {
        private readonly MainPageViewModel _mainPageViewModel;

        public MainPageRefresh(MainPageViewModel mainPageViewModel)
        {
            _mainPageViewModel = mainPageViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter)
        {
            _mainPageViewModel.IsBusy = true;
            await _mainPageViewModel.GetData();
            _mainPageViewModel.IsBusy = false;
        }

        public event EventHandler CanExecuteChanged;
    }
}
