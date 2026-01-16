using WebApi.Infrastructure.Models.Enums;

namespace WebApi.Infrastructure.Models.Requests;

public class GenerateTest
{
    public TestTypes TestType { get; set; }
    public int ThemeId { get; set; }
    public int UserId { get; set; }
}