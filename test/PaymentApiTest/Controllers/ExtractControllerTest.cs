using api.Controllers;
using api.Interfaces;
using api.Models;
using api.Models.Extract;
using api.Models.Movements;
using api.Models.Withdraws;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentApiTest.Controllers;

public class ExtractControllerTest
{
    protected readonly PaymentDbContext _context;

    public ExtractControllerTest()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDbContext(options);

        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task ShouldListByAccountId()
    {
        // Arrange
        var mockedViewModel = new ExtractViewModel{
            Index = 0,
            Length = 5
        };
        
        var id = new Random().Next();
                var account = new Account{
            Id = 1,
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = CreateRandomStringBySize(5),
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(7),
            Withdraws = new List<Withdraw>(),
            Payments = new List<Payment>(),
            Movements = new List<Movement>()
        };
        
        var movement1 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        account.Movements.Add(movement1);
        account.Movements.Add(movement2);
        account.Movements.Add(movement4);
        account.Movements.Add(movement5);
        account.Movements.Add(movement6);

        _context.Accounts.Add(account);
        var movements = new List<string>{
            "String genérica.",
            "String genérica.",
            "String genérica.",
            "String genérica.",
            "String genérica."
        };
        await _context.SaveChangesAsync();

        var manager = new Mock<IExtractManager>();
        manager.Setup(x => x.GetByAccountIdAsync(id, 0, 5)).ReturnsAsync(movements);

        var extractController = new ExtractController(manager.Object);

        // Act
        var result = (OkObjectResult)await extractController.GetByIdAsync(id, mockedViewModel);

        // Assert
        Assert.Equal(200, result.StatusCode);

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