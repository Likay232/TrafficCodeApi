using System.Diagnostics;

namespace MauiApp.Infrastructure.Services;

public class ExchangeDataService(LocalDataService localDataService, ApiService apiService)
{
    private async Task ActualizeLocalDataBase(int userId)
    {
        var newData = await apiService.GetNewData(userId);
        
        if (newData is null) return;

        await localDataService.ProcessNewData(newData);
    }

    private async Task UploadData()
    {
        var uploadData = localDataService.GetDataToUpload();
        
        if (!uploadData.HasData()) return;
        
        await apiService.UploadData(uploadData);
    }

    public async Task ExchangeDataWithServer()
    {
        try
        {
            var userId = Preferences.Get("user_id", 0);

            if (userId is 0) return;

            await UploadData();
            await ActualizeLocalDataBase(userId);

            await localDataService.SaveExchange();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
        }
    }
}