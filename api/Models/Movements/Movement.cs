using System.ComponentModel.DataAnnotations;
using PaymentAPI.Models;

namespace api.Models.Movements;

public class Movement
{
    // Account
    [Required]
    public Account Account { get; set; }
    
    [Required]
    public int AccountId { get; set; } 

    // Columns

    [Required]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public decimal NetValue { get; set; }

    public decimal? GrossValue { get; set; }

    [Required]
    public string Comments { get; set; } = " ";

    // Withdraw
    public api.Models.Withdraws.Withdraw? Withdraw { get; set; }

    public int? WithdrawId { get; set; }

    // Transaction
    public Payment? Payment { get; set; }

    public int? PaymentId { get; set; }
}
