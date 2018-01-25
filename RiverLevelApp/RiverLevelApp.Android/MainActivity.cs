using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;
using Android.Content;
using RiverLevelApp;
using Notification.Android;
using Android;

[assembly: Xamarin.Forms.Dependency(typeof(NotificationImplementation))]
namespace Notification.Android
{
    public class NotificationImplementation : INotification
    {
        public NotificationImplementation() { }

        public void SendNotification(string title, string text)
        {

            Intent intent = new Intent(Application.Context, typeof(RiverLevelApp.Droid.MainActivity));
            const int pendingIntentId = 0;
            PendingIntent pendingIntent = PendingIntent.GetActivity(Application.Context, pendingIntentId, intent, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)     
                .SetSmallIcon(Resource.Drawable.IcDialogAlert) 
                .SetContentText(text)
                .SetStyle(new NotificationCompat.BigTextStyle());


            if (DateTime.Now.Hour>7 && DateTime.Now.Hour < 23) builder.SetDefaults((int)(NotificationDefaults.Sound | NotificationDefaults.Vibrate));
            builder.SetPriority((int)NotificationPriority.High);






            NotificationManager notificationManager =
                (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(1, builder.Build());
        }
    }
}


namespace RiverLevelApp.Droid
{



    [Activity(Label = "River Monitor", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public void SendNotification(string text)
        {


        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }


    }
}

