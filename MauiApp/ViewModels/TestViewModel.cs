using System.Diagnostics;
using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Models;
using MauiApp.Services;
using MauiApp.Views;

namespace MauiApp.ViewModels;

public class TestViewModel : ViewModelBase<List<TaskForTest>>
{
    private int _currentIndex;
    private int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            _currentIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentTask));
            OnPropertyChanged(nameof(CurrentAnswer));

            ((RelayCommand)NextTaskCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousTaskCommand).RaiseCanExecuteChanged();
        }
    }

    public TaskForTest? CurrentTask => Model.Count > CurrentIndex ? Model[CurrentIndex] : null;

    public UserAnswer CurrentAnswer
    {
        get
        {
            var taskId = CurrentTask?.Id ?? 0;
            if (taskId == 0)
                return new UserAnswer();

            var answer = _answers.FirstOrDefault(a => a.TaskId == taskId);
            if (answer == null)
            {
                answer = new UserAnswer { TaskId = taskId, Answer = "" };
                _answers.Add(answer);
            }

            return answer;
        }
    }


    private readonly List<UserAnswer> _answers = new();
    private new readonly ApiService _apiService;
    private readonly SharedObjectStorageService _result;

    public ICommand NextTaskCommand { get; }
    public ICommand PreviousTaskCommand { get; }
    public ICommand SaveAnswerCommand { get; }
    public ICommand CheckTestCommand { get; }

    public TestViewModel(ApiService service, SharedObjectStorageService result)
    {
        _apiService = service;
        _result = result;
        Model = new List<TaskForTest>();

        NextTaskCommand = new RelayCommand(_ => NextTask(), _ => CanNext());
        PreviousTaskCommand = new RelayCommand(_ => PreviousTask(), _ => CanPrevious());

        SaveAnswerCommand = new RelayCommand(ExecuteSaveAnswer, CanExecuteSaveAnswer);
        CheckTestCommand = new RelayCommand(ExecuteCheckTest, CanExecuteCheckTest);
    }

    public async void LoadTestAsync()
    {
        try
        {
            Model = _result.CurrentTest ?? new List<TaskForTest>();
            CurrentIndex = 0;

            foreach (var task in Model)
            {
                if (_answers.Any(a => a.TaskId == task.Id))
                    continue;
            
                _answers.Add(new UserAnswer
                {
                    Answer = "",
                    TaskId = task.Id
                });
            }

            OnPropertyChanged(nameof(Model));
            OnPropertyChanged(nameof(CurrentTask));
            OnPropertyChanged(nameof(CurrentAnswer));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    private void NextTask()
    {
        if (CanNext())
        {
            CurrentIndex++;
            
            OnPropertyChanged(nameof(CurrentTask));
        }
    }

    private bool CanNext() => CurrentIndex < Model.Count - 1;

    private void PreviousTask()
    {
        if (CanPrevious())
        {
            CurrentIndex--;
            
            OnPropertyChanged(nameof(CurrentTask));
        }
    }

    private bool CanPrevious() => CurrentIndex > 0;

    private void ExecuteSaveAnswer(object obj)
    {
        if (obj is not string answerText || CurrentTask == null)
            return;

        var answer = _answers.FirstOrDefault(a => a.TaskId == CurrentTask.Id);
        if (answer != null)
        {
            answer.Answer = answerText;
        }
        else
        {
            _answers.Add(new UserAnswer { TaskId = CurrentTask.Id, Answer = answerText });
        }

        OnPropertyChanged(nameof(CurrentAnswer));
    }

    private bool CanExecuteSaveAnswer(object obj)
    {
        return obj is string s && !string.IsNullOrWhiteSpace(s);
    }

    private async void ExecuteCheckTest(object obj)
    {
        try
        {
            var testForCheck = new TestForCheck
            {
                UserId = Preferences.Get("user_id", 0),
                Answers = _answers,
            };

            var checkedTest = await _apiService.CheckTestAsync(testForCheck);
            _result.CurrentResult = checkedTest;
        
            if (checkedTest != null)
            {
                await Shell.Current.GoToAsync(nameof(CheckedTestView));
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    private bool CanExecuteCheckTest(object obj) => true;
}