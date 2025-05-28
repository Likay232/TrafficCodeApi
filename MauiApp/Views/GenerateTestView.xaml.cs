using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class GenerateTestView : ContentPage
{
    public GenerateTestView(GenerateTestViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is GenerateTestViewModel vm)
        {
            vm.LoadThemesAsync();
        }
    }
}