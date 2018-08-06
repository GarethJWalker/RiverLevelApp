using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RiverLevelApp.ViewModel
{
    public class StationRiverLevelViewModel : INotifyPropertyChanged
    {
        public Station Station { get; }
        private StationRiverLevelViewModel(Station s)
        {
            Station = s;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isBusy;

        public bool IsBusy {
            get => _isBusy;
            set
            {
                if (_isBusy != value) OnPropertyChanged("IsBusy");
                _isBusy = value;
            }
        }

        public ICommand RefreshCommand => new StationRiverLevelRefresh(this);
        public static async Task<StationRiverLevelViewModel> CreateAsync(Station s)
        {
            var stationRiverLevelViewModel = new StationRiverLevelViewModel(s);
            await stationRiverLevelViewModel.GetData();
            return stationRiverLevelViewModel;
        }
        
        public async Task GetData()
        {
            Station.Levels = new List<Level>();
            OnPropertyChanged("Station.Levels");
            var t = await RiverLevel.GetAsync(Station.StationDetails.stationReference);
            await Task.Run(() =>
            {
                Station.Levels = t.items.ToList().OrderByDescending(x => x.dateTime).ToList();
                Station.Levels.ToList().AsParallel().ForAll(x =>
                    x.changecm = 100f * (x.value - (Station.Levels.OrderByDescending(y => y.dateTimeTimeZone)
                                                        .FirstOrDefault(y => y.dateTimeTimeZone < x.dateTimeTimeZone)
                                                        ?.value ?? 0)));
            });
        }


    }

    internal class StationRiverLevelRefresh : ICommand
    {
        private readonly StationRiverLevelViewModel _stationRiverLevelViewModel;

        public StationRiverLevelRefresh(StationRiverLevelViewModel stationRiverLevelViewModel)
        {
            _stationRiverLevelViewModel = stationRiverLevelViewModel;
        }

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter)
        {
            _stationRiverLevelViewModel.IsBusy = true;
            await _stationRiverLevelViewModel.GetData();
            _stationRiverLevelViewModel.IsBusy = false;
        }

        public event EventHandler CanExecuteChanged;
    }
}
