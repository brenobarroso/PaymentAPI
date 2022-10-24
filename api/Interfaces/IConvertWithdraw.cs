using api.Models.Withdraw;
using api.Models.Withdraws;

namespace api.Interfaces;

public interface IConvertWithdraw
{
    WithdrawResult ConvertToResultWithdraw(Withdraw withdraw);  
}
