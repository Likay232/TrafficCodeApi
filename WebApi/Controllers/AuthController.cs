using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.Models.Requests;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController(AuthService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<bool>> Register(Register request)
    {
        try
        {
            return StatusCode(200, await service.Register(request));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<string?>> Login(Login request)
    {
        try
        {
            return StatusCode(200, await service.Login(request));
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

}