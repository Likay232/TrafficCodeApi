namespace WebApi.Infrastructure.Models.Requests;

public class SaveAnswer
{
    public int TaskId { get; set; }
    public bool IsCorrect { get; set; }
}