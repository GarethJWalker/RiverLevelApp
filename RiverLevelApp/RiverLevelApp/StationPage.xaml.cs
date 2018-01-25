
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RiverLevelApp
{

    public partial class StationPage : ContentPage
    {
        private StationData _stationData;
        public StationPage(StationData s)
        {
            _stationData = s;
            InitializeComponent();
            
            stationName.Text = _stationData.Station.riverName + " at " + _stationData.Station.town;
            levelData.RefreshCommand = RefreshCommand;
            Task.Run(async () => await RefreshData());
        }


        private async Task RefreshData()
        {
            var l = new Label() { Text = "Loading..." };
            
            Device.BeginInvokeOnMainThread(() =>
            {

                dataContainer.Children.Add(l);
                levelData.ItemsSource = null;
                
            });

            var levels = await RiverLevel.GetAsync(_stationData.Station.stationReference);

            levels.items.ToList().ForEach(
                x => x.changecm = 100f*(x.value - (levels.items.OrderByDescending(y=>y.dateTime).FirstOrDefault(y => y.dateTime < x.dateTime)?.value ?? 0))
            );

            Device.BeginInvokeOnMainThread(() =>
                {



                    levelData.ItemsSource = levels.items.OrderByDescending(x => x.dateTime);
                    levelData.ItemTemplate = new DataTemplate(() =>
                    {
                        Label dateLabel = new Label();
                        dateLabel.SetBinding(Label.TextProperty, "dateTime", stringFormat: "{0:dd/MM/yy HH:mm}");
                        Label levelLabel = new Label();
                        levelLabel.SetBinding(Label.TextProperty, "valuecm",stringFormat: "{0:0.0}");
                        Label changeLabel = new Label();
                        changeLabel.SetBinding(Label.TextProperty, "changecm", stringFormat: "({0:+0.0;-0.0;0.0})");

                        return new ViewCell
                        {

                            View = new StackLayout
                            {
                                Padding = new Thickness(0, 5),
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                    {
                                        new BoxView(),
                                        new StackLayout
                                        {
                                            Orientation = StackOrientation.Horizontal,
                                            VerticalOptions = LayoutOptions.Start,
                                            Spacing = 10,
                                            Children =
                                                {
                                                dateLabel,
                                                levelLabel,
                                                changeLabel
                                                }
                                            }
                                    }
                            }
                        };

                        // levels.items.OrderByDescending(x => x.dateTime).ToList().ForEach(x => levelData.Add(new Label() { Text = x.dateTime.ToString("dd/MM/yy HH:mm") + " : " + x.value + "cm"}));
                    });
                    dataContainer.Children.Clear();
                    levelData.IsRefreshing = false;
                });
   
        }


        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    levelData.IsRefreshing = true;

                    await RefreshData();

                    levelData.IsRefreshing = false;
                });
            }
        }
    }
}