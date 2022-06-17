using System;
using HttpDataServer.Core;
using HttpDataServer.Dtos.Account;
using HttpDataServer.Dtos.RespDto;
using HttpDataServer.Models;
using HttpDataServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HttpDataServer.Controllers;

[ApiController, Route("[controller]")]
[Consumes("application/x-www-form-urlencoded"), Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class AccountController : ControllerBase
{
    private readonly AccountRepo accountRepo;
    private RespDto resp = new RespDto();

    public AccountController(AccountRepo accountRepo)
    {
        this.accountRepo = accountRepo;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var accounts = accountRepo.GetAll();
        if (accountRepo.RespCode != Code.Success)
        {
            resp.Code = accountRepo.RespCode;
        }
        else
        {
            resp.Data = accounts;
        }
        return Ok(resp);
    }

    [HttpGet("{uid}")]
    public IActionResult Get(Guid uid)
    {
        if (uid.Equals(Guid.Empty))
        {
            resp.Code = Code.ParametereError;
        }
        else
        {
            resp.Data = accountRepo.Get(uid);
            resp.Code = accountRepo.RespCode;
        }

        return Ok(resp);
    }

    [HttpPost]
    public IActionResult Post([FromForm] AccountPostDto data)
    {
        Guid uid = Guid.NewGuid();
        Account player = data.ToPlayer(uid);
        Device device = data.ToDevice(uid);

        if (accountRepo.Insert(player, device))
        {
            resp.Data = new { UID = uid };
        }
        resp.Code = accountRepo.RespCode;
        return Ok(resp);
    }

    [HttpPatch("{uid}")]
    public IActionResult Patch(Guid uid, [FromForm] AccountUpdateDto data)
    {
        if (!ModelState.IsValid || uid.Equals(Guid.Empty))
        {
            resp.Code = Code.ParametereError;
            return Ok(resp); ;
        }

        if (accountRepo.Update(uid, data))
        {
            resp.Data = new { UID = uid };
        }
        resp.Code = accountRepo.RespCode;
        return Ok(resp);
    }
}