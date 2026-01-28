using System.ComponentModel.DataAnnotations;

namespace MauiApp.Infrastructure.Models.Storage;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}