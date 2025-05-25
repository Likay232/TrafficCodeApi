using MauiApp.Views;

namespace MauiApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(AuthView), typeof(AuthView));
        Routing.RegisterRoute(nameof(ThemesView), typeof(ThemesView));
        Routing.RegisterRoute(nameof(LessonsView), typeof(LessonsView));
        Routing.RegisterRoute(nameof(TasksView), typeof(TasksView));
        Routing.RegisterRoute(nameof(TestsView), typeof(TestsView));
        Routing.RegisterRoute(nameof(TestView), typeof(TestView));
    }
}