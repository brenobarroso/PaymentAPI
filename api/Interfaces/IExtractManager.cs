using api.Models.Movements;

namespace api.Interfaces;

public interface IExtractManager
{
    Task<List<MovementResult>> GetByAccountIdAsync(int accountId);
}    
