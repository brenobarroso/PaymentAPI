using api.Interfaces;
using api.Models.Movements;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;

namespace api.Managers;

public class ExtractManager : IExtractManager
{
    private readonly PaymentDbContext _context;
    private readonly IAccountManager _accountManager;

    public ExtractManager(PaymentDbContext context, IAccountManager accountManager)
    {
        _context = context;
        _accountManager = accountManager;
    }

    public async Task<List<string>> GetByAccountIdAsync(int accountId)
    {
        var result = new List<string>();

        var movements = await _context.Movements
                                .Where(x => x.AccountId == accountId)
                                .Select(x => new MovementResult{
                                    Id = x.Id,
                                    AccountId = x.AccountId,
                                    Date = x.Date,
                                    NetValue = x.NetValue,
                                    GrossValue = null,
                                    Comments = x.Comments
                                })
                                .ToListAsync();

        string extractHeader = $"***Extrato da conta {movements.First().AccountId} - {DateTime.UtcNow.Date}*** ";
        result.Add(extractHeader);

        foreach (var movement in movements)
        {
            result.Add(movement.Comments);
        }

        return result;
    }
}
