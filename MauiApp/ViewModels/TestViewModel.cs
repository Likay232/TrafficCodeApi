using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

public class TestViewModel : ViewModelBase<List<TaskForTest>>
{
    public int TestId { get; set; }

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
        }
    }

    public TaskForTest? CurrentTask => Model != null && Model.Count > CurrentIndex ? Model[CurrentIndex] : null;

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
    private readonly ApiService _apiService;

    public ICommand NextTaskCommand { get; }
    public ICommand PreviousTaskCommand { get; }
    public ICommand SaveAnswerCommand { get; }
    public ICommand CheckTestCommand { get; }
    
    public TestViewModel(ApiService service)
    {
        _apiService = service;
        Model = new List<TaskForTest>();

        NextTaskCommand = new RelayCommand(_ => NextTask(), _ => CanNext());
        PreviousTaskCommand = new RelayCommand(_ => PreviousTask(), _ => CanPrevious());

        SaveAnswerCommand = new RelayCommand(ExecuteSaveAnswer, CanExecuteSaveAnswer);
        CheckTestCommand = new RelayCommand(ExecuteCheckTest, CanExecuteCheckTest);
    }

    public async void LoadTestAsync()
    {
        var result = await _apiService.GetTestAsync(TestId);
        Model = result ?? new List<TaskForTest>();
        CurrentIndex = 0;

        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(CurrentTask));
        OnPropertyChanged(nameof(CurrentAnswer));
    }

    private void NextTask()
    {
        if (CanNext())
        {
            CurrentIndex++;
        }
    }

    private bool CanNext() => CurrentIndex < Model.Count - 1;

    private void PreviousTask()
    {
        if (CanPrevious())
        {
            CurrentIndex--;
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
        var testForCheck = new TestForCheck
        {
            UserId = Preferences.Get("user_id", 0),
            Answers = _answers,
            TestId = TestId
        };

        var checkedTest = await _apiService.CheckTestAsync(testForCheck);
    }

    private bool CanExecuteCheckTest(object obj) => true;
}