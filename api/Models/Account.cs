using System.ComponentModel.DataAnnotations;

namespace api.Models;

public enum Status
{
    Active,
    Inactive
}
public class Account
{
    [Required]
    public int IdAccount { get; set; }

    [Required]
    public string CPF { get; set; } = " ";

    [Required]
    public string Agency { get; set; } = " ";

    [Required]
    public string HolderName { get; set; } = " ";

    [Required]
    public float Balance { get; set; }

    [Required]
    public Status Status { get; set; }
}
