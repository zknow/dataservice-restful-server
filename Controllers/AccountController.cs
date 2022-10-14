using DataServer.Dtos.Request.User;
using DataServer.Dtos.Response.User;
using DataServer.Dtos.Sql;
using DataServer.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DataServer.Controllers;

[ApiController, Route("[controller]")]
[Consumes("application/json"), Produces("application/json")]
[ApiConventionType(typeof(DefaultApiConventions))]
public class AccountController : ControllerBase
{
    private readonly AccountRepo accountRepo;
    private static object syncLock = new object();

    public AccountController(AccountRepo accountRepo)
    {
        this.accountRepo = accountRepo;
    }

    [HttpGet("{uid}")]
    public IActionResult Get(long uid)
    {
        var resp = new AccountSelectResponse();
        resp.ErrorCode = accountRepo.ErrCode;
        resp.Data = accountRepo.Get(uid);
        return Ok(resp);
    }

    [HttpPost]
    public IActionResult Post([FromBody] AccountInsertRequest data)
    {
        var resp = new AccountInsertResponse();
        lock (syncLock)
        {
            if (accountRepo.TryGetUserSN(out long sn))
            {
                Account player = data.ToPlayer(sn);
                Device device = data.ToDevice(sn);
                if (accountRepo.Insert(player, device))
                {
                    resp.UID = sn;
                }
            }
        }
        resp.ErrorCode = accountRepo.ErrCode;
        return Ok(resp);
    }

    [HttpPatch("{uid}")]
    public IActionResult Patch(long uid, [FromBody] AccountUpdateRequest data)
    {
        var resp = new AccountUpdateResponse();
        if (!ModelState.IsValid)
        {
            return Ok(resp); ;
        }

        if (accountRepo.Update(uid, data))
        {
            resp.UID = uid;
        }
        resp.ErrorCode = accountRepo.ErrCode;
        return Ok(resp);
    }
}