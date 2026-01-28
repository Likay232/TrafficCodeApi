using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TaskViewModel : ViewModelBase<TaskForTest>
{
    public int TaskId { get; set; }

    private bool? _isCorrect;

    public bool? IsCorrect
    {
        get => _isCorrect;
        set
        {
            _isCorrect = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ResultText));
            OnPropertyChanged(nameof(ResultColor));
            OnPropertyChanged(nameof(IsResultVisible));
        }
    }

    public string ResultText => IsCorrect == null ? string.Empty : (IsCorrect.Value ? "Верно" : "Неверно");

    public Color ResultColor =>
        IsCorrect == null ? Colors.Transparent :
        IsCorrect.Value ? Colors.Green : Colors.Red;

    public bool IsResultVisible => IsCorrect != null;

    public ICommand CheckTaskCommand { get; set; }
    
    public ICommand DownloadFileCommand { get; set; }

    public string Answer { get; set; }

    public TaskViewModel(ApiService service)
    {
        _apiService = service;

        Answer = "";
        CheckTaskCommand = new RelayCommand(ExecuteCheckTask, CanExecuteCheckTask);
        DownloadFileCommand = new DownloadFileCommand(service);
    }

    public async void LoadTask()
    {
        var result = await _apiService.GetTaskById(TaskId);

        Model = result ?? new TaskForTest();

        OnPropertyChanged(nameof(Model));
    }

    private bool CanExecuteCheckTask(object obj) => true;

    private async void ExecuteCheckTask(object obj)
    {
        var answer = new CheckTask
        {
            UserId = Preferences.Get("user_id", 0),
            TaskId = TaskId,
            Answer = Answer
        };

        IsCorrect = await _apiService.CheckTask(answer);
    }
}