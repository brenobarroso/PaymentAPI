using api.Interfaces;
using api.Models.Extract;
using api.Models.Movements;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using System;

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
        var result = new List<string>();

        //tratamento
        

        if(viewModel.Length == 0 || viewModel.Length <= 0) viewModel.Length = defaultValeuExtract;
        if(viewModel.Length >= maximumValueExtract) viewModel.Length = maximumValueExtract;
    
        try
        {
            var parsedStartDate = viewModel.StartDate?.ToUniversalTime().Date;
            var parsedEndDate = viewModel.EndDate?.ToUniversalTime().Date;

            var query = _context.Movements
                                .Where(x => x.AccountId == accountId);

            var count = await query.LongCountAsync();
                                    
            if(viewModel.JustIn == true)
                query = query.Where(x => x.Comments.Contains("entrada"));

            if(viewModel.JustOut == true)
                query = query.Where(x => x.Comments.Contains("saída"));

            if(viewModel.StartDate.HasValue && viewModel.EndDate.HasValue)
            {
                if(viewModel.StartDate == viewModel.EndDate)
                    query = query.Where(x => x.Date.Date == parsedStartDate);

                query = query.Where(x => x.Date.Date >= parsedStartDate);
                query = query.Where(x => x.Date.Date <= parsedEndDate);

            }

            if(viewModel.StartDate.HasValue && (viewModel.EndDate.HasValue == false))
                query = query.Where(x => x.Date.Date >= parsedStartDate);

            if(viewModel.EndDate.HasValue && (viewModel.StartDate.HasValue == false))
                query = query.Where(x => x.Date.Date <= parsedEndDate);

            var list = await query
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

            foreach (var movement in list)
            {
                    result.Add(movement.Comments);
            }
            
            await _context.SaveChangesAsync();

            var extract = new ExtractResult{
                Index = viewModel.Index,
                Length = viewModel.Length,
                Itens = result,
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
