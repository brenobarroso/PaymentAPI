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
}
