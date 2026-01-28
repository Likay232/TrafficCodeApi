using MauiApp.Services;
using MauiApp.ViewModels;
using MauiApp.Views;

namespace MauiApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Navigating += OnNavigating;

        Routing.RegisterRoute(nameof(AuthView), typeof(AuthView));
        Routing.RegisterRoute(nameof(ThemesView), typeof(ThemesView));
        Routing.RegisterRoute(nameof(LessonsView), typeof(LessonsView));
        Routing.RegisterRoute(nameof(TasksView), typeof(TasksView));
        Routing.RegisterRoute(nameof(TestsView), typeof(TestsView));
        Routing.RegisterRoute(nameof(TestView), typeof(TestView));
        Routing.RegisterRoute(nameof(CheckedTestView), typeof(CheckedTestView));
        Routing.RegisterRoute(nameof(TaskView), typeof(TaskView));
        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(GenerateTestView), typeof(GenerateTestView));

        if (Preferences.ContainsKey("username"))
        {
            UserNameLabel.Text = Preferences.Get("username", "Имя пользователя");
        }
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        Preferences.Default.Clear();
        SecureStorage.RemoveAll();

        await Shell.Current.GoToAsync(nameof(AuthView));
        ;
    }

    private void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        var userName = Preferences.Get("username", null);

        var isAllowedTarget = e.Target?.Location?.OriginalString?.Contains("AuthView") == true ||
                              e.Target?.Location?.OriginalString?.Contains("RegisterView") == true || 
                              e.Current.Location.OriginalString.Contains("RegisterView")== true;

        if (isAllowedTarget || userName != null)
            return;

        e.Cancel();
    }
}