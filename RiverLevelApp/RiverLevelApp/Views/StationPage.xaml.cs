
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RiverLevelApp.ViewModel;
using RiverLevelApp.ViewModels;
using Xamarin.Forms;

namespace RiverLevelApp
{

    public partial class StationPage : ContentPage
    {
        private readonly Station _station;
        public StationPage(Station s)
        {
            _station = s;
            InitializeComponent();
        }
        
        protected override async void OnAppearing()
        {
            var model = await StationRiverLevelViewModel.CreateAsync(_station);
            BindingContext = model;
        }
    }
}