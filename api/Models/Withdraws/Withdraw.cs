using System.ComponentModel.DataAnnotations;
using api.Models.Movements;

namespace api.Models.Withdraws;

public class Withdraw
{
    [Required]
    public Account Account { get; set; }
    public int AccountId { get; set; }
    public int Id { get; set; }
    public decimal Value { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovalDate { get; set; } = null;
    public DateTime? DisapprovalDate { get; set; } = null;
    public string Comments { get; set; } = " ";
    public int Type { get; set; }
    public Movement Movement { get; set; }
}
