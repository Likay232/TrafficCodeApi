using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.ViewModels;

namespace MauiApp.Pages;

public partial class AuthView : ContentPage
{
    public AuthView(AuthViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}