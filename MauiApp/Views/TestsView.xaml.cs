using MauiApp.Infrastructure.Models.DTO;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class TestsView
{
    public TestsView(TestsViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is TestsViewModel vm)
            vm.LoadTests();
    }
    
    private async void OnTestTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is Test test)
        {
            // if (await ((TestsViewModel)BindingContext).GetTestById(test.Id))
            //     await Shell.Current.GoToAsync($"{nameof(TestView)}");
        }
    }
}