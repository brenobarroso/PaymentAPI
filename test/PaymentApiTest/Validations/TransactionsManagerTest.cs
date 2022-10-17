using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using api.Managers;
using api.Interfaces;
using PaymentAPI.Models;
using Moq;

namespace PaymentApiTest.Validations;

public class TransactionsManagerTest
{
    protected readonly PaymentDbContext _context;
    protected readonly IAccountManager _manager;
    public TransactionsManagerTest()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDbContext(options);

        _context.Database.EnsureCreated();
    }

    [Theory]
    [InlineData(5000f, "1023654785698745", 1)]
    [InlineData(5000f, "1023654787498745", 2)]
    [InlineData(5000f, "1023054785698025", 3)]
    [InlineData(5000f, "1023654485698025", 4)]
    [InlineData(5000f, "1023654785098745", 5)]
    [InlineData(5000f, "1023654787498745", 6)]
    [InlineData(5000f, "1023054785697725", 7)]
    [InlineData(5000f, "1027754485698025", 8)]
    public async Task ShouldConvertToApproved(float grossValue, string cardNumber, int installmentQuantity)
    {
        // Arrange
        var newAccount = new Account{
            Id = 2,
            CPF = "12345678901",
            Balance = 0f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
        };

        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber,
            InstallmentQuantity = installmentQuantity,
            IdAccount = newAccount.Id
        };
        var managerAccount = new AccountManager(_context);

        var paymentNetValue = payment.GrossValue - 0.9f;
        

        Mock<IAccountManager> test = new Mock<IAccountManager>();
        
        test.Setup(x => x.getByAccountNumberAsync(newAccount.Id)).ReturnsAsync(newAccount);
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

        var auxInstallmentNetValue = (result.payment.NetValue / (float)payment.InstallmentQuantity) - result.payment.FlatRate;

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
                    p => Assert.Equal((payment.GrossValue / (float)payment.InstallmentQuantity)
                        , p.InstallmentGrossValue));
        Assert.All(result.payment.Installments, 
                    p => Assert.NotNull(p.InstallmentNumber));
    }

    [Theory]
    [InlineData(5000f, "5999654785698745", 1)]
    [InlineData(5000f, "5999654785698785", 1)]
    [InlineData(5000f, "5999654775698745", 1)]
    public async Task ShouldConvertToReprovedAsync(float grossValue, string cardNumber, int installmentQuantity)
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

        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber,
            InstallmentQuantity = installmentQuantity,
            IdAccount = newAccount.Id
        };

        var managerAccount = new TransactionsManager(_context, _manager);

        Mock<IAccountManager> test = new Mock<IAccountManager>();
        
        test.Setup(x => x.getByAccountNumberAsync(newAccount.Id)).ReturnsAsync(newAccount);
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

}
