using api.Models;

namespace api.Interfaces;

public interface IAccountManager
{
    Task<Account?> CreateAccount(AccountViewModel person);
    Task<Account?> getByAccountNumberAsync(int idAccount);
    Task<Account?> getByCPFAsync(string cpf);
    Task<List<Account>> getAllAccountsAsync();
    Task<Account?> DeleteAccount(int idAccount);
    AccountResult ConvertToResult(Account account);
}
