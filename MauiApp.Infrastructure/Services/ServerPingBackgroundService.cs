using System.Diagnostics;

namespace MauiApp.Infrastructure.Services;

public class ServerPingBackgroundService(LocalDataService localDataService, ApiService apiService)
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
                if (await ApiService.PingServer())
                {
                    var authToken = await SecureStorage.GetAsync("auth_token");
                    
                    if (!string.IsNullOrEmpty(authToken) && !TokenService.IsExpired(authToken) && !TokenService.ShouldRefresh(authToken))
                    {
                        await ExchangeDataWithServer();
                        return;
                    }

                    await TokenService.RefreshToken();
                    
                    await ExchangeDataWithServer();
                }
            }
            catch
            {
                // ignored
            }

            await Task.Delay(_interval, token);
        }
    }
    
    private async Task ActualizeLocalDataBase()
    {
        var lastExchange = localDataService.GetLastExchange();

        var userId = Preferences.Get("user_id", 0);

        if (userId is 0) return;

        var newData = await apiService.GetNewData(lastExchange, userId);
        
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
