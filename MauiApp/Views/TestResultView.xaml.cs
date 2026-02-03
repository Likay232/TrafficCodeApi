using System.ComponentModel;
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
                OnPropertyChanged(nameof(Passed));
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
                OnPropertyChanged(nameof(Mistakes));
            }
        }
    }

    public string Text => Passed ? "Экзамен сдан" : "Экзамен не сдан";

    public TestResultView(bool passed, int mistakes)
    {
        InitializeComponent();

        Passed = passed;
        Mistakes = mistakes;

        BindingContext = this;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync(animated: false);
    }
}