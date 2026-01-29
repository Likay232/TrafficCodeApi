namespace MauiApp.Infrastructure.Models.DTO;

public class ThemesStatistic
{
    public int ThemeId { get; set; }
    public string ThemeName { get; set; } = string.Empty;
    
    public double SolvedCorrectPercent { get; set; }
    public double SolvedPercent { get; set; }
    
    public int Level { get; set; }
}