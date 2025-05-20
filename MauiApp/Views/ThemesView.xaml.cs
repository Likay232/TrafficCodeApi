using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.Services;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class ThemesView : ContentPage
{
    public ThemesView(ThemesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}