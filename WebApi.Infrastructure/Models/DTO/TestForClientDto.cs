namespace WebApi.Infrastructure.Models.DTO;

public class TestForClientDto
{
    public List<TaskDto> Tasks { get; set; } = new();
    public Dictionary<int, List<TaskDto>> AdditionalQuestions { get; set; } = new();
}