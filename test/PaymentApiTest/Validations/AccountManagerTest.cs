using api.Models;
using api.Validations;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;

namespace PaymentApiTest.Validations;

public class AccountManagerTest
{
    protected readonly PaymentDbContext _context;
    public AccountManagerTest()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDbContext(options);

        _context.Database.EnsureCreated();
    }
    
    [Fact]
    public async Task ShouldMakeInactive()
    {
        // Arrange
        var newAccount = new Account{
            Id = 2,
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var manager = new AccountManager(_context);
        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.DeleteAccount(newAccount.Id);

        // Assert
        Assert.Equal(false, result.IsActive);
    }

    [Fact]
    public async Task ShouldBeCreated()
    {
        // Arrange
        var newAccount = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var manager = new AccountManager(_context);

        // Act
        var result = manager.CreateAccount(newAccount);

        // Assert
        Assert.NotNull(newAccount);
    }

    [Fact]
    public async Task ShouldListAll()
    {
        // Arrange
        var manager = new AccountManager(_context);

        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount2 = new Account{
            CPF = "12375678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);
        
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.getAllAccountsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ShouldListByCPF()
    {
        // Arrange
        var manager = new AccountManager(_context);

        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.getByCPFAsync("12315678901");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccount3.Id, result.Id);

    }

    [Fact]
    public async Task ShouldNotListByCPF()
    {
        // Arrange
        var manager = new AccountManager(_context);

        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.getByCPFAsync("12315678961");

        // Assert
        Assert.Null(result);

    }

    [Fact]
    public async Task ShouldListById()
    {
        // Arrange
        var manager = new AccountManager(_context);

        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.getByAccountNumberAsync(newAccount1.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccount1.Id, result.Id);

    }

    [Fact]
    public async Task ShouldNotListById()
    {
        // Arrange
        var manager = new AccountManager(_context);

        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.getByAccountNumberAsync(5000);

        // Assert
        Assert.Null(result);

    }
}
