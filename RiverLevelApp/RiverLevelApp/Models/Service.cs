

namespace RiverLevelApp
{


    //public class RiverLevelBinder : Binder
    //{
    //    public RiverLevelBinder(RiverLevelBinderService service)
    //    {
    //        this.Service = service;
    //    }

    //    public RiverLevelBinderService Service { get; private set; }
    //}

    //[Service(Name = "com.xamarin.ReiverAppService")]
    //public class RiverLevelBinderService : Service
    //{
    //    static readonly string TAG = typeof(RiverLevelBinderService).FullName;


    //    public IBinder Binder { get; private set; }

    //    public override void OnCreate()
    //    {
    //        // This method is optional to implement
    //        base.OnCreate();
    //        Log.Debug(TAG, "OnCreate");

    //    }

    //    public override IBinder OnBind(Intent intent)
    //    {
    //        // This method must always be implemented
    //        Log.Debug(TAG, "OnBind");
    //        return this.Binder;
    //    }

    //    public override bool OnUnbind(Intent intent)
    //    {
    //        // This method is optional to implement
    //        Log.Debug(TAG, "OnUnbind");
    //        return base.OnUnbind(intent);
    //    }

    //    public override void OnDestroy()
    //    {
    //        // This method is optional to implement
    //        Log.Debug(TAG, "OnDestroy");
    //        Binder = null;
    //        base.OnDestroy();
    //    }

    //    public string GetData()
    //    {
    //        return "Data";
    //    }
    //}

    //public class RiverLevelServiceConnection : Java.Lang.Object, IServiceConnection
    //{
    //    static readonly string TAG = typeof(RiverLevelServiceConnection).FullName;

    //    MainActivity mainActivity;
    //    public RiverLevelServiceConnection(MainActivity activity)
    //    {
    //        IsConnected = false;
    //        Binder = null;
    //        mainActivity = activity;
    //    }

    //    public bool IsConnected { get; private set; }
    //    public RiverLevelBinder Binder { get; private set; }

    //    public void OnServiceConnected(ComponentName name, IBinder service)
    //    {
    //        Binder = service as RiverLevelBinder;
    //        IsConnected = this.Binder != null;

    //        string message = "onServiceConnected - ";
    //        Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

    //        if (IsConnected)
    //        {
    //            message = message + " bound to service " + name.ClassName;
    //            mainActivity.UpdateUiForBoundService();
    //        }
    //        else
    //        {
    //            message = message + " not bound to service " + name.ClassName;
    //            mainActivity.UpdateUiForUnboundService();
    //        }

    //        Log.Info(TAG, message);


    //    }

    //    public void OnServiceDisconnected(ComponentName name)
    //    {
    //        Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
    //        IsConnected = false;
    //        Binder = null;
    //        mainActivity.UpdateUiForUnboundService();
    //    }

    //    public string GetData()
    //    {
    //        if (!IsConnected)
    //        {
    //            return null;
    //        }

    //        return Binder?.GetData();
    //    }
    //}
}