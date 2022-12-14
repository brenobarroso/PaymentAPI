using api.Models.Extract;
using api.Models.Movements;

namespace api.Interfaces;

public interface IExtractManager
{
    Task<ExtractResult> GetByAccountIdAsync(ExtractViewModel viewModel, int accountId);
}    
