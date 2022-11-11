using api.Models.Movements;

namespace api.Interfaces;

public interface IExtractManager
{
    Task<List<string>> GetByAccountIdAsync(int accountId, int startIndex, int extractCount);
}    
