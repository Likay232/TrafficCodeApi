using System.Text.Json;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using MauiApp.Models;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace MauiApp;

[BroadcastReceiver(Label = "MyAppWidget")]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData("android.appwidget.provider", Resource = "@xml/random_task_widget_provider")]
[Service(Exported = true)]
public class RandomTaskWidget : AppWidgetProvider
{
    public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
    {
        foreach (int widgetId in appWidgetIds)
        {
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.random_task_widget);

            appWidgetManager.UpdateAppWidget(widgetId, views);
        }
    }

    public override void OnReceive(Context context, Intent intent)
    {
        base.OnReceive(context, intent);

        var action = intent.Action;

        if (action == "com.myapp.ACTION_UPDATE_WIDGET")
        {
            string serializedTask = intent.GetStringExtra("task") ?? "Нет задания";
            var task = JsonSerializer.Deserialize<TaskForTest>(serializedTask);

            if (task == null) return;
            
            RemoteViews views = new RemoteViews(context.PackageName, Resource.Layout.random_task_widget);
            views.SetTextViewText(Resource.Id.widget_text, task.Text);
            views.SetTextViewText(Resource.Id.widget_difficulty, $"Сложность: {task.DifficultyLevel}");

            if (task.Image != null && task.Image.Length > 0)
            {
                Bitmap bitmap = BitmapFactory.DecodeByteArray(task.Image, 0, task.Image.Length);
                views.SetViewVisibility(Resource.Id.widget_image, ViewStates.Visible);
                views.SetImageViewBitmap(Resource.Id.widget_image, bitmap);
            }
            else
            {
                views.SetViewVisibility(Resource.Id.widget_image, ViewStates.Gone);
            }

            Intent answerIntent = new Intent(context, typeof(RandomTaskWidget));
            answerIntent.SetAction("com.myapp.ACTION_ANSWER");
            answerIntent.PutExtra("answer", "4");
            PendingIntent answerPendingIntent = PendingIntent.GetBroadcast(context, 0, answerIntent, PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

            AppWidgetManager manager = AppWidgetManager.GetInstance(context);
            ComponentName widget = new ComponentName(context, Java.Lang.Class.FromType(typeof(RandomTaskWidget)));
            manager.UpdateAppWidget(widget, views);
        }
        else if (action == "com.myapp.ACTION_ANSWER")
        {
            string? answer = intent.GetStringExtra("answer");
            
            Android.Util.Log.Info("WidgetAnswer", $"Ответ получен: {answer}");
            
            Toast.MakeText(context, $"Ответ: {answer}", ToastLength.Short).Show();
        }
    }
}