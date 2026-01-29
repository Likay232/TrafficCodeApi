namespace MauiApp.Infrastructure.Models.DTO;

public class WrongTask
{
    public string Text { get; set; } = string.Empty;
    public byte[]? ImageData { get; set; }
    public byte[]? FileData { get; set; }
    public string Answer { get; set; } = string.Empty;
}