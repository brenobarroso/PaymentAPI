using api.Models;

namespace api.Interfaces;

public interface IAccountManager
{
    Task<Account?> CreateAccount(AccountViewModel person);
    Task<Account?> GetByAccountNumberAsync(string accountNumber);
    Task<Account?> GetByCPFAsync(string cpf);
    Task<List<Account>> GetAllAccountsAsync();
    Task<Account?> DeleteAccount(int idAccount);
    AccountResult ConvertToResult(Account account);
}
