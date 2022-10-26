using api.Interfaces;
using api.Managers;
using api.Models;
using api.Models.Withdraws;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentApiTest.Validations;

public class WithdrawManagerTest
{
    protected readonly PaymentDbContext _context;
    private readonly IAccountManager _accountManager;

    public WithdrawManagerTest()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDbContext(options);

        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task ShouldListAll()
    {
        // Arrange
        var manager = new WithdrawManager(_context, _accountManager);

        var withdraw1 = new Withdraw{
            Value = 150m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw2 = new Withdraw{
            Value = 100m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw3 = new Withdraw{
            Value = 150m,
            Date = DateTime.UtcNow,
            ApprovalDate = null,
            DisapprovalDate = DateTime.UtcNow,
            Comments = "Saque genérico.",
            Type = 1
        };

        _context.Withdraws.Add(withdraw1);
        _context.Withdraws.Add(withdraw2);
        _context.Withdraws.Add(withdraw3);

        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetAllWithdrawsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Saque genérico.", result.First().Comments);
    }

    [Fact]
    public async Task ShouldListById()
    {
        // Arrange
        var manager = new WithdrawManager(_context, _accountManager);

        var withdraw1 = new Withdraw{
            AccountId = 1,
            Value = 150m,
            Date = DateTime.UtcNow,
            ApprovalDate = null,
            DisapprovalDate = DateTime.UtcNow,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw2 = new Withdraw{
            AccountId = 1,
            Value = 100m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        _context.Withdraws.Add(withdraw1);
        _context.Withdraws.Add(withdraw2);
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetWithdrawsByIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task ShouldNotListById()
    {
        // Arrange
        var manager = new WithdrawManager(_context, _accountManager);

        var withdraw1 = new Withdraw{
            AccountId = 1,
            Value = 150m,
            Date = DateTime.UtcNow,
            ApprovalDate = null,
            DisapprovalDate = DateTime.UtcNow,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw2 = new Withdraw{
            AccountId = 1,
            Value = 100m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        _context.Withdraws.Add(withdraw1);
        _context.Withdraws.Add(withdraw2);
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetWithdrawsByIdAsync(3);

        // Assert
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public async Task ShouldMakeAWithdraw()
    {
        // Arrange
        
        var accountForTest = new Account{
            HolderName = "Pessoa de Teste",
            Balance = 1500m,
            Agency = "0001",
            AccountNumber = "0000001",
            CPF = "00000000000",
            IsActive = true,
            Payments = new List<Payment>(),
            Withdraws = new List<Withdraw>()
        };

        _context.Accounts.Add(accountForTest);
        await _context.SaveChangesAsync();

        Mock<IAccountManager> test = new Mock<IAccountManager>();
        test.Setup(x => x.GetByAccountNumberAsync(accountForTest.AccountNumber)).ReturnsAsync(accountForTest);
        var manager = new WithdrawManager(_context, test.Object);
        
        // Act
        var result = await manager.MakeWithdraw(accountForTest.AccountNumber, 150m);
       
        // Assert
        Assert.Equal("Aprovado! Retirado R$ " + 150m + " restando R$ " + 1350m + ".", result.account.Comments);
        Assert.Equal(accountForTest.AccountNumber, result.account.Account.AccountNumber);
        Assert.Equal(DateTime.UtcNow.Date, result.account.ApprovalDate.Value.Date);
        Assert.Null(result.account.DisapprovalDate);
        Assert.Equal(01, result.account.Type);
        Assert.Equal(150m, result.account.Value);
        Assert.Equal(true, result.sucess);
    }

    [Fact]
    public async Task ShouldNotMakeAWithdraw()
    {
        // Arrange
        
        var accountForTest = new Account{
            HolderName = "Pessoa de Teste",
            Balance = 1500m,
            Agency = "0001",
            AccountNumber = "0000001",
            CPF = "00000000000",
            IsActive = true,
            Payments = new List<Payment>(),
            Withdraws = new List<Withdraw>()
        };

        _context.Accounts.Add(accountForTest);
        await _context.SaveChangesAsync();

        Mock<IAccountManager> test = new Mock<IAccountManager>();
        test.Setup(x => x.GetByAccountNumberAsync(accountForTest.AccountNumber)).ReturnsAsync(accountForTest);
        var manager = new WithdrawManager(_context, test.Object);
        
        // Act
        var result = await manager.MakeWithdraw(accountForTest.AccountNumber, 2000m);
       
        // Assert
        Assert.Equal("Saldo insuficiente! Por isso reprovado.", result.account.Comments);
        Assert.Equal(accountForTest.AccountNumber, result.account.Account.AccountNumber);
        Assert.Equal(DateTime.UtcNow.Date, result.account.DisapprovalDate.Value.Date);
        Assert.Null(result.account.ApprovalDate);
        Assert.Equal(01, result.account.Type);
        Assert.Equal(2000m, result.account.Value);
        Assert.Equal(false, result.sucess);
    }
}