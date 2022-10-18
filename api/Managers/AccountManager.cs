using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace api.Managers;

public class AccountManager : IAccountManager
{
    private readonly PaymentDbContext _context;
    public AccountManager(PaymentDbContext context) => _context = context;

    public async Task<List<Account>> getAllAccountsAsync()
    {
        var result = await _context.Accounts
                                    .Include(x => x.Payments)
                                    .ThenInclude(x => x.Installments)
                                    .ToListAsync();
        return result;
    }

    public async Task<Account?> getByCPFAsync(string cpf)
    {
        var result = await _context.Accounts
                        .Include(x => x.Payments)
                        .ThenInclude(x => x.Installments)
                        .Where(x => x.CPF == cpf)
                        .Where(x => x.IsActive)
                        .FirstOrDefaultAsync();

        if (result == null)
            return null;
        if(result.IsActive == false)
            return null;
    
        return result;
    }

    public async Task<Account?> getByAccountNumberAsync(string accountNumber)
    {
        var result = await _context.Accounts.Include(x => x.Payments)
                                            .ThenInclude(x => x.Installments)
                                            .Where(x => x.AccountNumber == accountNumber).SingleOrDefaultAsync();

        if (result == null)
            return null;
        if(result.IsActive == false)
            return null;

        return result;
    }

    public async Task<Account?> CreateAccount(AccountViewModel viewModel)
    {
        var query = await getByCPFAsync(viewModel.CPF);
        if(query != null)
            return null;

        var queryLastAccount = await getAllAccountsAsync();
        if(queryLastAccount == null)
            return null;

        var lastAccount = queryLastAccount.LastOrDefault();
        var accountNumber = CreateAccountNumber(lastAccount);

        var newAccount = new Account
        {
            CPF = viewModel.CPF,
            Agency = viewModel.Agency,
            HolderName = viewModel.HolderName,
            IsActive = true,
            AccountNumber = accountNumber
        };

        var payments = new List<Payment>();
        newAccount.Payments = payments;

        await _context.Accounts.AddAsync(newAccount);
        await _context.SaveChangesAsync();

        return newAccount;
    }

    public async Task<Account?> DeleteAccount(int idAccount)
    {
        var result = await _context.Accounts.FindAsync(idAccount);
        if (result == null)
            return null;

        result.IsActive = false;
        await _context.SaveChangesAsync();

        return result;
    }

    public AccountResult ConvertToResult(Account account)
    {
        var accountResult = new AccountResult
            {
                Id = account.Id,
                CPF = account.CPF,
                Agency = account.Agency,
                HolderName = account.HolderName,
                Balance = account.Balance,
                IsActive = account.IsActive,
                AccountNumber = account.AccountNumber
            };

        return accountResult;
    }

    public string CreateAccountNumber(Account account)
    {
        var toInt = Int32.Parse(account.AccountNumber);
        toInt++;
        var toString = toInt.ToString().PadLeft(7, '0');
        return toString;
    }
}
