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

    public async Task<List<MovementResult>> GetByAccountIdAsync(int accountId)
    {
        var result = await _context.Movements
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

        return result;
    }
}
