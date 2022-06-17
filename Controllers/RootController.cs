using Microsoft.AspNetCore.Mvc;

namespace HttpDataServer.Controllers;

[ApiController]
[Route("/")]
public class RootController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Redirect(Url.Content("~/swagger"));
    }

    [HttpGet, Route("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}