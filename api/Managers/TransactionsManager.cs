using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;
using api.Models;
using api.Interfaces;
using api.Validations;
using api.Managers;
using api.Models.Movements;

namespace api.Managers;

public class TransactionsManager : ITransactionsManager
{
    private readonly PaymentDbContext _context;
    private readonly IAccountManager _accountManager;

    public TransactionsManager(PaymentDbContext context, IAccountManager accountManager)
    {
        _context = context;
        _accountManager = accountManager;
    }

    
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

    public async Task<(Payment? payment, bool sucess)> CreatAsync(PaymentViewModel viewModel)
    {
        var query = await _accountManager.GetByAccountNumberAsync(viewModel.AccountNumber);
        if(query == null)
            return (null, false);


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
                CardNumber = fourLastDigitsOfCardReproved,
                Account = query
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
            CardNumber = fourLastDigitsOfCardApproved,
            Account = query
        };
        approvedTransation.NetValue = approvedTransation.GrossValue - approvedTransation.FlatRate;

        query.Balance = ((decimal)(query.Balance + approvedTransation.NetValue));

        var listOfInstallments = new List<Installment>();
        
        for(int i = 1; i <= viewModel.InstallmentQuantity; i++)
        {
            var newInstallment = new Installment
            {
                ReceiptDate = DateTime.UtcNow.AddDays(30 * i),
                InstallmentNumber = i,
                InstallmentGrossValue = (viewModel.GrossValue / viewModel.InstallmentQuantity),
            };

            newInstallment.InstallmentNetValue = ((decimal)(((approvedTransation.NetValue ) / viewModel.InstallmentQuantity) - approvedTransation.FlatRate));
            
            listOfInstallments.Add(newInstallment);

            approvedTransation.Installments = listOfInstallments;
            newInstallment.Payment = approvedTransation;
        }

        var newMovement = new Movement{
            Date = DateTime.UtcNow,
            NetValue = (decimal)approvedTransation.NetValue,
            GrossValue = (decimal)approvedTransation.GrossValue,
            Comments = " " + DateTime.UtcNow.Date + " - " + DateTime.UtcNow.Hour + ":" + DateTime.UtcNow.Minute +
                 "R$" + approvedTransation.NetValue + 
                 " entrada - pagamento cartão - " + "transação em " + viewModel.InstallmentQuantity + 
                 " vezes no cartão de final " + fourLastDigitsOfCardApproved + ".",
            Withdraw = null,
            Payment = approvedTransation,
            Account = query
        };

        approvedTransation.Movement = newMovement;

        await _context.Payments.AddAsync(approvedTransation);
        await _context.SaveChangesAsync();

        return (approvedTransation, true);
    }

}
