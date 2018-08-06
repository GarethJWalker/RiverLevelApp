

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace RiverLevelApp
{
    public class Station
    {
        public StationDetailRaw Detail { get; set; }
        public float? Change { get; set; }
        public string NotificationText => $@"River level at {StationDetails.town} as of {LatestLevel.dateTimeTimeZone:HH:mm}: {LatestLevel.value*100}cm" + (Change != null ? $" ({(Change*100f)?.ToString("{0:+0.0;-0.0;0.0}")})" : "");
        public string ButtonText =>  $@"{StationDetails.town} @ {LatestLevel.dateTimeTimeZone:HH:mm}  :  {LatestLevel.value * 100}cm" + (Change != null ? $" ({(Change * 100f)?.ToString("{0:+0.0;-0.0;0.0}")})" : "");
        public Level LatestLevel => StationDetails.measures.latestReading;
        public StationDetailRaw.Items StationDetails => Detail.items;
        public List<Level> Levels { get; set; }
        public Color ButtonColor => this.LatestLevel.value > this.StationDetails.stageScale.typicalRangeHigh ? Color.Salmon : Color.Gray;
        public ICommand Clicked => new StationClickedCommand(this);
        public string StationName => StationDetails.riverName + " at " + StationDetails.town;
    }

    public class StationClickedCommand : ICommand
    {
        private readonly Station _station;
        public StationClickedCommand(Station station)
        {
            this._station = station;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var f = new StationPage(_station);
            Application.Current.MainPage.Navigation.PushModalAsync(f);
        }

        public event EventHandler CanExecuteChanged;
    }
}