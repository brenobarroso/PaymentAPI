using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

using System;
using api.Models;

namespace PaymentAPI.Validations;

public interface ITransactionsManager
{
    Task<(Payment payment, bool sucess)> CreatAsync(PaymentViewModel viewModel);
    Task<List<Payment>> getAllAsync();
    Task<Payment?> getByIdAsync(int id);
}

public class TransactionsManager : ITransactionsManager
{
    private readonly PaymentDbContext _context;
    public TransactionsManager(PaymentDbContext context) => _context = context;

    public async Task<List<Payment>> getAllAsync()
    {
        var result = await _context.Payments.ToListAsync();
        return result;
    }

    public async Task<Payment?> getByIdAsync(int id)
    {
        var result = await _context.Payments.FindAsync(id);
        if (result == null)
            return null;

        return result;
    }

    public async Task<(Payment payment, bool sucess)> CreatAsync(PaymentViewModel viewModel)
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

            await _context.Payments.AddAsync(reprovedTransaction);
            await _context.SaveChangesAsync();

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

        await _context.Payments.AddAsync(approvedTransation);
        await _context.SaveChangesAsync();

        return (approvedTransation, true);
    }
}