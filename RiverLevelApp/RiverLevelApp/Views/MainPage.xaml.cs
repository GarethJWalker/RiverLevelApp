
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using System.IO;
using RiverLevelApp.ViewModels;

namespace RiverLevelApp
{
    public partial class MainPage : ContentPage
	{
        public MainPage()
		{
			InitializeComponent();
        }

	    protected override async void OnAppearing()
	    {
	        var model = await MainPageViewModel.CreateAsync();
	        BindingContext = model;
            Device.StartTimer(TimeSpan.FromMinutes(1), () =>
            {
                Task.Run(async () =>
                {
                    if (DateTime.Now.Minute % 15 == 9) await model.GetData();  // Reduce hits as it tends to happen around 23 minutes after. TODO: increase this in flood situation
                });
                return true;
            });
        }
    }
}
