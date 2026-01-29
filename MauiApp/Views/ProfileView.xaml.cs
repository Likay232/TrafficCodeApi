using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class ProfileView
{
    public ProfileView(ProfileViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProfileViewModel vm)
            await vm.LoadProfileInfo();
    }
}