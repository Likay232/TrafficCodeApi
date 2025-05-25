using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.Models;
using MauiApp.Services;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class ThemesView : ContentPage
{
    public ThemesView(ThemesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
    private async void SelectableItemsView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count == 1)
        {
            if (e.CurrentSelection[0] is Theme selectedItem)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync($"{nameof(TasksView)}?themeId={selectedItem.Id}");
                });

            }
            
            ((CollectionView)sender).SelectedItem = null;
        }
    }
    
    private async void OnThemeTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Theme theme)
        {
            await Shell.Current.GoToAsync($"{nameof(TasksView)}?themeId={theme.Id}");
        }
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is ThemesViewModel vm)
            vm.LoadThemesAsync();
    }

    private async void OnMaterialsClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Theme theme)
        {
            await Shell.Current.GoToAsync($"{nameof(LessonsView)}?themeId={theme.Id}");
        }
    }
}