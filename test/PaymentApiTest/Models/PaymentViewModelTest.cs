using api.Models;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Controllers;
using api.Models;
using PaymentAPI.Validations;
using PaymentAPI.Models;

namespace PaymentApiTest.Models;

public class PaymentViewModelTest
{
    [Theory]
    // [InlineData(0f, "0000000000000000")]
    // [InlineData(-100f, "0000000000000000")]
    // [InlineData(null, "0000000000000000")]
    [InlineData(1f, "0000")]
    [InlineData(1f, "00000000000000000")]
    [InlineData(1f, "000000000000000 0")]
    [InlineData(1f, "000000000000000a")]
    [InlineData(1f, "0000000000000!00")]
    [InlineData(1f, "01254703h!023654")]
    public void TransactionCannotBeMade(int grossValue, string cardNumber)
    {
        // Arrange

        var payment = new PaymentViewModel
        {
            GrossValue = grossValue,
            CardNumber = cardNumber
        };

        // Act

        var result = TransactionsManager.Validation(payment);

        // Assert

        //Assert.Null(()payment);
        
    }
}
