using api.Models.Withdraws;
using PaymentAPI.Models;

namespace api.Models;

public class AccountResult
{
    public int Id { get; set; }

    public string CPF { get; set; } = " ";

    public string Agency { get; set; } = " ";

    public string AccountNumber { get; set; }

    public string HolderName { get; set; } = " ";

    public decimal Balance { get; set; }

    public bool IsActive { get; set; }

    public ICollection<api.Models.Withdraws.Withdraw> Withdraws { get; set; }
}
