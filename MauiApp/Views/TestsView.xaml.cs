using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class TestsView : ContentPage
{
    public TestsView(TestsViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }

    private async void SelectableItemsView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count == 1)
        {
            if (e.CurrentSelection[0] is Test selectedItem)
            {
                await Shell.Current.GoToAsync($"{nameof(TestView)}?testId={selectedItem.Id})");
            }
            
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is TestsViewModel vm)
            vm.LoadTests();
    }

    private async void OnGenerateTestClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(GenerateTestView));
    }

    private async void OnTestTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is Test test)
        {
            if (await ((TestsViewModel)BindingContext).GetTestById(test.Id))
                await Shell.Current.GoToAsync($"{nameof(TestView)}");
        }
    }
}