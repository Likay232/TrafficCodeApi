using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class CheckedTestView
{
    public CheckedTestView(CheckedTestViewModel vm)
    {
        InitializeComponent();
        
        BindingContext = vm;
    }
}