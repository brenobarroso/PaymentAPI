using System.Globalization;
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

    public async Task<ExtractResult> GetByAccountIdAsync(ExtractViewModel viewModel, int accountId)
    {
        int defaultValeuExtract = 60;
        int maximumValueExtract = 90;
        var query = new List<string>();

        if(viewModel.Length == 0 || viewModel.Length == 0) viewModel.Length = defaultValeuExtract;
        if(viewModel.Length >= maximumValueExtract) viewModel.Length = maximumValueExtract;
    
        try
        {
            var count = await _context.Movements
                                        .Where(x => x.AccountId == accountId)
                                        .LongCountAsync();
            var result = _context.Movements.Where(x => x.AccountId == accountId);

            if(viewModel.JustIn == true)
                result.Where(x => x.Comments.Contains("entrada"));

            if(viewModel.JustOut == true)
                result.Where(x => x.Comments.Contains("saída"));

            if(viewModel.StartDate != null && viewModel.EndDate != null)
                result.Where(x => x.Date >= viewModel.StartDate && x.Date <= viewModel.EndDate);

            if(viewModel.StartDate != null)
                result.Where(x => x.Date >= viewModel.StartDate);

            if(viewModel.EndDate != null)
                result.Where(x => x.Date <= viewModel.EndDate);

            await result.Skip(viewModel.Index)
                        .Take(viewModel.Length)
                        .Select(x => new MovementResult{
                            Id = x.Id,
                            AccountId = x.AccountId,
                            Date = x.Date,
                            Value = x.Value,
                            Comments = x.Comments
                        })
                        .ToListAsync();

            foreach (var movement in result)
            {
                    query.Add(movement.Comments);
            }
            
            await _context.SaveChangesAsync();

            var extract = new ExtractResult{
                Index = viewModel.Index,
                Length = viewModel.Length,
                Itens = query,
                Count = count
            };

            return extract;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Exception thrown");
        }
    }
}
