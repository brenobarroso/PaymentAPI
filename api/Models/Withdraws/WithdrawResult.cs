namespace api.Models.Withdraw;

public class WithdrawResult
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovalDate { get; set; } = null;
    public DateTime? DisapprovalDate { get; set; } = null;
    public string Comments { get; set; } = " ";
    public int Type { get; set; }
}
