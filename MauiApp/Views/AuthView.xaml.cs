using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class AuthView : ContentPage
{
    private readonly RegisterView _rV;
    
    public AuthView(AuthViewModel vm, RegisterView rV)
    {
        InitializeComponent();

        BindingContext = vm;
        _rV = rV;
        
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        if (Application.Current == null) return;
        if (Application.Current.MainPage == null) return;

        
        await Application.Current.MainPage.Navigation.PushModalAsync(_rV);
    }
}