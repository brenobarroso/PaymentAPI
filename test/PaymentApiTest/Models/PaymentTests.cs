using api.Models;

namespace PaymentApiTest.Models;

public class PaymentTests
{
    [Theory]
    [InlineData(1f, "0000000000000000")]
    [InlineData(100f, "0000000000000000")]
    [InlineData(1.5f, "0000000000000000")]
    public void PaymentIsValid(int grossValue, string cardNumber)
    {
        // Arrange
        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber
        };

        // Act
        var errors = TestModelHelper.Validate(payment);

        // Assert
        Assert.Equal(0, errors.Count);
    }

    [Theory]
    [InlineData(0f, "0000000000000000")]
    [InlineData(-100f, "0000000000000000")]
    [InlineData(null, "0000000000000000")]
    [InlineData(100f, "00000 0000000000")]
    public void PaymentIsNotValid(int grossValue, string cardNumber)
    {
        // Arrange
        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber
        };

        // Act
        var errors = TestModelHelper.Validate(payment);

        // Assert
        Assert.NotNull(errors);
    }
}