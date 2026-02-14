using System.Diagnostics;

namespace MauiApp.Infrastructure.Services;

public class ServerPingBackgroundService(ExchangeDataService service)
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
                        await service.ExchangeDataWithServer();
                        return;
                    }

                    if (await TokenService.RefreshToken())
                        await service.ExchangeDataWithServer();
                }
            }
            catch
            {
                // ignored
            }

            await Task.Delay(_interval, token);
        }
    }
}
