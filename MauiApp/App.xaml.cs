using MauiApp.ViewModels;
using MauiApp.Views;

namespace MauiApp;

public partial class App : Application
{
    public App(AuthView authView)
    {
        InitializeComponent();
        
        MainPage = authView;
    }
}