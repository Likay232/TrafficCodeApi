using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class CheckedTestView : ContentPage
{
    public CheckedTestView(CheckedTestViewModel vm)
    {
        InitializeComponent();
        
        BindingContext = vm;
    }
}