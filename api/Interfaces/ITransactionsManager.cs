using api.Models;
using PaymentAPI.Models;

namespace api.Interfaces;

public interface ITransactionsManager
{
    Task<(Payment payment, bool sucess)> CreatAsync(PaymentViewModel viewModel);
    Task<List<Payment>> getAllAsync();
    Task<Payment?> getByIdAsync(int id);
}
