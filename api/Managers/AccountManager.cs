using api.Interfaces;
using api.Models;
using api.Models.Withdraw;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace api.Managers;

public class AccountManager : IAccountManager
{
    private readonly PaymentDbContext _context;
    private readonly IConvertWithdraw _convert;
    public AccountManager(PaymentDbContext context, IConvertWithdraw convert)
    {
        _context = context;
        _convert = convert;
    }

    public async Task<List<Account>> GetAllAccountsAsync() // Tested
    {
        var result = await _context.Accounts
                                    .Include(x => x.Withdraws)
                                    .Include(x => x.Payments)
                                    .ThenInclude(x => x.Installments)
                                    .ToListAsync();
        return result;
    }

    public async Task<Account?> GetByCPFAsync(string cpf) // Tested
    {
        var result = await _context.Accounts
                        .Include(x => x.Withdraws)
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

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber) // Tested
    {
        var result = await _context.Accounts
                                    .Include(x => x.Withdraws)
                                    .Include(x => x.Payments)
                                    .ThenInclude(x => x.Installments)
                                    .Where(x => x.AccountNumber == accountNumber).SingleOrDefaultAsync();

        if (result == null)
            return null;
        if(result.IsActive == false)
            return null;

        return result;
    }

    public async Task<Account?> CreateAccount(AccountViewModel viewModel) // Tested
    {
        var queryLastAccount = await GetAllAccountsAsync();
        if(queryLastAccount.Count == 0){
            var firstAccount = new Account
            {
                CPF = viewModel.CPF,
                Agency = viewModel.Agency,
                HolderName = viewModel.HolderName,
                IsActive = true,
                AccountNumber = "0000001"
            };

            var firstAccountPayments = new List<Payment>();
            firstAccount.Payments = firstAccountPayments;

            await _context.Accounts.AddAsync(firstAccount);
            await _context.SaveChangesAsync();

            return firstAccount;
        }
        
        var query = await GetByCPFAsync(viewModel.CPF);
        if(query != null)
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

    public async Task<Account?> DeleteAccount(int idAccount) // Tested
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
        var withdrawsResult = new List<WithdrawResult>();

        var accountResult = new AccountResult
        {
            Id = account.Id,
            CPF = account.CPF,
            Agency = account.Agency,
            HolderName = account.HolderName,
            Balance = (decimal)account.Balance,
            IsActive = account.IsActive,
            AccountNumber = account.AccountNumber,
        };
        
        foreach (var withdraw in account.Withdraws)
        {
            var resultWithdraw = _convert.ConvertToResultWithdraw(withdraw);
            withdrawsResult.Add(resultWithdraw);
        }

        accountResult.Withdraws = withdrawsResult;
        
        _context.SaveChangesAsync();
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
