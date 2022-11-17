using api.Interfaces;
using api.Models.Extract;
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

    public async Task<ExtractResult> GetByAccountIdAsync(int accountId, int index, int length)
    {
        int defaultValeuExtract = 10; // valor default da quantidade de itens no extrato;
        int maximumValueExtract = 90; // valor maximo da quantidade de itens no extrato;

        // Validação quantidade de itens
        if(length == 0 || length == null) length = defaultValeuExtract; // trata caso o valor passado seja 0 ou nulo.
        if(length >= maximumValueExtract) length = maximumValueExtract; // trata caso o valor seja maior que o máximo permitido.
    

        try
        {
            var query = new List<string>();

            var movements = await _context.Movements
                                .Where(x => x.AccountId == accountId)
                                .Skip(index)
                                .Take(length)
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

            var count = await _context.Movements
                                        .Where(x => x.AccountId == accountId)
                                        .LongCountAsync();

            var result = new ExtractResult{
                Index = index,
                Length = length,
                Itens = query,
                Count = count
            };

            return result;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Exception thrown");
        }


    }
}
