using Microsoft.AspNetCore.Mvc;

namespace HttpDataServer.Controllers.Game
{
    [ApiController, Route("Game/[controller]")]
    [Consumes("application/x-www-form-urlencoded"), Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PokerController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {
            return Ok("Poker Game");
        }
    }
}
