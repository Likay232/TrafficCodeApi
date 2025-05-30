#if ANDROID
using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Webkit;
using Uri = Android.Net.Uri;
#endif

namespace MauiApp.Services;

public static class FileService
{
    public static async Task<string?> SaveFileToDownloadsAsync(byte[] data, string fileName)
    {
#if ANDROID
        try
        {
            var context = Android.App.Application.Context;

            if (context.ContentResolver == null)
                return null;

            var mimeType = GetMimeType(fileName) ?? "application/octet-stream";

            var existingUri = FindFileInDownloadsAsync(context, fileName);
            if (existingUri != null)
            {
                return existingUri.ToString();
            }

            var contentValues = new ContentValues();
            contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            contentValues.Put(MediaStore.IMediaColumns.MimeType, mimeType);
            contentValues.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);

            var uri = context.ContentResolver.Insert(MediaStore.Downloads.ExternalContentUri, contentValues);

            if (uri != null)
            {
                await using var outputStream = context.ContentResolver.OpenOutputStream(uri);
                if (outputStream == null) 
                    throw new Exception("Не удалось открыть выходящий поток.");
                
                await outputStream.WriteAsync(data);
                await outputStream.FlushAsync();

                return uri.ToString();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Ошибка при сохранении файла: " + ex);
        }
#endif
        return null;
    }

#if ANDROID
    private static Uri? FindFileInDownloadsAsync(Context context, string fileName)
    {
        var projection = new string[] { MediaStore.MediaColumns.Id, MediaStore.MediaColumns.DisplayName };
        string selection = MediaStore.MediaColumns.DisplayName + "=?";
        string[] selectionArgs = [fileName];

        using ICursor cursor = context.ContentResolver.Query(
            MediaStore.Downloads.ExternalContentUri,
            projection,
            selection,
            selectionArgs,
            null);
        if (cursor != null && cursor.MoveToFirst())
        {
            int idColumn = cursor.GetColumnIndexOrThrow(MediaStore.MediaColumns.Id);
            long id = cursor.GetLong(idColumn);
            Uri contentUri = ContentUris.WithAppendedId(MediaStore.Downloads.ExternalContentUri, id);
            return contentUri;
        }

        return null;
    }

    private static string? GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.TrimStart('.')?.ToLowerInvariant();
        return MimeTypeMap.Singleton?.GetMimeTypeFromExtension(extension);
    }
#endif
}