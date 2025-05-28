using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class ChangePassView : ContentPage
{
    public ChangePassView(ChangePassViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}