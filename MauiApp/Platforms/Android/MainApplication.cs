using Android.App;
using Android.Runtime;

namespace MauiApp;

using Microsoft.Maui.Hosting;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        //FirebasePushNotificationManager.Initialize(this, true);
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}