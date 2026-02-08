using MauiApp.Views;

namespace MauiApp;

public partial class AppShell
{
    public AppShell()
    {
        InitializeComponent();
        Navigating += OnNavigating;

        Routing.RegisterRoute(nameof(AuthView), typeof(AuthView));
        Routing.RegisterRoute(nameof(ThemesView), typeof(ThemesView));
        Routing.RegisterRoute(nameof(LessonsView), typeof(LessonsView));
        Routing.RegisterRoute(nameof(TasksView), typeof(TasksView));
        Routing.RegisterRoute(nameof(TestView), typeof(TestView));
        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(ProfileView), typeof(ProfileView));
        Routing.RegisterRoute(nameof(PracticeView), typeof(PracticeView));

        if (Preferences.ContainsKey("username"))
        {
            UserNameLabel.Text = Preferences.Get("username", "Имя пользователя");
        }
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        Preferences.Default.Clear();
        SecureStorage.RemoveAll();

        await Current.GoToAsync(nameof(AuthView));
    }

    private void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        var userName = Preferences.Get("username", null);

        var isAllowedTarget = e.Target?.Location?.OriginalString.Contains("AuthView") == true ||
                              e.Target?.Location?.OriginalString.Contains("RegisterView") == true || 
                              e.Current.Location.OriginalString.Contains("RegisterView");

        if (isAllowedTarget || userName != null)
            return;

        e.Cancel();
    }
}