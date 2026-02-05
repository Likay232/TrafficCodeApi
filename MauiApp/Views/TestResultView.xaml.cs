using System.ComponentModel;
using MauiApp.Infrastructure.Models.Enums;
using Microsoft.Maui.Controls;

namespace MauiApp.Views;

public partial class TestResultView : ContentPage, INotifyPropertyChanged
{
    private bool _passed;
    private int _mistakes;

    public bool Passed
    {
        get => _passed;
        set
        {
            if (_passed != value)
            {
                _passed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Text));
            }
        }
    }

    public int Mistakes
    {
        get => _mistakes;
        set
        {
            if (_mistakes != value)
            {
                _mistakes = value;
                OnPropertyChanged();
            }
        }
    }
    
    private TestTypes TestType { get; set; }

    public string Text
    {
        get
        {
            switch (TestType)
            {
                case TestTypes.Marathon:
                    return Passed ? "Марафон пройден" : "Марафон провален";
                case TestTypes.Exam:
                    return Passed ? "Экзамен сдан" : "Экзамен не сдан";
                default:
                    return Passed ? "Тест сдан" : "Тест не сдан";
            }        
        }
    }

public TestResultView(bool passed, int mistakes, TestTypes testType)
    {
        InitializeComponent();

        Passed = passed;
        TestType = testType;
        Mistakes = mistakes;

        BindingContext = this;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync(animated: false);
    }
}