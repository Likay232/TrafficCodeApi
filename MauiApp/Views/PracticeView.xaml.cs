using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.Infrastructure.Models.Enums;

namespace MauiApp.Views;

public partial class PracticeView : ContentPage
{
    public PracticeView()
    {
        InitializeComponent();
    }

    private async void ExamClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(TestView)}?testType={TestTypes.Exam}", false);
    }
    
    private async void MarathonClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(TestView)}?testType={TestTypes.Marathon}", false);
    }
    
    private async void ChallengingQuestionsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(TestView)}?testType={TestTypes.ChallengingQuestions}", false);
    }

}