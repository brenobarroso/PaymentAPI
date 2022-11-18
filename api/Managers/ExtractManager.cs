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

    public Task<ExtractResult> GetByAccountIdAsync(ExtractViewModel viewModel, int accountId)
    {
        int defaultValeuExtract = 60;
        int maximumValueExtract = 90;

        if(viewModel.Length == 0 || viewModel.Length == 0) viewModel.Length = defaultValeuExtract;
        if(viewModel.Length >= maximumValueExtract) viewModel.Length = maximumValueExtract;
    
        try
        {
            if(viewModel.JustIn == true)
            {
                var resultIn = TakeJustIn(accountId, viewModel);

                return resultIn;
            }

            if(viewModel.JustOut == true)
            {
                var resultOut = TakeJustOut(accountId, viewModel);

                return resultOut;
            }

            var result = Take(accountId, viewModel);

            return result;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Exception thrown");
        }
    }

    public async Task<ExtractResult> TakeJustIn(int accountId, ExtractViewModel viewModel)
    {
        var query = new List<string>();

        var count = await _context.Movements
                                        .Where(x => x.AccountId == accountId)
                                        .LongCountAsync();

        var movements = await _context.Movements
                                .Where(x => x.AccountId == accountId)
                                .Where(x => x.Comments.Contains("entrada"))
                                .Skip(viewModel.Index)
                                .Take(viewModel.Length)
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

        await _context.SaveChangesAsync();

        var resultIn = new ExtractResult{
        Index = viewModel.Index,
        Length = viewModel.Length,
        Itens = query,
        Count = count
        };

        return resultIn;
    }

    public async Task<ExtractResult> TakeJustOut(int accountId, ExtractViewModel viewModel)
    {
        var query = new List<string>();

        var count = await _context.Movements
                                        .Where(x => x.AccountId == accountId)
                                        .LongCountAsync();

        var movements = await _context.Movements
                                .Where(x => x.AccountId == accountId)
                                .Where(x => x.Comments.Contains("saída"))
                                .Skip(viewModel.Index)
                                .Take(viewModel.Length)
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

        await _context.SaveChangesAsync();

        var resultOut = new ExtractResult{
        Index = viewModel.Index,
        Length = viewModel.Length,
        Itens = query,
        Count = count
        };

        return resultOut;
    }

    public async Task<ExtractResult> Take(int accountId, ExtractViewModel viewModel)
    {
        var query = new List<string>();

        var count = await _context.Movements
                                        .Where(x => x.AccountId == accountId)
                                        .LongCountAsync();

        var movements = await _context.Movements
                                .Where(x => x.AccountId == accountId)
                                .Skip(viewModel.Index)
                                .Take(viewModel.Length)
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
        
        await _context.SaveChangesAsync();

        var result = new ExtractResult{
        Index = viewModel.Index,
        Length = viewModel.Length,
        Itens = query,
        Count = count
        };

        return result;
    }
}
