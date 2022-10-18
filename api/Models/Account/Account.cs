using System.ComponentModel.DataAnnotations;
using PaymentAPI.Models;

namespace api.Models;

public class Account
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string CPF { get; set; } = " ";
    [Required]
    public string Agency { get; set; } = " ";
    [Required]
    public string AccountNumber { get; set; }
    [Required]
    public string HolderName { get; set; } = " ";
    [Required]
    public float Balance { get; set; }
    [Required]
    public bool IsActive { get; set; } = true;
    public ICollection<Payment> Payments { get; set; } // transações de uma conta.
}
