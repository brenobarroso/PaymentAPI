using System;
using System.Linq;
using System.Threading;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using PaymentAPI.Models;
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

        var paymentAPIController = new PaymentController(_context);


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

        await _context.AddAsync(payment);
        await _context.AddAsync(payment2);
        await _context.AddAsync(payment3);
        await _context.AddAsync(payment4);
        await _context.AddAsync(payment5);


        //Act

        var result = (OkObjectResult)await paymentAPIController.Get();

        // Assert

        Assert.Equal(200, result.StatusCode);

    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task ShouldListAnExistentId(int id)
    {
        // Arrange

        var paymentController = new PaymentController(_context);

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

        await _context.AddAsync(payment);
        await _context.AddAsync(payment2);
        await _context.AddAsync(payment3);
        await _context.AddAsync(payment4);
        await _context.AddAsync(payment5);

        //Act

        var result = (OkObjectResult)await paymentController.GetById(id);

        // Assert

        Assert.Equal(200, result.StatusCode);
    }

    [Theory]
    [InlineData(500)]
    [InlineData(50)]
    [InlineData(90)]
    [InlineData(6)]
    [InlineData(30)]
    [InlineData(10)]
    public async Task ShouldNotListANotExistentId(int id)
    {
        // Arrange

        var paymentController = new PaymentController(_context);

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

        await _context.AddAsync(payment);
        await _context.AddAsync(payment2);
        await _context.AddAsync(payment3);
        await _context.AddAsync(payment4);
        await _context.AddAsync(payment5);

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
        var paymentController = new PaymentController(_context);

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

        var paymentController = new PaymentController(_context);

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
