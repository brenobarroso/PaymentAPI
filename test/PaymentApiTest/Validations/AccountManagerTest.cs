using api.Interfaces;
using api.Managers;
using api.Models;
using api.Models.Withdraws;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentApiTest.Validations;

public class AccountManagerTest
{
    protected readonly PaymentDbContext _context;
    private readonly IConvertWithdraw _convert;
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
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>(),
            Withdraws = new List<Withdraw>()
        };

        var manager = new AccountManager(_context, _convert);
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

        var manager = new AccountManager(_context, _convert);

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
        var manager = new AccountManager(_context, _convert);

        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>(),
            Withdraws = new List<Withdraw>()
        };

        var newAccount2 = new Account{
            CPF = "12375678901",
            Balance = 5000m,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>(),
            Withdraws = new List<Withdraw>()
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000m,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>(),
            Withdraws = new List<Withdraw>()
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
        var manager = new AccountManager(_context, _convert);

        var newAccount1 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);
        
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetAllAccountsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ShouldListByCPF()
    {
        // Arrange
        var manager = new AccountManager(_context, _convert);

        var newAccount1 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByCPFAsync(newAccount1.CPF);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccount1.Id, result.Id);

    }

    [Fact]
    public async Task ShouldNotListByCPF()
    {
        // Arrange
        var manager = new AccountManager(_context, _convert);

        var newAccount1 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByCPFAsync("12315678901");

        // Assert
        Assert.Null(result);

    }

    [Fact]
    public async Task ShouldListByAccountNumber()
    {
        // Arrange
        var manager = new AccountManager(_context, _convert);

        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = "12315678901",
            Balance = 5000m,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000m,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountNumberAsync(newAccount1.AccountNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccount1.AccountNumber, result.AccountNumber);

    }

    [Fact]
    public async Task ShouldNotListById()
    {
        // Arrange
        var manager = new AccountManager(_context, _convert);

        var newAccount1 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = false,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account{
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        _context.Accounts.Add(newAccount2);
        _context.Accounts.Add(newAccount3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountNumberAsync(newAccount2.AccountNumber);

        // Assert
        Assert.Null(result);

    }

    public static string CreateRandomStringBySize(int size)
    {
        var chars = "0123456789";
        var random = new Random();
        var result = new string(
            Enumerable.Repeat(chars, size)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
        return result;
    }
}
