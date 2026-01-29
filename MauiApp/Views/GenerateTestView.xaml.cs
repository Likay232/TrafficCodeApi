using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class GenerateTestView
{
    public GenerateTestView(GenerateTestViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is GenerateTestViewModel vm)
        {
            vm.LoadThemesAsync();
        }
    }
}