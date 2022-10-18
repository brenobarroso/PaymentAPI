using api.Managers;
using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

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
            IsActive = true,
            Payments = new List<Payment>()
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
        var newAccount = new AccountViewModel{
            CPF = "12345678901",
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
        };

        var manager = new AccountManager(_context);

        // Act
        var result = manager.CreateAccount(newAccount);

        // Assert
        Assert.NotNull(newAccount);
        Assert.Equal(newAccount.CPF, result.Result.CPF);
        Assert.Equal(newAccount.Agency, result.Result.Agency);
        Assert.Equal(newAccount.HolderName, result.Result.HolderName);
        Assert.NotNull(result.Result.Id);
        Assert.NotNull(result.Result.Payments);
        Assert.Equal(true, result.Result.IsActive);
    }

    [Fact]
    public async Task ShouldNotBeCreated()
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

        var invalidAccount = new AccountViewModel{
            CPF = "12315678901",
            HolderName = "Breno Barroso",
            Agency = "00239-9",
        };

        // Act
        var result = await manager.CreateAccount(invalidAccount);

        // Assert
        Assert.Null(result);

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
            IsActive = true,
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = "12375678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
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
            IsActive = true,
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
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
            IsActive = true,
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = "12315678900",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.getByCPFAsync("12315678901");

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
            IsActive = true,
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
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
            IsActive = true,
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.getByAccountNumberAsync(newAccount2.Id);

        // Assert
        Assert.Null(result);

    }
}
