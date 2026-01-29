// using System.Diagnostics;
// using System.Windows.Input;
// using MauiApp.Infrastructure.Models.DTO;
// using MauiApp.Services;
//
// namespace MauiApp.Commands;
//
// public class DownloadFileCommand(ApiService apiService) : ICommand
// {
//     public event EventHandler? CanExecuteChanged;
//
//     public bool CanExecute(object? obj)
//     {
//         if (obj is not TaskForTest task)
//         {
//             return false;
//         }
//
//         return task.File != null;
//
//     }
//
//     public async void Execute(object? obj)
//     {
//         if (obj is not TaskForTest task) return;
//
//         try
//         {
//             var fileBytes = await apiService.GetFileBytes(task.File!);
//
//             if (fileBytes == null || fileBytes.Length == 0)
//             {
//                 await ShowErrorAsync("Не удалось скачать файл.");
//                 return;
//             }
//
//             var savedUri = await FileService.SaveFileToDownloadsAsync(fileBytes, task.File!);
//
//             if (savedUri == null)
//             {
//                 await ShowErrorAsync("Не удалось сохранить файл.");
//                 return;
//             }
//
//             if (Uri.TryCreate(savedUri, UriKind.Absolute, out var uri))
//             {
//                 await Launcher.Default.OpenAsync(uri);
//             }
//             else
//             {
//                 await ShowErrorAsync("Файл сохранен, но не удалось его открыть.");
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine("Ошибка при обработке файла: " + ex);
//             await ShowErrorAsync("Произошла ошибка при скачивании или открытии файла.");
//         }
//
//     }
//     
//     private async Task ShowErrorAsync(string message)
//     {
//         if (Application.Current?.MainPage != null)
//         {
//             await Application.Current.MainPage.DisplayAlert("Ошибка", message, "Ок");
//         }
//     }
//
//
//     public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
//
// }

namespace MauiApp.Infrastructure.Models.Commands;