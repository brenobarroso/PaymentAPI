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

    public async Task<List<string>> GetByAccountIdAsync(int accountId, int index, int lenth)
    {
        int defaultValeuExtract = 10; // valor default da quantidade de itens no extrato;
        int maximumValueExtract = 90; // valor maximo da quantidade de itens no extrato;

        // Validação quantidade de itens
        if(lenth == 0 || lenth == null) lenth = defaultValeuExtract; // trata caso o valor passado seja 0 ou nulo.
        if(lenth >= maximumValueExtract) lenth = maximumValueExtract; // trata caso o valor seja maior que o máximo permitido.
    

        try
        {
            var query = new List<string>();

            var movements = await _context.Movements
                                .Where(x => x.AccountId == accountId)
                                .Skip(index)
                                .Take(lenth)
                                .Select(x => new MovementResult{
                                    Id = x.Id,
                                    AccountId = x.AccountId,
                                    Date = x.Date,
                                    Value = x.Value,
                                    Comments = x.Comments
                                })
                                .ToListAsync();

            foreach (var movement in movements)
            {
                query.Add(movement.Comments);
            }
            return query;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Exception thrown");
        }

    }
}
