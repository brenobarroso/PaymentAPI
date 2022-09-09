using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;
using System;

namespace api.Controllers;

public class PaymentValidations
{

    public (Payment, bool) Validations(Payment payment)
    {
        if (payment.CardNumber.Length != 16)
            return (payment, false);

        if (!(payment.CardNumber.All(char.IsDigit)))
            return (payment, false);


        if (payment.CardNumber.IndexOf("5999") == 0)
        {
            string fourLastDigitsOfCardReproved = payment.CardNumber.Substring(payment.CardNumber.Length - 4);

            var reprovedTransaction = new Payment
            {
                TransationDate = DateTime.UtcNow,
                ApprovalDate = null,
                DisapprovalDate = DateTime.UtcNow,
                Confirmation = false,
                GrossValue = payment.GrossValue,
                NetValue = payment.NetValue,
                FlatRate = payment.FlatRate,
                CardNumber = fourLastDigitsOfCardReproved
            };
            return (reprovedTransaction, true);
        }
        else
        {
            string fourLastDigitsOfCardApproved = payment.CardNumber.Substring(payment.CardNumber.Length - 4);

            var approvedTransation = new Payment
            {
                TransationDate = DateTime.UtcNow,
                ApprovalDate = DateTime.UtcNow,
                DisapprovalDate = null,
                Confirmation = true,
                FlatRate = payment.FlatRate,
                GrossValue = payment.GrossValue,
                NetValue = payment.GrossValue - payment.FlatRate,
                CardNumber = fourLastDigitsOfCardApproved
            };
            return (approvedTransation, true);
        }
    }
}

