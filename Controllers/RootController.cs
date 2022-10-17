using Microsoft.AspNetCore.Mvc;

namespace DataServer.Controllers;

[ApiController]
[Route("/")]
[Consumes("application/json"), Produces("application/json")]
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