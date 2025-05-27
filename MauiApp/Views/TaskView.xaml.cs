using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp.ViewModels;

namespace MauiApp.Views;

[QueryProperty(nameof(TaskId), "taskId")]
public partial class TaskView : ContentPage
{
    public int TaskId
    {
        get => ((TaskViewModel)BindingContext).TaskId;
        set
        {
            ((TaskViewModel)BindingContext).TaskId = value;
            ((TaskViewModel)BindingContext).LoadTask();
        }

    }
    
    public TaskView(TaskViewModel vm)
    {
        InitializeComponent();
        
        BindingContext = vm;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TaskViewModel vm)
            vm.LoadTask();
    }

}