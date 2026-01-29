using MauiApp.ViewModels;

namespace MauiApp.Views;

[QueryProperty(nameof(ThemeId), "themeId")]
public partial class LessonsView
{ 
    public int ThemeId
    {
        get => ((LessonsViewModel)BindingContext).ThemeId;
        set
        {
            ((LessonsViewModel)BindingContext).ThemeId = value;
            ((LessonsViewModel)BindingContext).LoadLessonsAsync();
        }
    }

    
    public LessonsView(LessonsViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }

    private async void OnOpenLinkClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string url && !string.IsNullOrEmpty(url))
        {
            try
            {
                await Launcher.OpenAsync(url);
            }
            catch (Exception exception)
            {
                await DisplayAlert("Ошибка", $"Не удалось открыть ссылку: {exception.Message}", "OK");
            }
        }
}
}