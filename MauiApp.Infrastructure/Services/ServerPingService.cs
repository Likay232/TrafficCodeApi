using System.Diagnostics;
using MauiApp.Infrastructure.Models.DTO;

namespace MauiApp.Infrastructure.Services;

public class ServerPingService(LocalDataService localDataService, ApiService apiService)
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);
    private readonly CancellationTokenSource _cts = new();
    
    public void Start()
    {
        _ = RunAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts.Cancel();
    }

    private async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var serverAvailable = await PingServerAsync();

                if (serverAvailable)
                {
                    await ExchangeDataWithServer();

                    var authToken = await SecureStorage.GetAsync("auth_token");
                    
                    if (!string.IsNullOrEmpty(authToken)) return;
                    
                    var username = await SecureStorage.GetAsync("username");
                    var password = await SecureStorage.GetAsync("password");

                    authToken = await apiService.Login(new AuthModel
                    {
                        Username = username,
                        Password = password
                    });
                    
                    if (authToken is not null)
                        await SecureStorage.SetAsync("token", authToken);
                }
            }
            catch
            {
                // ignored
            }

            await Task.Delay(_interval, token);
        }
    }

    private async Task<bool> PingServerAsync()
    {
        try
        {
            return await apiService.PingServer();
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    private async Task ActualizeLocalDataBase()
    {
        var lastExchange = localDataService.GetLastExchange();

        var newData = await apiService.GetNewData(lastExchange);
        
        if (newData is null) return;

        await localDataService.ProcessNewData(newData);
    }

    private async Task UploadData()
    {
        var uploadData = localDataService.GetDataToUpload();
        
        await apiService.UploadData(uploadData);
    }

    private async Task ExchangeDataWithServer()
    {
        try
        {
            await UploadData();
            await ActualizeLocalDataBase();

            await localDataService.SaveExchange();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }
}
