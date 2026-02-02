using System.Diagnostics;
using System.Windows.Input;
using MauiApp.Infrastructure.Models.Commands;
using MauiApp.Infrastructure.Models.DTO;
using MauiApp.Infrastructure.Models.Enums;
using MauiApp.Infrastructure.Services;
using MauiApp.Views;

namespace MauiApp.ViewModels;

public class TestViewModel : ViewModelBase<Test>
{
    public TestTypes TestType { get; set; }
    public int ThemeId { get; set; }
    public bool IsExam => TestType == TestTypes.Exam;

    private int _currentIndex;

    private int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            _currentIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentTask));

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
            }
        }
    }

    public TaskForTest? CurrentTask =>
        Model.Tasks.Count > CurrentIndex ? Model.Tasks[CurrentIndex] : null;

    private readonly List<UserAnswer> _answers = new();

    public ICommand NextTaskCommand { get; }
    public ICommand PreviousTaskCommand { get; }
    public ICommand SelectAnswerCommand { get; }

    public TestViewModel(ApiService service)
    {
        ApiService = service;

        NextTaskCommand = new RelayCommand(_ => NextTask(), _ => CanNext());
        PreviousTaskCommand = new RelayCommand(_ => PreviousTask(), _ => CanPrevious());
        SelectAnswerCommand = new RelayCommand(ExecuteSelectAnswer, _ => CanExecuteSelectAnswer());
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

        OnPropertyChanged(nameof(Model));
        OnPropertyChanged(nameof(CurrentTask));
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
    }
    private bool CanExecuteSelectAnswer()
    {
        if (IsExam || CurrentTask is null) return true;
        
        var task = _answers.FirstOrDefault(a => a.TaskId == CurrentTask.Id);
        
        if (task is null) return true;
        
        return task.Answer is null;
    }
}
