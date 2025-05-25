using System.Text.Json;
using MauiApp.Models;
using MauiApp.Services;


var service = new ApiService();

var themes = await service.GetThemesAsync();



foreach (var theme in themes)
{
    Console.WriteLine(theme.Id);
}

