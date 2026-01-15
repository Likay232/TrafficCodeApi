using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("[controller]/[action]")]
public class ClientMvcController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}