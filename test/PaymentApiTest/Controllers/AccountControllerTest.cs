using System;
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
    private readonly IConvertWithdraw _convert;
    private readonly IWithdrawManager _manager;
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
        var newAccount1 = new Account
        {
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = "0000001",
            Payments = new List<Payment>()
        };

        var newAccount2 = new Account
        {
            CPF = "12375678901",
            Balance = 5000m,
            HolderName = "Breno Santos",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = "0000002",
            Payments = new List<Payment>()
        };

        var newAccount3 = new Account
        {
            CPF = "12315678901",
            Balance = 5000m,
            HolderName = "Breno Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = "0000003",
            Payments = new List<Payment>()
        };

        var accountList = new List<Account>();

        accountList.Add(newAccount1);
        accountList.Add(newAccount2);
        accountList.Add(newAccount3);

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.GetAllAccountsAsync()).ReturnsAsync(accountList);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (OkObjectResult)await accountController.Get();

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ShouldListAnExistingAccountByAccountNumber()
    {
        // Arrange
        var newAccount1 = new Account
        {
            CPF = CreateRandomStringBySize(11),
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            AccountNumber = CreateRandomStringBySize(11),
            Payments = new List<Payment>()
        };

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.GetByAccountNumberAsync(newAccount1.AccountNumber)).ReturnsAsync(newAccount1);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (OkObjectResult)await accountController.GetByAccountNumber(newAccount1.AccountNumber);

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ShouldNotListAnNotExistingAccountByAccountNumber()
    {
        // Arrange
        var accountNumber = CreateRandomStringBySize(7);

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.GetByAccountNumberAsync(accountNumber)).ReturnsAsync((Account)null);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (NotFoundResult)await accountController.GetByAccountNumber(accountNumber);

        // Assert
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task ShouldListAnExistingAccountByCPF()
    {
        // Arrange

        var newAccount1 = new Account
        {
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
        };

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.GetByCPFAsync(newAccount1.CPF)).ReturnsAsync(newAccount1);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (OkObjectResult)await accountController.GetByCPF(newAccount1.CPF);

        // Assert
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ShouldNotListAnNotExistingAccountByCPF()
    {
        // Arrange
        var randomCPF = CreateRandomStringBySize(11);
        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.GetByCPFAsync(randomCPF)).ReturnsAsync((Account)null);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (NotFoundResult)await accountController.GetByCPF("15423458741");

        // Assert
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task ShouldBeRegistredAnAccount()
    {
        // Arrange
        var mockedAccount = new AccountViewModel
        {
            CPF = "12345678901",
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
        };

        var newAccount = new Account
        {
            CPF = mockedAccount.CPF,
            HolderName = mockedAccount.HolderName,
            Agency = mockedAccount.Agency,
            IsActive = true,
            Payments = new List<Payment>()
        };

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.CreateAccount(mockedAccount)).ReturnsAsync(newAccount);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (OkObjectResult)await accountController.Register(mockedAccount);

        // Assert

        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task ShouldNotBeRegistredAnAccount()
    {
        // Arrange
        var mockedAccount = new AccountViewModel
        {
            CPF = "12345678 01",
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
        };

        var newAccount = new Account
        {
            CPF = mockedAccount.CPF,
            HolderName = mockedAccount.HolderName,
            Agency = mockedAccount.Agency,
            IsActive = true,
            Payments = new List<Payment>()
        };

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.CreateAccount(mockedAccount)).ReturnsAsync(newAccount);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (UnprocessableEntityResult)await accountController.Register(mockedAccount);

        // Assert

        Assert.Equal(422, result.StatusCode);
    }

    [Fact]
    public async Task ShouldBeInactivated()
    {
        // Arrange
        var id = new Random().Next();
        var newAccount1 = new Account
        {
            CPF = "12345678901",
            Balance = 5000m,
            HolderName = "Breno Santos Barroso",
            Agency = "00239-9",
            IsActive = true,
            Payments = new List<Payment>()
        };

        var manager = new Mock<IAccountManager>();
        manager.Setup(x => x.DeleteAccount(id)).ReturnsAsync(newAccount1);

        var accountController = new AccountController(manager.Object, _convert, _manager);

        // Act
        var result = (OkObjectResult)await accountController.MakeInactive(id);

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
