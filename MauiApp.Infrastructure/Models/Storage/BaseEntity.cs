using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiApp.Infrastructure.Models.Storage;

public class BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}