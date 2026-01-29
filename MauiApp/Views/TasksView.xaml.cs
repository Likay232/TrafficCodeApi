using MauiApp.ViewModels;

namespace MauiApp.Views;

[QueryProperty(nameof(ThemeId), "themeId")]
public partial class TasksView
{
    public int ThemeId
    {
        get => ((TasksViewModel)BindingContext).ThemeId;
        set
        {
            ((TasksViewModel)BindingContext).ThemeId = value;
        }
    }

    public TasksView(TasksViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        Loaded += async (_, _) =>
        {
            await Dispatcher.DispatchAsync(async () =>
            {
                await Task.Yield();
                if (BindingContext is TasksViewModel vm)
                    await vm.LoadTasksAsync();
            });
        };
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // // Этот трюк позволяет сначала отрисовать UI, а потом начать асинхронную загрузку
        // Dispatcher.Dispatch(async () =>
        // {
        //     await Task.Delay(1000);
        //     await Task.Yield(); // даём UI возможность отрисоваться
        //     if (BindingContext is TasksViewModel vm)
        //     {
        //         await vm.LoadTasksAsync(); // теперь начинается загрузка
        //     }
        // });
    }
}
