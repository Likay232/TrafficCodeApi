using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class RegisterView
{
    public RegisterView(RegisterViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }
    
    private async void LoginButton_OnClicked(object? sender, EventArgs e)
    {
        if (Application.Current == null) return;
        if (Application.Current.MainPage == null) return;

        
        await Application.Current.MainPage.Navigation.PopModalAsync();
    }
}