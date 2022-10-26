using api.Models;
using api.Models.Withdraw;
using api.Models.Withdraws;

namespace api.Interfaces
{
    public interface IWithdrawManager
    {
        Task<List<WithdrawResult>> GetAllWithdrawsAsync();
        Task<(Withdraw? account, bool sucess)> MakeWithdraw (string accountNumber, decimal value);
        Task<List<WithdrawResult>> GetWithdrawsByIdAsync(int id);
    }
}