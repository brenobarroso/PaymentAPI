using api.Interfaces;
using api.Models;
using api.Models.Withdraw;
using api.Models.Withdraws;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;

namespace api.Managers;

public class WithdrawManager : IWithdrawManager
{
    private readonly PaymentDbContext _context;
    private readonly IAccountManager _accountManager;

    public WithdrawManager(PaymentDbContext context, IAccountManager accountManager)
    {
        _context = context;
        _accountManager = accountManager;
    }

    public async Task<List<Withdraw>> GetAllWithdrawsAsync()
    {
        var result = await _context.Withdraws.ToListAsync();

        return result;
    }

    public async Task<(Withdraw? account, bool sucess)> MakeWithdraw (string accountNumber, decimal value)
    {
        var query = await _accountManager.GetByAccountNumberAsync(accountNumber);
        if(query == null)
            return (null, false);

        if(value > query.Balance)
        {
            var reprovedWithdraw = new Withdraw{
                Account = query,
                Value = value,
                DisapprovalDate = DateTime.UtcNow,
                Comments = "saldo insuficiente! Por isso reprovado.",
                Type = 01
            };

            await _context.Withdraws.AddAsync(reprovedWithdraw);
            await _context.SaveChangesAsync();

            return (reprovedWithdraw, false);
        }
        
        query.Balance = query.Balance - value;
        var approvedWithdraw = new Withdraw{
            Account = query,
            Value = value,
            ApprovalDate = DateTime.UtcNow,
            Comments = "Aprovado! Retirado R$ " + value + " restando R$ " + query.Balance + ".",
            Type = 01
        };
        await _context.Withdraws.AddAsync(approvedWithdraw);
        await _context.SaveChangesAsync();

        return (approvedWithdraw, true);
    }
}
