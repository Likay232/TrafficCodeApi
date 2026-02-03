using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp.Infrastructure.Models.Commands;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Services;
using MauiApp.Views;

namespace MauiApp.ViewModels;

public class TestViewModel : ViewModelBase<Test>
{
    public ObservableCollection<TaskNavigationItem> TaskNavigation { get; private set; } = new();
    public TestTypes TestType { get; set; }
    public int ThemeId { get; set; }
    public bool IsExam => TestType == TestTypes.Exam;
    private bool IsFinished { get; set; }

    private int _currentIndex;

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            _currentIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentTask));

            ((RelayCommand)NextTaskCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousTaskCommand).RaiseCanExecuteChanged();
            
            UpdateNavigationState();
            OnPropertyChanged(nameof(TaskNavigation));
            ScrollToCurrentRequested?.Invoke();
        }
    }

    private AnswerVariant? _selectedAnswerVariant;

    public AnswerVariant? SelectedAnswerVariant
    {
        get => _selectedAnswerVariant;
        set
        {
            if (SetProperty(ref _selectedAnswerVariant, value) && CurrentTask != null)
            {
                foreach (var v in CurrentTask.AnswerVariantsWithFlag)
                    v.IsSelected = v == value;

                SaveSelectedAnswer();
            }
        }
    }

    public TaskForTest? CurrentTask =>
        Model.Tasks.Count > CurrentIndex ? Model.Tasks[CurrentIndex] : null;

    private readonly List<UserAnswer> _answers = new();

    public ICommand NextTaskCommand { get; }
    public ICommand PreviousTaskCommand { get; }
    public ICommand SelectAnswerCommand { get; }
    public ICommand GoToTaskCommand { get; }
    
    public Action? ScrollToCurrentRequested;
    
    public TestViewModel(ApiService service)
    {
        ApiService = service;

        NextTaskCommand = new RelayCommand(_ => NextTask(), _ => CanNext());
        PreviousTaskCommand = new RelayCommand(_ => PreviousTask(), _ => CanPrevious());
        SelectAnswerCommand = new RelayCommand(ExecuteSelectAnswer, _ => CanExecuteSelectAnswer());
        GoToTaskCommand = new RelayCommand(GoToTask, _ => CanGoToTask());
    }
    
    

    public async void LoadTestAsync()
    {
        var userId = Preferences.Default.Get("user_id", 0);
        var result = await ApiService.GenerateTest(TestType, userId,ThemeId);
        
        Model = result ?? new Test();

        foreach (var task in Model.Tasks)
        {
            task.BuildVariants();

            if (_answers.Any(a => a.TaskId == task.Id))
                continue;

            _answers.Add(new UserAnswer
            {
                TaskId = task.Id,
                Answer = null
            });
        }

        CurrentIndex = 0;
        
        TaskNavigation.Clear();

        for (int i = 0; i < Model.Tasks.Count; i++)
        {
            TaskNavigation.Add(new TaskNavigationItem()
            {
                Index = i,
                Task = Model.Tasks[i]
            });
        }

        UpdateNavigationState();

        OnPropertyChanged(nameof(TaskNavigation));

        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(CurrentTask));
    }
    
    private void UpdateNavigationState()
    {
        foreach (var item in TaskNavigation)
        {
            item.IsCurrent = item.Index == CurrentIndex;

            var answer = _answers.FirstOrDefault(a => a.TaskId == item.Task.Id);

            if (answer?.Answer == null)
            {
                item.IsAnswered = false;
                item.IsCorrect = null;
                continue;
            }

            item.IsAnswered = true;

            if (!IsExam)
            {
                var selected = item.Task.AnswerVariantsWithFlag
                    .FirstOrDefault(v => v.Text == answer.Answer);

                item.IsCorrect = selected?.IsCorrect;
            }
        }
    }
    
    private void GoToTask(object obj)
    {
        if (obj is not TaskNavigationItem item)
            return;

        CurrentIndex = item.Index;
        RestoreSelectedAnswer();
    }
    
    private bool CanGoToTask()
    {
        return true;
    }

    private void NextTask()
    {
        if (!CanNext()) return;

        CurrentIndex++;
        RestoreSelectedAnswer();
    }

    private bool CanNext() => CurrentIndex < Model.Tasks.Count - 1;

    private void PreviousTask()
    {
        if (!CanPrevious()) return;

        CurrentIndex--;
        RestoreSelectedAnswer();
    }

    private bool CanPrevious() => CurrentIndex > 0;

    private void SaveSelectedAnswer()
    {
        if (CurrentTask == null || SelectedAnswerVariant == null)
            return;

        var answer = _answers.First(a => a.TaskId == CurrentTask.Id);
        answer.Answer = SelectedAnswerVariant.Text;
    }

    private void RestoreSelectedAnswer()
    {
        if (CurrentTask == null)
            return;

        var saved = _answers.FirstOrDefault(a => a.TaskId == CurrentTask.Id);
        if (saved == null)
            return;
    
        SelectedAnswerVariant =
            CurrentTask.AnswerVariantsWithFlag
                .FirstOrDefault(v => v.Text == saved.Answer);
    }
    
    private async void ExecuteSelectAnswer(object obj)
    {
        if (obj is not AnswerVariant answer)
            return;
        
        var userId = Preferences.Get("user_id", 0);
        var taskId =  CurrentTask!.Id;
        var isCorrect = answer.IsCorrect;
        
        SelectedAnswerVariant = answer;

        if (!IsExam) await ApiService.SaveAnswer(userId, taskId, isCorrect);

        UpdateNavigationState();
        
        if (_answers.All(a => a.Answer is not null))
        {
            IsFinished = true;
            
            int mistakesCount = 0;
            foreach (var a in _answers)
            {
                var task = Model.Tasks.FirstOrDefault(t => t.Id == a.TaskId);

                var selectedVariant = task?.AnswerVariantsWithFlag.FirstOrDefault(v => v.Text == a.Answer);
                if (selectedVariant is { IsCorrect: false })
                    mistakesCount++;
            }

            var passed = mistakesCount <= 2;

            var resultPage = new TestResultView(passed, mistakesCount);

            await Shell.Current.Navigation.PushModalAsync(resultPage, animated:false);
        }
            
    }
    private bool CanExecuteSelectAnswer()
    {
        if (IsExam) return true;
        if (CurrentTask is null || IsFinished) return  false;
        
        var task = _answers.FirstOrDefault(a => a.TaskId == CurrentTask.Id);
        
        if (task is null) return true;
        
        return task.Answer is null;
    }
}
