using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MauiApp.Models;
using MauiApp.ViewModels;

namespace MauiApp.Views;

[QueryProperty(nameof(JsonData), "CheckedTest")]
public partial class CheckedTestView : ContentPage
{
    public CheckedTest? CheckedTest
    {
        set
        {
            ((CheckedTestViewModel)BindingContext).Model = value;
            ((CheckedTestViewModel)BindingContext).UpdateModel();
        }
    }
    
    public string JsonData
    {
        get => JsonData;
        set => CheckedTest = JsonSerializer.Deserialize<CheckedTest>(Uri.UnescapeDataString(value));
    }
    
    public CheckedTestView(CheckedTestViewModel vm)
    {
        InitializeComponent();
        
        BindingContext = vm;
    }
}