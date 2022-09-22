using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;
using api.Models;
using api.Interfaces;

namespace PaymentAPI.Validations;

public class TransactionsManager : ITransactionsManager
{
    private readonly PaymentDbContext _context;
    public TransactionsManager(PaymentDbContext context) => _context = context;

    public async Task<List<Payment>> getAllAsync()
    {
        var result = await _context.Payments.Include(x => x.Installments).ToListAsync();
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

        var newInstallment = new Installment
        {
            ReceiptDate = DateTime.UtcNow.AddDays(30),
            InstallmentNumber = 1,
            InstallmentGrossValue = viewModel.GrossValue / 1f,
            InstallmentNetValue = (float)(viewModel.GrossValue - approvedTransation.FlatRate),
        };

        var listOfInstallments = new List<Installment>();
        listOfInstallments.Add(newInstallment);

        approvedTransation.Installments = listOfInstallments;
        newInstallment.Payment = approvedTransation;

        await _context.Payments.AddAsync(approvedTransation);
        await _context.SaveChangesAsync();

        return (approvedTransation, true);
    }
}