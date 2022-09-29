using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Validations;

namespace PaymentApiTest.Validations;

public class TransactionsManagerTest
{
    protected readonly PaymentDbContext _context;
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

        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber,
            InstallmentQuantity = installmentQuantity
        };

        var manager = new TransactionsManager(_context);

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

        var auxInstallmentNetValue = (payment.GrossValue / (float)payment.InstallmentQuantity) - result.payment.FlatRate;

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
    [InlineData(5000f, "5999654785698745")]
    [InlineData(5000f, "5999654785698785")]
    [InlineData(5000f, "5999654775698745")]
    public async Task ShouldConvertToReprovedAsync(float grossValue, string cardNumber)
    {
        // Arrange

        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber
        };

        var manager = new TransactionsManager(_context);

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
