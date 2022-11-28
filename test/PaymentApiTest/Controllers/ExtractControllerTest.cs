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
    public async Task ShouldListByAccountIdJustPayments()
    {
        // Arrange
        var mockedViewModel = new ExtractViewModel{
            Index = 0,
            Length = 5,
            JustIn = false,
            JustOut = false,
            StartDate = null,
            EndDate = null
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
            Comments = "String genérica entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica entrada.",
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
            Comments = "String genérica entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica entrada.",
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
            "String genérica entrada.",
            "String genérica entrada.",
            "String genérica entrada.",
            "String genérica entrada.",
            "String genérica entrada.",
        };
        var mockedResult = new ExtractResult{
            Index = mockedViewModel.Index,
            Length = mockedViewModel.Length,
            Itens = movements,
            Count = 5

        };
        await _context.SaveChangesAsync();

        var manager = new Mock<IExtractManager>();
        manager.Setup(x => x.GetByAccountIdAsync(mockedViewModel, id)).ReturnsAsync(mockedResult);

        var extractController = new ExtractController(manager.Object);

        // Act
        var result = (OkObjectResult)await extractController.GetByIdAsync(id, mockedViewModel);

        // Assert
        Assert.Equal(200, result.StatusCode);

    }

    [Fact]
    public async Task ShouldListByAccountIdJustWithdraws()
    {
        // Arrange
        var mockedViewModel = new ExtractViewModel{
            Index = 0,
            Length = 5,
            JustIn = true,
            JustOut = false,
            StartDate = null,
            EndDate = null
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
            Comments = "String genérica saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica saída.",
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
            Comments = "String genérica saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica saída.",
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
            "String genérica saída.",
            "String genérica saída.",
            "String genérica saída.",
            "String genérica saída.",
            "String genérica saída.",
        };
        var mockedResult = new ExtractResult{
            Index = mockedViewModel.Index,
            Length = mockedViewModel.Length,
            Itens = movements,
            Count = 5

        };
        await _context.SaveChangesAsync();

        var manager = new Mock<IExtractManager>();
        manager.Setup(x => x.GetByAccountIdAsync(mockedViewModel, id)).ReturnsAsync(mockedResult);

        var extractController = new ExtractController(manager.Object);

        // Act
        var result = (OkObjectResult)await extractController.GetByIdAsync(id, mockedViewModel);

        // Assert
        Assert.Equal(200, result.StatusCode);

    }

    [Fact]
    public async Task ShouldListByAccountIdJustAll()
    {
        // Arrange
        var mockedViewModel = new ExtractViewModel{
            Index = 0,
            Length = 5,
            JustIn = false,
            JustOut = false,
            StartDate = null,
            EndDate = null
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
            Comments = "String genérica saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica entrada.",
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
            Comments = "String genérica saida.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement{
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica entrada.",
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
            "String genérica saída.",
            "String genérica entrada.",
            "String genérica saída.",
            "String genérica saída.",
            "String genérica entrada.",
        };
        var mockedResult = new ExtractResult{
            Index = mockedViewModel.Index,
            Length = mockedViewModel.Length,
            Itens = movements,
            Count = 5

        };
        await _context.SaveChangesAsync();

        var manager = new Mock<IExtractManager>();
        manager.Setup(x => x.GetByAccountIdAsync(mockedViewModel, id)).ReturnsAsync(mockedResult);

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