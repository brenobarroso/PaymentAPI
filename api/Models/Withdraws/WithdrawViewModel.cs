using System.ComponentModel.DataAnnotations;
using api.Validations.Withdraw;

namespace api.Models.Withdraw;

public class WithdrawViewModel
{
    [Required]
    [WithdrawPattern]
    public string AccountNumber { get; set; }

    [Required]
    [WithdrawValue]
    public decimal Value { get; set; }
}
