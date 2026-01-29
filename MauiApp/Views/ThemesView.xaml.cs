using MauiApp.Infrastructure.Models.DTO;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class ThemesView
{
    public ThemesView(ThemesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
    private void SelectableItemsView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection is { Count: 1 })
        {
            if (e.CurrentSelection[0] is Theme selectedItem)
            {
                MainThread.BeginInvokeOnMainThread(async void() =>
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
            await Shell.Current.GoToAsync(
                $"{nameof(TasksView)}?themeId={theme.Id}", animate:false
            );
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
        if (sender is Button { CommandParameter: Theme theme })
        {
            await Shell.Current.GoToAsync($"{nameof(LessonsView)}?themeId={theme.Id}", animate:false);
        }
    }

    private async void OnTasksClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: Theme theme })
        {
            await Shell.Current.GoToAsync($"{nameof(TasksView)}?themeId={theme.Id}", animate:false);
        }
    }
}