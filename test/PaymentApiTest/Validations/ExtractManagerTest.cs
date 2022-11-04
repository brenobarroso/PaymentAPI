using api.Interfaces;
using api.Managers;
using api.Models;
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

    [Fact]
    public async Task ShouldListByAccountId()
    {
        // Arrange
        var manager = new ExtractManager(_context, _manager);
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

        account.Movements.Add(movement1);
        account.Movements.Add(movement2);

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await manager.GetByAccountIdAsync(account.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("String genérica.", result.First().ToString());
        Assert.Equal("String genérica.", result.Last().ToString());

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
