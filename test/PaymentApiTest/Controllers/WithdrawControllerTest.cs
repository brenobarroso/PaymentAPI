using api.Controllers;
using api.Interfaces;
using api.Models;
using api.Models.Withdraw;
using api.Models.Withdraws;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentApiTest.Controllers;

public class WithdrawControllerTest
{
    protected readonly PaymentDbContext _context;
    private readonly IConvertWithdraw _convertWithdraw;

    public WithdrawControllerTest()
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
        var withdraw1 = new WithdrawResult{
            Value = 150m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw2 = new WithdrawResult{
            Value = 100m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw3 = new WithdrawResult{
            Value = 150m,
            Date = DateTime.UtcNow,
            ApprovalDate = null,
            DisapprovalDate = DateTime.UtcNow,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdrawResultList = new List<WithdrawResult>();

        withdrawResultList.Add(withdraw1);
        withdrawResultList.Add(withdraw2);
        withdrawResultList.Add(withdraw3);

        await _context.SaveChangesAsync();

        var manager = new Mock<IWithdrawManager>();
        manager.Setup(x => x.GetAllWithdrawsAsync()).ReturnsAsync(withdrawResultList);

        var withdrawController = new WithdrawController(_convertWithdraw ,manager.Object);

        // Act
        var result = (OkObjectResult) await withdrawController.Get();

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ShouldListById()
    {
        // Arrange
        var withdraw1 = new Withdraw{
            AccountId = 1,
            Value = 150m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw2 = new Withdraw{
            AccountId = 2,
            Value = 100m,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Comments = "Saque genérico.",
            Type = 1
        };

        var withdraw3 = new Withdraw{
            AccountId = 3,
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

        var withdrawResultList = new List<WithdrawResult>();
        var convertToResult = new WithdrawResult()
        {
            Value = withdraw1.Value,
            Date = withdraw1.Date,
            ApprovalDate = withdraw1.ApprovalDate,
            DisapprovalDate = withdraw1.DisapprovalDate,
            Comments = withdraw1.Comments,
            Type = withdraw1.Type
        };
        
        withdrawResultList.Add(convertToResult);
        await _context.SaveChangesAsync();

        var manager = new Mock<IWithdrawManager>();
        manager.Setup(x => x.GetWithdrawsByIdAsync(1)).ReturnsAsync(withdrawResultList);

        var withdrawController = new WithdrawController(_convertWithdraw ,manager.Object);

        // Act
        var result = (OkObjectResult)await withdrawController.GetById(withdraw1.Id);

        // Assert
        Assert.Equal(200, result.StatusCode);

    }

    [Fact]
    public async Task ShoudNotListById()
    {
        // Arrange
        Random random = new Random();
        int randomValue = random.Next();
        var manager = new Mock<IWithdrawManager>();
        manager.Setup(x => x.GetWithdrawsByIdAsync(randomValue)).ReturnsAsync(new List<WithdrawResult>());

        var withdrawController = new WithdrawController(_convertWithdraw ,manager.Object);

        // Act
        var result = (NotFoundObjectResult)await withdrawController.GetById(randomValue);

        // Assert
        Assert.Equal(404, result.StatusCode);
    }

    [Theory]
    [InlineData("0000001", 1)]
    public async Task ShouldMakeAWithdraw(string accountValue, decimal value)
    {
        // Arrange

        var viewModel = new WithdrawViewModel
        {
            AccountNumber = accountValue,
            Value = value
        };

       var newAccount1 = new Account
        {
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = "0000001",
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var mockedWithdraw = new Withdraw
        {
            Account = newAccount1,
            AccountId = newAccount1.Id,
            Id = 1,
            Value = viewModel.Value,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Type = 1,
            Comments = "Saque genérico."
        };

        var withdrawApprovedResult = new WithdrawResult{
                Id = mockedWithdraw.Id,
                Value = mockedWithdraw.Value,
                Date = mockedWithdraw.Date,
                ApprovalDate = mockedWithdraw.ApprovalDate,
                DisapprovalDate = mockedWithdraw.DisapprovalDate,
                Comments = mockedWithdraw.Comments,
                Type = mockedWithdraw.Type
        };
        
        _context.Accounts.Add(newAccount1);
        await _context.SaveChangesAsync();


        var manager = new Mock<IWithdrawManager>();
        manager.Setup(x => x.MakeWithdraw(accountValue, value)).ReturnsAsync((withdrawApprovedResult, true));

        var withdrawController = new WithdrawController(_convertWithdraw ,manager.Object);


        // Act
        var result = (OkObjectResult)await withdrawController.MakeAWithdraw(viewModel);

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Theory]
    [InlineData("0000001", 10000)]
    public async Task ShouldNotMakeAWithdraw(string accountValue, decimal value)
    {
        // Arrange

        var viewModel = new WithdrawViewModel
        {
            AccountNumber = accountValue,
            Value = value
        };

       var newAccount1 = new Account
        {
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = "0000001",
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        var mockedWithdraw = new Withdraw
        {
            Account = newAccount1,
            AccountId = newAccount1.Id,
            Id = 1,
            Value = viewModel.Value,
            Date = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Type = 1,
            Comments = "Saque genérico."
        };

        var withdrawReprovedResult = new WithdrawResult{
                Id = mockedWithdraw.Id,
                Value = mockedWithdraw.Value,
                Date = mockedWithdraw.Date,
                ApprovalDate = mockedWithdraw.ApprovalDate,
                DisapprovalDate = mockedWithdraw.DisapprovalDate,
                Comments = mockedWithdraw.Comments,
                Type = mockedWithdraw.Type
        };
        
        _context.Accounts.Add(newAccount1);
        await _context.SaveChangesAsync();


        var manager = new Mock<IWithdrawManager>();
        manager.Setup(x => x.MakeWithdraw(accountValue, value)).ReturnsAsync((withdrawReprovedResult, false));

        var withdrawController = new WithdrawController(_convertWithdraw ,manager.Object);


        // Act
        var result = (UnprocessableEntityObjectResult)await withdrawController.MakeAWithdraw(viewModel);

        // Assert
        Assert.Equal(422, result.StatusCode);
    }

    [Theory]
    [InlineData("0000001", 10000)]
    [InlineData("0000002", 1500)]
    public async Task ShouldNotMakeAWithdrawByInvalidInput(string accountValue, decimal value)
    {
        // Arrange

        var viewModel = new WithdrawViewModel
        {
            AccountNumber = accountValue,
            Value = value
        };

       var newAccount1 = new Account
        {
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = "0000001",
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>()
        };

        _context.Accounts.Add(newAccount1);
        await _context.SaveChangesAsync();


        var manager = new Mock<IWithdrawManager>();
        manager.Setup(x => x.MakeWithdraw(accountValue, value)).ReturnsAsync(((WithdrawResult)null, false));

        var withdrawController = new WithdrawController(_convertWithdraw ,manager.Object);


        // Act
        var result = (BadRequestResult)await withdrawController.MakeAWithdraw(viewModel);

        // Assert
        Assert.Equal(400, result.StatusCode);
    }
}
