using HttpDataServer.Core;
using HttpDataServer.Dtos.RespDto;
using HttpDataServer.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using HttpDataServer.Dtos.ValidationCode;
using StackExchange.Redis;

namespace HttpDataServer.Controllers;

[ApiController, Route("[controller]")]
[Consumes("application/x-www-form-urlencoded"), Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class ValidationCodeController : ControllerBase
{
    private const string keyPrefix = "ValidationCode";

    private readonly HashSet<string> handleTypes = new() { "phone", "email", "password" };
    private readonly ValidationCodeRepo cache;

    private RespDto resp = new RespDto();

    public ValidationCodeController(ValidationCodeRepo repo)
    {
        this.cache = repo;
    }

    [HttpGet("{uid}")]
    public IActionResult Get(long uid, [FromQuery] string type)
    {
        string handleType = type?.ToLower();
        if (!handleTypes.Contains(handleType))
        {
            resp.Code = Code.ParametereError;
            return Ok(resp);
        }

        string key = $"{keyPrefix}:{handleType}:{uid}";
        if (!cache.CheckExists(key))
        {
            resp.Code = Code.VerificationCodeNotFound;
            return Ok(resp);
        }

        if (cache.Get(key, out HashEntry data))
        {
            resp.Data = new ValidationCodeRespDto
            {
                Code = data.Name,
                ExtraValue = data.Value,
            };
        }
        else
        {
            resp.Code = cache.RespCode;
        }

        return Ok(resp);
    }

    [HttpPost]
    public IActionResult Post([FromForm] ValidationCodePostDto data)
    {
        string typeStr = data.Type.ToLower();
        if (handleTypes.Contains(typeStr))
        {
            string key = $"{keyPrefix}:{typeStr}:{data.UID}";
            cache.Set(key, data.Code, data.ExtraData, data.ExpireSecond);
            resp.Code = cache.RespCode;
        }
        else
        {
            resp.Code = Code.ParametereError;
        }
        return Ok(resp);
    }
}