using DataServer.Core;
using DataServer.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DataServer.Dtos.Request.User;
using DataServer.Dtos.Response.User;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http;

namespace DataServer.Controllers;

[ApiController, Route("[controller]")]
[Consumes("application/json"), Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class ValidCodeController : ControllerBase
{
    private const string keyPrefix = "ValidCode";

    private readonly HashSet<string> handleTypes = new() { "phone", "email", "password" };
    private readonly ValidCodeRepo repo;

    public ValidCodeController(ValidCodeRepo repo)
    {
        this.repo = repo;
    }

    [HttpGet("{uid}")]
    [ProducesResponseType(typeof(ValidCodeSelectResponse), StatusCodes.Status200OK)]
    public IActionResult Get(long uid, [FromQuery] string type)
    {
        var resp = new ValidCodeSelectResponse();
        string handleType = type?.ToLower();
        if (!handleTypes.Contains(handleType))
        {
            resp.ErrorCode = ErrorCode.ParametereError;
            return Ok(resp);
        }

        string key = $"{keyPrefix}:{handleType}:{uid}";
        if (!repo.CheckExists(key))
        {
            resp.ErrorCode = ErrorCode.VerificationCodeNotFound;
            return Ok(resp);
        }

        if (repo.Get(key, out HashEntry data))
        {
            resp.ErrorCode = ErrorCode.Success;
            resp.ValidationCode = data.Name;
            resp.ExtraValue = data.Value;
        }
        else
        {
            resp.ErrorCode = repo.ErrCode;
        }

        return Ok(resp);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ValidCodeInsertResponse), StatusCodes.Status200OK)]
    public IActionResult Post([FromBody] ValidCodeInsertRequest data)
    {
        var resp = new ValidCodeInsertResponse();
        string validCodeTpye = data.Type.ToLower();
        if (handleTypes.Contains(validCodeTpye))
        {
            string key = $"{keyPrefix}:{validCodeTpye}:{data.UID}";
            repo.Set(key, data.Code, data.ExtraData, data.ExpireSecond);
            resp.ErrorCode = repo.ErrCode;
        }
        else
        {
            resp.ErrorCode = ErrorCode.ParametereError;
        }
        return Ok(resp);
    }
}