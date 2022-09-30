using api.Models;

namespace api.Interfaces;

public interface IAccountManager
{
    Task<Account> CreateAccount(Account person);
    Task<Account?> getByAccountNumberAsync(string idAccount);
    Task<Account?> getByCPFAsync(string cpf);
    Task<List<Account>> getAllAccountsAsync();

    // TODO: colocar função do delete
}
