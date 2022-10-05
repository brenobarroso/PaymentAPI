using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentApiTest.Controllers;

public class AccountControllerTest
{
    protected readonly PaymentDbContext _context;
    public AccountControllerTest()
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
        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount2 = new Account{
            CPF = "12375678901",
            Balance = 5000f,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = true
        };

        var newAccount3 = new Account{
            CPF = "12315678901",
            Balance = 5000f,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var accountList = new List<Account>();

        accountList.Add(newAccount1);
        accountList.Add(newAccount2);
        accountList.Add(newAccount3);

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.getAllAccountsAsync()).ReturnsAsync(accountList);

        var accountController = new AccountController(manager.Object);

        // Act
        var result = (OkObjectResult)await accountController.Get();

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ShouldListAnExistingAccountByAccountNumber()
    {
        // Arrange
        var id = new Random().Next();
        var newAccount1 = new Account{
            CPF = "12345678901",
            Balance = 5000f,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true
        };

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.getByAccountNumberAsync(id)).ReturnsAsync(newAccount1);

        var accountController = new AccountController(manager.Object);

        // Act
        var result = (OkObjectResult)await accountController.GetByIdAccount(id);

        // Assert
        Assert.Equal(200, result.StatusCode);
    }
}
