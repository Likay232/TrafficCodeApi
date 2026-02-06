using MauiApp.Infrastructure.Models.Enums;
using MauiApp.ViewModels;

namespace MauiApp.Views;

[QueryProperty(nameof(TestType), "testType")]
[QueryProperty(nameof(ThemeId), "themeId")]
public partial class TestView
{
    public string TestType
    {
        set
        {
            if (Enum.TryParse<TestTypes>(value, out var result) && FirstAppearance)
                ((TestViewModel)BindingContext).TestType = result;
        }
    }

    public int ThemeId
    {
        get => ((TestViewModel)BindingContext).ThemeId;
        set
        {
            ((TestViewModel)BindingContext).ThemeId = value;
        }
    }

    private bool FirstAppearance { get; set; } = true;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (FirstAppearance)
        {
            FirstAppearance = false;
            ((TestViewModel)BindingContext).LoadTestAsync();
        }
    }

    public TestView(TestViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}