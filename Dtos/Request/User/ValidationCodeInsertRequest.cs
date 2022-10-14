using System;
using System.ComponentModel.DataAnnotations;

namespace DataServer.Dtos.Request.User;

public class ValidationCodeInsertRequest
{
    [Required]
    public Guid UID { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    public string ExtraData { get; set; }

    public double ExpireSecond { get; set; }
}