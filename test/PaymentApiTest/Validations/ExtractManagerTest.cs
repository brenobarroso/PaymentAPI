using api.Interfaces;
using api.Managers;
using api.Models;
using api.Models.Extract;
using api.Models.Movements;
using api.Models.Withdraws;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentApiTest.Validations;

public class ExtractManagerTest
{
    private PaymentDbContext _context;
    private IAccountManager _manager;

    public ExtractManagerTest()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDbContext(options);

        _context.Database.EnsureCreated();
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(1, 0, 4)]
    [InlineData(1, 0, 5)]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 3)]
    [InlineData(1, 3, 2)]
    [InlineData(1, 4, 1)]
    [InlineData(1, 5, 0)]
    public async Task ShouldListByAccountIdAllTypes(int accountId, int startIndex, int extractCount)
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
        var viewModel = new ExtractViewModel
        {
            Index = startIndex,
            Length = extractCount,
            JustIn = false,
            JustOut = false,
            StartDate = null,
            EndDate = null
        };
        var account = new Account
        {
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

        var movement1 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement
        {
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
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(viewModel, accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(extractCount, result.Itens.Count);
        Assert.All(result.Itens,
                    p => Assert.Equal("String genérica.", p.ToString()));
        Assert.Equal(startIndex, result.Index);
        Assert.Equal(5, result.Count);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(1, 0, 4)]
    [InlineData(1, 0, 5)]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 3)]
    [InlineData(1, 3, 2)]
    [InlineData(1, 4, 1)]
    [InlineData(1, 5, 0)]
    public async Task ShouldListByAccountIdJustPayments(int accountId, int startIndex, int extractCount)
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
        var viewModel = new ExtractViewModel
        {
            Index = startIndex,
            Length = extractCount,
            JustIn = true,
            JustOut = false,
            StartDate = null,
            EndDate = null
        };
        var account = new Account
        {
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

        var movement1 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
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
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(viewModel, accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(extractCount, result.Itens.Count);
        Assert.All(result.Itens,
                    p => Assert.Equal("String genérica - entrada.", p.ToString()));
        Assert.Equal(startIndex, result.Index);
        Assert.Equal(5, result.Count);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(1, 0, 4)]
    [InlineData(1, 0, 5)]
    [InlineData(1, 1, 1)]
    [InlineData(1, 2, 3)]
    [InlineData(1, 3, 2)]
    [InlineData(1, 4, 1)]
    [InlineData(1, 5, 0)]
    public async Task ShouldListByAccountIdJustWithdraws(int accountId, int startIndex, int extractCount)
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
        var viewModel = new ExtractViewModel
        {
            Index = startIndex,
            Length = extractCount,
            JustIn = false,
            JustOut = true,
            StartDate = null,
            EndDate = null
        };
        var account = new Account
        {
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

        var movement1 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
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
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(viewModel, accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(extractCount, result.Itens.Count);
        Assert.All(result.Itens,
                    p => Assert.Equal("String genérica - saída.", p.ToString()));
        Assert.Equal(startIndex, result.Index);
        Assert.Equal(5, result.Count);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(1, 0, 4)]
    public async Task ShouldListByAccountIdByStartDate(int accountId, int startIndex, int extractCount)
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
        var viewModel = new ExtractViewModel
        {
            Index = startIndex,
            Length = extractCount,
            JustIn = false,
            JustOut = false,
            StartDate = DateTime.UtcNow,
            EndDate = null
        };
        var account = new Account
        {
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

        var dateString = "5/1/2008 8:30:52 AM";
        var parsedDate = DateTime.Parse(dateString);

        var movement1 = new Movement
        {
            Date = parsedDate,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement
        {
            Date = DateTime.UtcNow.AddDays(3),
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
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
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(viewModel, accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(extractCount, result.Itens.Count);
        Assert.All(result.Itens,
                    p => Assert.Equal("String genérica - saída.", p.ToString()));
        Assert.Equal(startIndex, result.Index);
        Assert.Equal(extractCount, result.Itens.Count);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(1, 0, 4)]
    public async Task ShouldListByAccountIdByEndDate(int accountId, int startIndex, int extractCount)
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
        var viewModel = new ExtractViewModel
        {
            Index = startIndex,
            Length = extractCount,
            JustIn = false,
            JustOut = false,
            StartDate = null,
            EndDate = DateTime.UtcNow.AddDays(2)
        };
        var account = new Account
        {
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

        var dateString = "12/30/2022 8:30:52 AM";
        var parsedDate = DateTime.Parse(dateString);

        var movement1 = new Movement
        {
            Date = parsedDate,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
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
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(viewModel, accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(extractCount, result.Itens.Count);
        Assert.All(result.Itens,
                    p => Assert.Equal("String genérica - saída.", p.ToString()));
        Assert.Equal(startIndex, result.Index);
        Assert.Equal(extractCount, result.Itens.Count);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(1, 0, 4)]
    [InlineData(1, 0, 5)]
    public async Task ShouldListByAccountIdByStartDateAndEndDate(int accountId, int startIndex, int extractCount)
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
        var viewModel = new ExtractViewModel
        {
            Index = startIndex,
            Length = extractCount,
            JustIn = false,
            JustOut = false,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };
        var account = new Account
        {
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


        var movement1 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement
        {
            Date = DateTime.UtcNow.AddDays(3),
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - saída.",
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
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(viewModel, accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(extractCount, result.Itens.Count);
        Assert.All(result.Itens,
                    p => Assert.Equal("String genérica - saída.", p.ToString()));
        Assert.Equal(startIndex, result.Index);
        Assert.Equal(extractCount, result.Itens.Count);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(1, 0, 4)]
    [InlineData(1, 0, 5)]
    public async Task ShouldListByAccountIdByAllFilters(int accountId, int startIndex, int extractCount)
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
        var viewModel = new ExtractViewModel
        {
            Index = startIndex,
            Length = extractCount,
            JustIn = true,
            JustOut = false,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };
        var account = new Account
        {
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


        var movement1 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement2 = new Movement
        {
            Date = DateTime.UtcNow.AddDays(3),
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement3 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica.",
            Withdraw = null,
            Payment = new Payment(),
            Account = new Account(),
            AccountId = 2
        };

        var movement4 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement5 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
            Withdraw = null,
            Payment = new Payment(),
            Account = account,
            AccountId = account.Id
        };

        var movement6 = new Movement
        {
            Date = DateTime.UtcNow,
            Value = (decimal)1500m,
            Comments = "String genérica - entrada.",
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
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(viewModel, accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(extractCount, result.Itens.Count);
        Assert.All(result.Itens,
                    p => Assert.Equal("String genérica - entrada.", p.ToString()));
        Assert.Equal(startIndex, result.Index);
        Assert.Equal(extractCount, result.Itens.Count);
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
