using api.Models.Withdraw;

namespace PaymentApiTest.Models;

public class WithdrawViewModelTest
{
    [Theory]
    [InlineData("0000001", 1000)]
    [InlineData("0000002", 1000)]
    [InlineData("0000003", 1000)]
    [InlineData("0000004", 1000)]
    [InlineData("0000005", 1000)]
    public void WithdrawIsValid(string accountNumber, decimal value)
    {
        // Arrange
        var viewModel = new WithdrawViewModel{
            AccountNumber = accountNumber,
            Value = value
        };

        // Act
        var errors = TestModelHelper.Validate(viewModel);

        // Assert
        Assert.Equal(0, errors.Count);
    }

    [Theory]
    [InlineData("00000 1", 0)]
    [InlineData("00000a2", 1000)]
    [InlineData("00!0003", 1000)]
    [InlineData("000!a 4", 1000)]
    [InlineData("0000005000", 1000)]
    public void WithdrawIsNotValid(string accountNumber, decimal value)
    {
        // Arrange
        var viewModel = new WithdrawViewModel{
            AccountNumber = accountNumber,
            Value = value
        };

        // Act
        var errors = TestModelHelper.Validate(viewModel);

        // Assert
        Assert.NotEqual(0, errors.Count);
    }
    
}
