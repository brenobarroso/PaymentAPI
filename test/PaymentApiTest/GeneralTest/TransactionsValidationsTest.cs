using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using PaymentAPI.Models;
using PaymentAPI.Validations;
using Xunit;

namespace PaymentApiTest.GeneralTest;

public class TransactionsValidationsTest
{
    [Theory]
    [InlineData(5000f, "1023654785698745")]
    [InlineData(5000f, "1023654787498745")]
    [InlineData(5000f, "1023054785698025")]
    [InlineData(5000f, "1023654485698025")]
    [InlineData(5000f, "1023654785098745")]
    [InlineData(5000f, "1023654787498745")]
    [InlineData(5000f, "1023054785697725")]
    [InlineData(5000f, "1027754485698025")]
    
    public void ShouldConvertToApproved(float grossValue, string cardNumber)
    {
        // Arrange

        var payment = new Payment
        {
            GrossValue = grossValue,
            CardNumber = cardNumber
        };

        // Act

        var result = TransactionsValidations.Validation(payment);

        // Assert

        Assert.Equal(true, result.sucess);
        Assert.Equal(4, result.payment.CardNumber.Length);
        Assert.Equal(grossValue, result.payment.GrossValue);
        Assert.Equal((grossValue - result.payment.FlatRate), result.payment.NetValue);
        Assert.Equal(DateTime.UtcNow.Date, result.payment.TransationDate.Date);
        Assert.Equal(DateTime.UtcNow.Date, result.payment.ApprovalDate.Value.Date);
        Assert.NotNull(result.payment.FlatRate);
        Assert.NotNull(result.payment.ApprovalDate);
        Assert.Null(result.payment.DisapprovalDate);
        Assert.Equal(true, result.payment.Confirmation);

    }

}
