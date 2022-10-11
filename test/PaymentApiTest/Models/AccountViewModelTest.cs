using api.Models;
namespace PaymentApiTest.Models;

public class AccountViewModelTest
{
    [Theory]
    [InlineData("12345678901", "0001", "Breno Barroso")]
    [InlineData("12345678910", "0001", "Breno Santos Barroso")]
    [InlineData("12345678900", "0001", "Breno Barroso Santos")]
    public void AccountIsValid(string cpf, string agency, string holderName)
    {
        // Arrange
        var viewModel = new AccountViewModel
        {
            CPF = cpf,
            Agency = agency,
            HolderName = holderName
        };

        // Act
        var errors = TestModelHelper.Validate(viewModel);

        // Assert
        Assert.Equal(0, errors.Count);
    }

    [Theory]
    [InlineData("1234567890", "0001", "Breno Barroso")]
    [InlineData("123456789101", "0001", "Breno Santos Barroso")]
    [InlineData("123456789 0", "0001", "Breno Barroso Santos")]
    [InlineData("123456789a0", "0001", "Breno Barroso Santos")]
    [InlineData("123456789!0", "0001", "Breno Barroso Santos")]
    [InlineData("12345678970", "0001", "Br")]
    [InlineData("12345678944", "0001", "qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq")]
    public void AccountIsNotValid(string cpf, string agency, string holderName)
    {
        // Arrange
        var viewModel = new AccountViewModel
        {
            CPF = cpf,
            Agency = agency,
            HolderName = holderName
        };

        // Act
        var errors = TestModelHelper.Validate(viewModel);

        // Assert
        Assert.NotNull(errors);
        Assert.NotEmpty(errors);
    }
}
