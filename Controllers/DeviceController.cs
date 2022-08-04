using System;
using HttpDataServer.Core;
using HttpDataServer.Dtos.Account;
using HttpDataServer.Dtos.RespDto;
using HttpDataServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HttpDataServer.Controllers;

[ApiController, Route("[controller]")]
[Consumes("application/x-www-form-urlencoded"), Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class DeviceController : ControllerBase
{
    private readonly DeviceRepo deviceRepo;
    private RespDto resp = new RespDto();

    public DeviceController(DeviceRepo deviceRepo)
    {
        this.deviceRepo = deviceRepo;
    }

    [HttpPut("{firebaseCode}")]
    public IActionResult Put(string firebaseCode, [FromForm] DeviceUpdateDto device)
    {
        if (string.IsNullOrWhiteSpace(firebaseCode))
        {
            resp.Code = Code.ParametereError;
            return Ok(resp);
        }

        bool success = deviceRepo.Update(firebaseCode, device);
        if (success)
        {
            resp.Data = new { firebaseCode = firebaseCode };
        }

        resp.Code = deviceRepo.RespCode;
        return Ok(resp);
    }

    [HttpPatch("{firebaseCode}")]
    public IActionResult Patch(string firebaseCode, [FromForm] DateTime loginTime)
    {
        if (string.IsNullOrWhiteSpace(firebaseCode))
        {
            resp.Code = Code.ParametereError;
            return Ok(resp);
        }

        bool success = deviceRepo.UpdateTime(firebaseCode, loginTime);
        if (success)
        {
            resp.Data = new { FirebaseCode = firebaseCode };
        }
        resp.Code = deviceRepo.RespCode;
        return Ok(resp);
    }
}