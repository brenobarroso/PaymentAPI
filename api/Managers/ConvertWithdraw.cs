using api.Interfaces;
using api.Models.Withdraw;
using api.Models.Withdraws;

namespace api.Managers;

public class ConvertWithdraw : IConvertWithdraw
{
    public WithdrawResult ConvertToResultWithdraw(Withdraw withdraw)
    {
        var withdrawResult = new WithdrawResult{
            Value = withdraw.Value,
            Date = withdraw.Date,
            ApprovalDate = withdraw.ApprovalDate,
            DisapprovalDate = withdraw.DisapprovalDate,
            Comments = withdraw.Comments,
            Type = withdraw.Type
        };

        return withdrawResult;
    }
}
