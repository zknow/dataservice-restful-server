using DataServer.Core;
using DataServer.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DataServer.Dtos.Request.User;
using DataServer.Dtos.Response.User;
using StackExchange.Redis;

namespace DataServer.Controllers;

[ApiController, Route("[controller]")]
[Consumes("application/json"), Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class ValidationCodeController : ControllerBase
{
    private const string keyPrefix = "ValidationCode";

    private readonly HashSet<string> handleTypes = new() { "phone", "email", "password" };
    private readonly ValidationCodeRepo cache;

    private ValidationCodeSelectResponse resp = new ValidationCodeSelectResponse();

    public ValidationCodeController(ValidationCodeRepo repo)
    {
        this.cache = repo;
    }

    [HttpGet("{uid}")]
    public IActionResult Get(long uid, [FromBody] string type)
    {
        string handleType = type?.ToLower();
        if (!handleTypes.Contains(handleType))
        {
            resp.ErrorCode = ErrorCode.ParametereError;
            return Ok(resp);
        }

        string key = $"{keyPrefix}:{handleType}:{uid}";
        if (!cache.CheckExists(key))
        {
            resp.ErrorCode = ErrorCode.VerificationCodeNotFound;
            return Ok(resp);
        }

        if (cache.Get(key, out HashEntry data))
        {
            resp.ErrorCode = ErrorCode.Success;
            resp.ValidationCode = (int)data.Name;
            resp.ExtraValue = data.Value;
        }
        else
        {
            resp.ErrorCode = cache.ErrCode;
        }

        return Ok(resp);
    }

    [HttpPost]
    public IActionResult Post([FromBody] ValidationCodeInsertRequest data)
    {
        string typeStr = data.Type.ToLower();
        if (handleTypes.Contains(typeStr))
        {
            string key = $"{keyPrefix}:{typeStr}:{data.UID}";
            cache.Set(key, data.Code, data.ExtraData, data.ExpireSecond);
            resp.ErrorCode = cache.ErrCode;
        }
        else
        {
            resp.ErrorCode = ErrorCode.ParametereError;
        }
        return Ok(resp);
    }
}