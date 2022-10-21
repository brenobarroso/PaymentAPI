using api.Models.Withdraws;

namespace api.Interfaces
{
    public interface IWithdrawManager
    {
        Task<List<Withdraw>> GetAllWithdrawsAsync();
    }
}