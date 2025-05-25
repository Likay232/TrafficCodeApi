using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.ViewModels;

namespace MauiApp.Views;

[QueryProperty(nameof(TestId), "testId")]
public partial class TestView : ContentPage
{
    public int TestId
    {
        get => ((TestViewModel)BindingContext).TestId;
        set
        {
            ((TestViewModel)BindingContext).TestId = value;
            ((TestViewModel)BindingContext).LoadTestAsync();
        }
    }

    public TestView(TestViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        if (BindingContext is TestViewModel vm && sender is Entry entry)
        {
            vm.SaveAnswerCommand.Execute(entry.Text);
        }
    }
}