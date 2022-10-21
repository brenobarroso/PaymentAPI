using System.ComponentModel.DataAnnotations;

namespace api.Models.Withdraw;

public class WithdrawViewModel
{
    [Required]
    public string AccountNumber { get; set; }
    [Required]
    public decimal Value { get; set; }
}
