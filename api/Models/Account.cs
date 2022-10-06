using System.ComponentModel.DataAnnotations;
using api.Validations;
using PaymentAPI.Models;

namespace api.Models;

public class Account
{
    [Required]
    public int Id { get; set; }

    [Required]
    [CPFAttribute(eChar: " ")]
    [CPFAttributePattern]
    public string CPF { get; set; } = " ";

    [Required]
    public string Agency { get; set; } = " ";

    [Required]
    [StringLength(50,MinimumLength=3)]
    public string HolderName { get; set; } = " ";

    [Required]
    public float Balance { get; set; }

    [Required]
    public bool IsActive { get; set; }

    public ICollection<Payment> Payments { get; set; } // transações de uma conta.
}
