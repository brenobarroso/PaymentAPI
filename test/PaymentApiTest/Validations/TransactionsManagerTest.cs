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
    [InlineData(5000f, "1023654785698745")]
    [InlineData(5000f, "1023654787498745")]
    [InlineData(5000f, "1023054785698025")]
    [InlineData(5000f, "1023654485698025")]
    [InlineData(5000f, "1023654785098745")]
    [InlineData(5000f, "1023654787498745")]
    [InlineData(5000f, "1023054785697725")]
    [InlineData(5000f, "1027754485698025")]
    public async Task ShouldConvertToApproved(float grossValue, string cardNumber)
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
