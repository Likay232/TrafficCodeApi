using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.Services;
using MauiApp.Models;
using MauiApp.ViewModels;

namespace MauiApp.Views;

public partial class AuthView : ContentPage
{
    public AuthView(AuthViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
        
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

}