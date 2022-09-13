using System;
using System.Linq;
using System.Threading;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentAPI;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using PaymentAPI.Models;
using PaymentAPI.Validations;
using Xunit;


namespace PaymentApiTest.Controllers;

public class PaymentControllerTest
{
    protected readonly PaymentDbContext _context;
    public PaymentControllerTest()
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

        var payment = new Payment
        {
            GrossValue = 5000f,
            CardNumber = "1023654785698745"
        };

        var payment2 = new Payment
        {
            GrossValue = 5000f,
            CardNumber = "1023654787498745"
        };

        var payment3 = new Payment
        {
            GrossValue = 5000f,
            CardNumber = "1023054785698025"
        };

        var payment4 = new Payment
        {
            GrossValue = 5000f,
            CardNumber = "102365478569825"
        };

        var payment5 = new Payment
        {
            GrossValue = 5000f,
            CardNumber = "1023654485698025"
        };

        var paymentList = new List<Payment>();

        paymentList.Add(payment);
        paymentList.Add(payment2);
        paymentList.Add(payment3);
        paymentList.Add(payment4);
        paymentList.Add(payment5);

        var manager = new Mock<ITransactionsManager>();
        manager.Setup(x => x.getAllAsync()).ReturnsAsync(paymentList);

        var paymentAPIController = new PaymentController(manager.Object);


        //Act

        var result = (OkObjectResult)await paymentAPIController.Get();

        // Assert

        Assert.Equal(200, result.StatusCode);

    }

    [Fact]
    public async Task ShouldListAnExistentId()
    {
        // Arrange
        var id = new Random().Next();
        var payment = new Payment
        {
            GrossValue = 5000f,
            CardNumber = "1023654785698745"
        };

        var manager = new Mock<ITransactionsManager>();
        manager.Setup(x => x.getByIdAsync(id)).ReturnsAsync(payment);

        var paymentController = new PaymentController(manager.Object);

        //Act
        var result = (OkObjectResult)await paymentController.GetById(id);

        // Assert

        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ShouldNotListANotExistentId()
    {
        // Arrange
        var id = new Random().Next();

        var manager = new Mock<ITransactionsManager>();
        manager.Setup(x => x.getByIdAsync(id)).ReturnsAsync((Payment)null);

        var paymentController = new PaymentController(manager.Object);

        //Act
        var result = (NotFoundResult)await paymentController.GetById(id);

        // Assert

        Assert.Equal(404, result.StatusCode);
    }

    [Theory]
    [InlineData(5000f, "5630125478536540")]
    [InlineData(5000f, "0000599978536540")]
    public async Task TransactionShouldBeApproved(int grossValue, string cardNumber)
    {
        // Arrange
        var paymentController = new PaymentController(new TransactionsManager(_context));

        var viewModel = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber
        };

        // Act

        var result = (OkResult)await paymentController.Transaction(viewModel);

        var payments = await _context.Payments.ToListAsync();

        Assert.Equal(200, result.StatusCode);
        Assert.Single(payments);
        Assert.Equal(viewModel.GrossValue, payments.First().GrossValue);
        Assert.Equal((grossValue - payments.First().FlatRate), payments.First().NetValue);
        Assert.Equal(DateTime.UtcNow.Date, payments.First().TransationDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, payments.First().ApprovalDate.Value.Date);
        Assert.NotNull(payments.First().FlatRate);
        Assert.NotNull(payments.First().ApprovalDate);
        Assert.Null(payments.First().DisapprovalDate);
        Assert.Equal(true, payments.First().Confirmation);
        Assert.Equal(4, payments.First().CardNumber.Length);
    }

    [Theory]
    [InlineData(5000f, "5999125478536540")]
    public async Task TransactionShouldBeReproved(int grossValue, string cardNumber)
    {
        // Arrange

        var paymentController = new PaymentController(new TransactionsManager(_context));

        var viewModel = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber
        };

        // Act

        var result = (OkResult)await paymentController.Transaction(viewModel);

        var payments = await _context.Payments.ToListAsync();


        // Assert

        Assert.Equal(200, result.StatusCode);
        Assert.Single(payments);
        Assert.Equal(viewModel.GrossValue, payments.First().GrossValue);
        Assert.Null(payments.First().NetValue);
        Assert.Equal(DateTime.UtcNow.Date, payments.First().TransationDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, payments.First().DisapprovalDate.Value.Date);
        Assert.Null(payments.First().ApprovalDate);
        Assert.NotNull(payments.First().DisapprovalDate);
        Assert.Equal(false, payments.First().Confirmation);
        Assert.NotNull(payments.First().FlatRate);
        Assert.Equal(4, payments.First().CardNumber.Length);

    }
}
