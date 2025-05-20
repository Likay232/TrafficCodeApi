using System.Windows.Input;
using MauiApp.Commands;
using MauiApp.Models;
using MauiApp.Services;

namespace MauiApp.ViewModels;

[QueryProperty(nameof(TestId), "testId")]
public class TestViewModel : ViewModelBase<List<TaskForTest>>
{
    private int _testId;
    public int TestId
    {
        get => _testId;
        set
        {
            _testId = value;
            LoadTest();
        }
    }
    
    public ICommand SaveAnswerCommand { get; set; }
    public ICommand CheckTestCommand { get; set; }

    private readonly List<UserAnswer> _answers = new List<UserAnswer>();
    
    public TestViewModel(ApiService service)
    {
        _apiService = service;
        
        SaveAnswerCommand = new RelayCommand(ExecuteSaveAnswer, CanExecuteSaveAnswer);
        CheckTestCommand = new RelayCommand(ExecuteCheckTest, CanExecuteCheckTest);
    }

    private void LoadTest()
    {
        var result = _apiService.GetTestAsync(_testId).Result;
        
        Model = result ?? new List<TaskForTest>();
    }

    private void ExecuteSaveAnswer(object obj)
    {
        if (obj is not UserAnswer userAnswer || string.IsNullOrWhiteSpace(userAnswer.Answer))
            return;
        
        var existing = _answers.Any(a => a.TaskId == userAnswer.TaskId);
        if (existing)
            _answers.Remove(userAnswer);
        
        _answers.Add(userAnswer);
    }

    private bool CanExecuteSaveAnswer(object obj)
    {
        if (obj is not UserAnswer answer)
            return false;

        return !string.IsNullOrWhiteSpace(answer.Answer);
    }

    private void ExecuteCheckTest(object obj)
    {
    }

    private bool CanExecuteCheckTest(object obj)
    {
        return true;
    }
}