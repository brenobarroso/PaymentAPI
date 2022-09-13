using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

using System;
using api.Models;

namespace PaymentAPI.Validations;

public class TransactionsManager
{

    public static (Payment payment, bool sucess) Validation(PaymentViewModel viewModel)
    {
        if (viewModel.CardNumber.IndexOf("5999") == 0)
        {
            string fourLastDigitsOfCardReproved = viewModel.CardNumber.Substring(viewModel.CardNumber.Length - 4);

            var reprovedTransaction = new Payment
            {
                TransationDate = DateTime.UtcNow,
                ApprovalDate = null,
                DisapprovalDate = DateTime.UtcNow,
                Confirmation = false,
                GrossValue = viewModel.GrossValue,
                NetValue = null,
                CardNumber = fourLastDigitsOfCardReproved
            };
            return (reprovedTransaction, false);
        }
        
        string fourLastDigitsOfCardApproved = viewModel.CardNumber.Substring(viewModel.CardNumber.Length - 4);

        var approvedTransation = new Payment
        {
            TransationDate = DateTime.UtcNow,
            ApprovalDate = DateTime.UtcNow,
            DisapprovalDate = null,
            Confirmation = true,
            GrossValue = viewModel.GrossValue,
            CardNumber = fourLastDigitsOfCardApproved
        };
        approvedTransation.NetValue = approvedTransation.GrossValue - approvedTransation.FlatRate;
        
        return (approvedTransation, true);
    }
}