using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using PaymentAPI.Models;
using PaymentAPI.Validations;


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

    [Fact]
    public async Task TransactionShouldBeApproved()
    {
        // Arrange
        var viewModel = new PaymentViewModel();

        var manager = new Mock<ITransactionsManager>();

        var mockedTransation = new Payment
        {
            TransationDate = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Confirmation = true,
            GrossValue = new Random().NextSingle(),
            NetValue = new Random().NextSingle(),
            FlatRate = new Random().NextSingle(),
            CardNumber = new Random().Next(0, 9999).ToString().PadLeft(4, '0'),
        };

        mockedTransation.NetValue = mockedTransation.GrossValue - mockedTransation.FlatRate;

        manager.Setup(x => x.CreatAsync(viewModel)).ReturnsAsync((mockedTransation, true));

        var paymentController = new PaymentController(manager.Object);

        // Act

        var result = (OkObjectResult)await paymentController.Transaction(viewModel);

        // Assert
        var resultContent = (Payment)result.Value;

        Assert.Equal(200, result.StatusCode);
        Assert.Equal(mockedTransation.TransationDate, resultContent.TransationDate);
        Assert.Equal(mockedTransation.ApprovalDate, resultContent.ApprovalDate);
        Assert.Equal(mockedTransation.DisapprovalDate, resultContent.DisapprovalDate);
        Assert.Equal(mockedTransation.Confirmation, resultContent.Confirmation);
        Assert.Equal(mockedTransation.GrossValue, resultContent.GrossValue);
        Assert.Equal(mockedTransation.NetValue, resultContent.NetValue);
        Assert.Equal(mockedTransation.FlatRate, resultContent.FlatRate);
        Assert.Equal(mockedTransation.CardNumber, resultContent.CardNumber);
    }

    [Theory]
    [InlineData(5000f, "5999125478536540")]
    public async Task TransactionShouldBeReproved(int grossValue, string cardNumber)
    {
        // Arrange

        var viewModel = new PaymentViewModel();

        var manager = new Mock<ITransactionsManager>();

        var mockedTransation = new Payment
        {
            TransationDate = DateTime.UtcNow,
            ApprovalDate = null,
            DisapprovalDate = DateTime.UtcNow,
            Confirmation = false,
            GrossValue = new Random().NextSingle(),
            NetValue = new Random().NextSingle(),
            FlatRate = new Random().NextSingle(),
            CardNumber = new Random().Next(0, 9999).ToString().PadLeft(4, '0'),
        };

        mockedTransation.NetValue = mockedTransation.GrossValue - mockedTransation.FlatRate;

        manager.Setup(x => x.CreatAsync(viewModel)).ReturnsAsync((mockedTransation, true));

        var paymentController = new PaymentController(manager.Object);

        // Act

        var result = (OkObjectResult)await paymentController.Transaction(viewModel);
        var resultContent = (Payment)result.Value;


        // Assert

        Assert.Equal(200, result.StatusCode);
        Assert.Equal(mockedTransation.TransationDate, resultContent.TransationDate);
        Assert.Equal(mockedTransation.ApprovalDate, resultContent.ApprovalDate);
        Assert.Equal(mockedTransation.DisapprovalDate, resultContent.DisapprovalDate);
        Assert.Equal(mockedTransation.Confirmation, resultContent.Confirmation);
        Assert.Equal(mockedTransation.GrossValue, resultContent.GrossValue);
        Assert.Equal(mockedTransation.NetValue, resultContent.NetValue);
        Assert.Equal(mockedTransation.FlatRate, resultContent.FlatRate);
        Assert.Equal(mockedTransation.CardNumber, resultContent.CardNumber);


    }
}
