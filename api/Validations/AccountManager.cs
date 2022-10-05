using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;

namespace api.Validations;

public class AccountManager : IAccountManager
{
    private readonly PaymentDbContext _context;
    public AccountManager(PaymentDbContext context) => _context = context;

    public async Task<List<Account>> getAllAccountsAsync()
    {
        var result = await _context.Accounts.ToListAsync();
        return result;
    }

    public async Task<Account?> getByCPFAsync(string cpf)
    {
        var result = await _context.Accounts
                        .Where(x => x.CPF == cpf)
                        .Where(x => x.IsActive)
                        .FirstOrDefaultAsync();

        if (result == null)
            return null;
        if(result.IsActive == false)
            return null;
    
        return result;
    }

    public async Task<Account?> getByAccountNumberAsync(int idAccount)
    {
        var result = await _context.Accounts.FindAsync(idAccount);

        if (result == null)
            return null;
        if(result.IsActive == false)
            return null;

        return result;
    }

    public async Task<Account> CreateAccount(Account person)
    {
        var query = await getByCPFAsync(person.CPF);
        if(query != null)
            return null;

        await _context.Accounts.AddAsync(person);
        await _context.SaveChangesAsync();

        return person;
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
}
