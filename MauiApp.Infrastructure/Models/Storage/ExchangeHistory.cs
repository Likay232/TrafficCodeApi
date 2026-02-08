using System.ComponentModel.DataAnnotations;

namespace MauiApp.Infrastructure.Models.Storage;

public class ExchangeHistory
{
    [Key]
    public int Id { get; set; }
    public DateTime ExchangedAt { get; set; }
}