using System.Diagnostics;
using MauiApp.ViewModels;
using Plugin.Firebase.CloudMessaging;

namespace MauiApp.Views;

public partial class AuthView
{
    private readonly RegisterView _rV;
    
    public AuthView(AuthViewModel vm, RegisterView rV)
    {
        InitializeComponent();

        BindingContext = vm;
        _rV = rV;
        
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        Debug.WriteLine($"Token: {token}");

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        if (Application.Current == null) return;
        if (Application.Current.MainPage == null) return;

        
        await Application.Current.MainPage.Navigation.PushModalAsync(_rV);
    }
}