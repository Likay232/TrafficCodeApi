using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp.Infrastructure.Models.Commands;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Models.Repositories;
using MauiApp.Views;

namespace MauiApp.ViewModels;

public class TestViewModel : ViewModelBase<Test>
{
    public ObservableCollection<TaskNavigationItem> TaskNavigation { get; private set; } = new();

    private TestTypes _testType;
    public TestTypes TestType
    {
        get => _testType;
        set
        {
            _testType = value;
            OnPropertyChanged(nameof(IsExam));
        }
        
    }
    public int ThemeId { get; set; }
    public bool IsExam => TestType == TestTypes.Exam;
    private bool IsFinished => TestType == TestTypes.Finished;
    
    public bool ShouldShowHint =>
        !IsExam &&
        SelectedAnswerVariant is { IsCorrect: false };

    private int _currentIndex;

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            _currentIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentTask));
            
            UpdateNavigationState();
            OnPropertyChanged(nameof(TaskNavigation));
            
            ((RelayCommand)NextTaskCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousTaskCommand).RaiseCanExecuteChanged();
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
                OnPropertyChanged(nameof(ShouldShowHint));
            }
        }
    }

    private TimeSpan _remainingTime;

    public TimeSpan RemainingTime
    {
        get => _remainingTime;
        set => SetProperty(ref _remainingTime, value);
    }

    public string RemainingTimeDisplay => RemainingTime.ToString(@"mm\:ss");

    private System.Timers.Timer? _timer;

    public TaskForTest? CurrentTask => TaskNavigation.Count > CurrentIndex 
        ? TaskNavigation[CurrentIndex].Task 
        : null;

    private readonly List<UserAnswer> _answers = new();
    private readonly List<UserAnswer> _additionalQuestionsAnswers = new();

    public ICommand NextTaskCommand { get; }
    public ICommand PreviousTaskCommand { get; }
    public ICommand SelectAnswerCommand { get; }
    public ICommand GoToTaskCommand { get; }
    
    public TestViewModel(AppRepository repository)
    {
        AppRepository = repository;

        NextTaskCommand = new RelayCommand(_ => NextTask(), _ => CanNext());
        PreviousTaskCommand = new RelayCommand(_ => PreviousTask(), _ => CanPrevious());
        SelectAnswerCommand = new RelayCommand(ExecuteSelectAnswer, _ => CanExecuteSelectAnswer());
        GoToTaskCommand = new RelayCommand(GoToTask, _ => CanGoToTask());
    }


    public async void LoadTestAsync()
    {
        IsLoading = true;
        
        var userId = Preferences.Default.Get("user_id", 0);
        var result = await AppRepository.GenerateTest(TestType, userId, ThemeId);

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
        
        ((RelayCommand)NextTaskCommand).RaiseCanExecuteChanged();
        ((RelayCommand)PreviousTaskCommand).RaiseCanExecuteChanged();

        OnPropertyChanged(nameof(TaskNavigation));

        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(CurrentTask));
        
        if (IsExam)
            StartExamTimer(20);
        
        IsLoading = false;
    }

    private void UpdateNavigationState()
    {
        foreach (var item in TaskNavigation)
        {
            item.IsCurrent = item.Index == CurrentIndex;

            var answer = _answers.FirstOrDefault(a => a.TaskId == item.Task.Id)
                ?? _additionalQuestionsAnswers.FirstOrDefault(a => a.TaskId == item.Task.Id);

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

    private bool CanNext() => CurrentIndex < TaskNavigation.Count - 1;

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

        var answer = _answers.FirstOrDefault(a => a.TaskId == CurrentTask.Id) 
            ?? _additionalQuestionsAnswers.First(a => a.TaskId == CurrentTask.Id);
        
        answer.Answer = SelectedAnswerVariant.Text;
        answer.IsCorrect = SelectedAnswerVariant.IsCorrect;
    }

    private void RestoreSelectedAnswer()
    {
        if (CurrentTask == null) return;

        var saved = _answers.Concat(_additionalQuestionsAnswers)
            .FirstOrDefault(a => a.TaskId == CurrentTask.Id);
        if (saved == null) return;

        SelectedAnswerVariant =
            CurrentTask.AnswerVariantsWithFlag
                .FirstOrDefault(v => v.Text == saved.Answer);
    }

    private async void ExecuteSelectAnswer(object obj)
    {
        if (obj is not AnswerVariant answer)
            return;

        var userId = Preferences.Get("user_id", 0);
        var taskId = CurrentTask!.Id;
        var isCorrect = answer.IsCorrect;

        SelectedAnswerVariant = answer;

        if (!IsExam) await AppRepository.SaveAnswer(userId, taskId, isCorrect);

        UpdateNavigationState();

        if (_answers.Concat(_additionalQuestionsAnswers).All(a => a.Answer != null) ||
            (!isCorrect && TestType == TestTypes.Marathon))
        {
            ShowResult();
        }
    }

    private void AddAdditionalQuestions(List<int> wrongThemes)
    {
        if (wrongThemes.Count == 0)
            return;

        foreach (var themeId in wrongThemes)
        {
            if (!Model.AdditionalQuestions.TryGetValue(themeId, out var tasks))
                continue;

            foreach (var task in tasks)
            {
                task.BuildVariants();

                Model.Tasks.Add(task);

                _additionalQuestionsAnswers.Add(new UserAnswer
                {
                    TaskId = task.Id,
                    Answer = null
                });

                TaskNavigation.Add(new TaskNavigationItem
                {
                    Index = TaskNavigation.Count,
                    Task = task
                });
            }
        }

        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(TaskNavigation));
        
        ((RelayCommand)NextTaskCommand).RaiseCanExecuteChanged();
    }

    private async void ShowResult()
    {
        int mistakesCount = 0;
        var wrongThemes = new HashSet<int>();
        bool additionalFailed = false;

        foreach (var a in _answers)
        {
            var task = Model.Tasks.FirstOrDefault(t => t.Id == a.TaskId);
            var selectedVariant =
                task?.AnswerVariantsWithFlag.FirstOrDefault(v => v.Text == a.Answer);

            if (selectedVariant is { IsCorrect: false })
            {
                mistakesCount++;
                wrongThemes.Add(task!.ThemeId);
            }
        }

        if (IsExam)
        {
            if (mistakesCount is > 0 and < 3 && 
                _additionalQuestionsAnswers.Count == 0 && 
                wrongThemes.Count == mistakesCount)
            {
                
                AddAdditionalQuestions(wrongThemes.ToList());
                return;
            }

            additionalFailed = _additionalQuestionsAnswers.Any(a =>
            {
                var task = Model.Tasks.FirstOrDefault(t => t.Id == a.TaskId);
                var selected =
                    task?.AnswerVariantsWithFlag.FirstOrDefault(v => v.Text == a.Answer);

                return selected is { IsCorrect: false };
            });

            var userId = Preferences.Default.Get("user_id", 0);
            
            await AppRepository.SaveAnswers(userId, _answers.Concat(_additionalQuestionsAnswers).ToList());
        }

        var passed = mistakesCount <= 2 &&
                     !additionalFailed &&
                     _answers.Concat(_additionalQuestionsAnswers).All(a => a.Answer != null);

        if (TestType == TestTypes.Marathon && mistakesCount > 0 || wrongThemes.Count != mistakesCount)
        {
            passed = false;
        }

        var resultPage = new TestResultView(passed, mistakesCount, TestType);
        
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Dispose();
        }

        TestType = TestTypes.Finished;
        UpdateNavigationState();

        await Shell.Current.Navigation.PushModalAsync(resultPage, false);
    }

    private bool CanExecuteSelectAnswer()
    {
        if (CurrentTask is null || IsFinished) return false;

        if (IsExam) return true;
        
        var task = _answers.FirstOrDefault(a => a.TaskId == CurrentTask.Id)
            ?? _additionalQuestionsAnswers.FirstOrDefault(a => a.TaskId == CurrentTask.Id);

        if (task is null) return true;

        return task.Answer is null;
    }

    private void StartExamTimer(int minutes)
    {
        if (!IsExam || IsFinished)
            return;

        RemainingTime = TimeSpan.FromMinutes(minutes);
        OnPropertyChanged(nameof(RemainingTimeDisplay));

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (_, _) =>
        {
            RemainingTime = RemainingTime.Add(TimeSpan.FromSeconds(-1));
            OnPropertyChanged(nameof(RemainingTimeDisplay));

            if (RemainingTime <= TimeSpan.Zero)
            {
                _timer.Stop();
                _timer.Dispose();
                ShowResult();
            }
        };
        _timer.Start();
    }
}