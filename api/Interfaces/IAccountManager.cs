using api.Models;

namespace api.Interfaces;

public interface IAccountManager
{
    Task<Account> CreateAccount(Account person);
    Task<Account?> getByAccountNumberAsync(int idAccount);
    Task<Account?> getByCPFAsync(string cpf);
    Task<List<Account>> getAllAccountsAsync();
    Task<Account?> DeleteAccount(int idAccount);
}
