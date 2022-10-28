using System;
using DataServer.Core;
using DataServer.Dtos.Request.User;
using DataServer.Dtos.Response.User;
using DataServer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataServer.Controllers;

[ApiController, Route("[controller]")]
[Consumes("application/json"), Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class DeviceController : ControllerBase
{
    private readonly DeviceRepo deviceRepo;

    public DeviceController(DeviceRepo deviceRepo)
    {
        this.deviceRepo = deviceRepo;
    }

    [HttpPut("{firebaseCode}")]
    [ProducesResponseType(typeof(DeviceUpdateResponse), StatusCodes.Status200OK)]
    public IActionResult Put(string firebaseCode, [FromBody] DeviceUpdateRequest device)
    {
        var resp = new DeviceUpdateResponse();
        if (string.IsNullOrWhiteSpace(firebaseCode))
        {
            return Ok(resp);
        }

        bool success = deviceRepo.Update(firebaseCode, device);
        if (success)
        {
            resp.ErrorCode = ErrorCode.Success;
            resp.FirebaseCode = firebaseCode;
        }
        else
        {
            resp.ErrorCode = deviceRepo.ErrCode;
        }

        return Ok(resp);
    }

    [HttpPatch("{firebaseCode}")]
    [ProducesResponseType(typeof(DeviceLoginTimeUpdateResponse), StatusCodes.Status200OK)]
    public IActionResult Patch(string firebaseCode, [FromBody] DateTime loginTime)
    {
        var resp = new DeviceLoginTimeUpdateResponse();
        if (string.IsNullOrWhiteSpace(firebaseCode))
        {
            return Ok(resp);
        }

        bool success = deviceRepo.UpdateTime(firebaseCode, loginTime);
        if (success)
        {
            resp.ErrorCode = ErrorCode.Success;
            resp.FirebaseCode = firebaseCode;
        }
        else
        {
            resp.ErrorCode = deviceRepo.ErrCode;
        }

        return Ok(resp);
    }
}