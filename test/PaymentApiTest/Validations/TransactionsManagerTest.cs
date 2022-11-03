using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using api.Managers;
using api.Interfaces;
using PaymentAPI.Models;
using Moq;
using api.Models.Withdraws;
using api.Models.Movements;

namespace PaymentApiTest.Validations;

public class TransactionsManagerTest
{
    protected readonly PaymentDbContext _context;
    protected readonly IAccountManager _manager;
    private readonly IConvertWithdraw _convert;
    public TransactionsManagerTest()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDbContext(options);

        _context.Database.EnsureCreated();
    }

    [Theory]
    [InlineData(5000, "1023654785698745", 1)]
    [InlineData(5000, "1023654787498745", 2)]
    [InlineData(5000, "1023054785698025", 3)]
    [InlineData(5000, "1023654485698025", 4)]
    [InlineData(5000, "1023654785098745", 5)]
    [InlineData(5000, "1023654787498745", 6)]
    [InlineData(5000, "1023054785697725", 7)]
    [InlineData(5000, "1027754485698025", 8)]
    public async Task ShouldConvertToApproved(decimal grossValue, string cardNumber, int installmentQuantity)
    {
        // Arrange
        var newAccount = new Account{
            Id = 2,
            CPF = "12345678901",
            Balance = 0m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Payments = new List<Payment>(),
            Withdraws = new List<Withdraw>(),
            Movements = new List<Movement>()
        };

        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber,
            InstallmentQuantity = installmentQuantity,
            AccountNumber = newAccount.AccountNumber
        };
        var managerAccount = new AccountManager(_context, _convert);

        var paymentNetValue = (decimal)(payment.GrossValue - 0.9m);
        

        Mock<IAccountManager> test = new Mock<IAccountManager>();
        
        test.Setup(x => x.GetByAccountNumberAsync(newAccount.AccountNumber)).ReturnsAsync(newAccount);
        var manager = new TransactionsManager(_context, test.Object);

        // Act
        var result = await manager.CreatAsync(payment);

        // Assert

        Assert.Equal(true, result.sucess);
        Assert.Equal(4, result.payment.CardNumber.Length);
        Assert.Equal(grossValue, result.payment.GrossValue);
        Assert.Equal((grossValue - result.payment.FlatRate), result.payment.NetValue);
        Assert.Equal(DateTime.UtcNow.Date, result.payment.TransationDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, result.payment.ApprovalDate.Value.Date);
        Assert.NotNull(result.payment.FlatRate);
        Assert.NotNull(result.payment.ApprovalDate);
        Assert.Null(result.payment.DisapprovalDate);
        Assert.Equal(true, result.payment.Confirmation);
        Assert.Equal(newAccount ,result.payment.Account);
        Assert.Equal(paymentNetValue, newAccount.Balance);

        Assert.NotNull(result.payment.Movement);
        Assert.Equal(result.payment.Account, result.payment.Movement.Account);
        Assert.Equal(result.payment.NetValue, result.payment.Movement.Value);
        Assert.Equal(DateTime.UtcNow.Date, result.payment.Movement.Date.Date);
        Assert.Equal(newAccount, result.payment.Movement.Account);
        Assert.Equal(newAccount.Id, result.payment.Movement.AccountId);
        Assert.Equal(result.payment, result.payment.Movement.Payment);
        Assert.Equal(result.payment.Id, result.payment.Movement.PaymentId);
        Assert.Null(result.payment.Movement.Withdraw);
        Assert.Null(result.payment.Movement.WithdrawId);

        var auxInstallmentNetValue = (result.payment.NetValue / payment.InstallmentQuantity) - result.payment.FlatRate;

        Assert.All(result.payment.Installments, 
                    p => Assert.NotNull(p.Id));
        Assert.All(result.payment.Installments, 
                    p => Assert.NotNull(p.ReceiptDate));
        Assert.All(result.payment.Installments,
                p => Assert.Equal(DateTime.UtcNow.AddDays(30 * p.InstallmentNumber).Date,  p.ReceiptDate.Date)
            );
        Assert.All(result.payment.Installments, 
                    p => Assert.Equal(auxInstallmentNetValue, p.InstallmentNetValue));
        Assert.All(result.payment.Installments, 
                    p => Assert.Equal((payment.GrossValue / payment.InstallmentQuantity)
                        , p.InstallmentGrossValue));
        Assert.All(result.payment.Installments, 
                    p => Assert.NotNull(p.InstallmentNumber));
    }

    [Theory]
    [InlineData(5000, "5999654785698745", 1)]
    [InlineData(5000, "5999654785698785", 1)]
    [InlineData(5000, "5999654775698745", 1)]
    public async Task ShouldConvertToReprovedAsync(decimal grossValue, string cardNumber, int installmentQuantity)
    {
        // Arrange
        var newAccount = new Account{
            Id = 2,
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Payments = new List<Payment>()
        };

        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber,
            InstallmentQuantity = installmentQuantity,
            AccountNumber = newAccount.AccountNumber
        };

        var managerAccount = new TransactionsManager(_context, _manager);

        Mock<IAccountManager> test = new Mock<IAccountManager>();
        
        test.Setup(x => x.GetByAccountNumberAsync(newAccount.AccountNumber)).ReturnsAsync(newAccount);
        var manager = new TransactionsManager(_context, test.Object);

        // Act

        var result = await manager.CreatAsync(payment);

        // Assert

        Assert.Equal(false, result.sucess);
        Assert.Equal(4, result.payment.CardNumber.Length);
        Assert.Equal(grossValue, result.payment.GrossValue);
        Assert.Null(result.payment.NetValue);
        Assert.Equal(DateTime.UtcNow.Date, result.payment.TransationDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, result.payment.DisapprovalDate.Value.Date);
        Assert.NotNull(result.payment.FlatRate);
        Assert.Null(result.payment.ApprovalDate);
        Assert.NotNull(result.payment.DisapprovalDate);
        Assert.Equal(false, result.payment.Confirmation);
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
