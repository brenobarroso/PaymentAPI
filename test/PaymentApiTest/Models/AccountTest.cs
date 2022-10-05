using api.Models;

namespace PaymentApiTest.Models;

public class AccountTest
{
    [Theory]
    [InlineData(1, "12345678901", "0001", "Breno Barroso", 1000f, true)]
    [InlineData(2, "12345678910", "0001", "Marcus Melo", 1000f, true)]
    [InlineData(3, "12345678912", "0001", "Manoel Moreira", 1000f, true)]
    public void AccountIsValid(int id, string cpf, string agency,
                    string holderName, float balance, bool isActive)
    {
        // Arrange
        var account = new Account
        {
            Id = id,
            CPF = cpf,
            Agency = agency,
            HolderName = holderName,
            Balance = balance,
            IsActive = isActive
        };

        // Act
        var errors = TestModelHelper.Validate(account);

        // Assert
        Assert.Equal(0, errors.Count);
    }

    [Theory]
    [InlineData(1, "1234567801", "0001", "Breno Barroso", 1000f, true)]
    [InlineData(2, "12345678 10", "0001", "Marcus Melo", 1000f, true)]
    [InlineData(3, "12345678h12", "0001", "Manoel Moreira", 1000f, true)]
    [InlineData(3, "12345678912", "0001", "Ma", 1000f, true)]
    [InlineData(3, "123456789120", "0001", "Pedro Munhoz", 1000f, true)]
    [InlineData(3, "12345678912", "0001", "qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq",
         1000f, true)]
    public void AccountIsNotValid(int id, string cpf, string agency,
                    string holderName, float balance, bool isActive)
    {
        // Arrange
        var account = new Account
        {
            Id = id,
            CPF = cpf,
            Agency = agency,
            HolderName = holderName,
            Balance = balance,
            IsActive = isActive
        };

        // Act
        var errors = TestModelHelper.Validate(account);

        // Assert
        Assert.NotNull(errors);
    }
}
