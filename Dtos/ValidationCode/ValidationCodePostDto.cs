using System;
using System.ComponentModel.DataAnnotations;

namespace HttpDataServer.Dtos.ValidationCode;

public class ValidationCodePostDto
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