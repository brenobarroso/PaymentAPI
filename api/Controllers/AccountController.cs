using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Models;
using api.Models;
using api.Interfaces;
namespace PaymentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountManager _manager;

    public AccountController(IAccountManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var accounts = await _manager.getAllAccountsAsync();
        var accountsResult = new List<AccountResult>();

        foreach (Account account in accounts)
        {
            var payments = new List<PaymentResult>();

            foreach (var payment in account.Payments)
            {
                var paymentResult = new PaymentResult
                {
                    Id = payment.Id,
                    TransationDate = payment.TransationDate,
                    ApprovalDate = payment.ApprovalDate,
                    DisapprovalDate = payment.DisapprovalDate,
                    Confirmation = payment.Confirmation,
                    GrossValue = payment.GrossValue,
                    NetValue = payment.NetValue,
                    FlatRate = payment.FlatRate,
                    CardNumber = payment.CardNumber,
                    Installments = new List<InstallmentResult>()
                };

                foreach (var installment in payment.Installments)
                {
                    var installmentResult = new InstallmentResult
                    {
                        Id = installment.Id,
                        InstallmentGrossValue = installment.InstallmentGrossValue,
                        InstallmentNetValue = installment.InstallmentNetValue,
                        InstallmentNumber = installment.InstallmentNumber,
                        ReceiptDate = installment.ReceiptDate
                    };
                    paymentResult.Installments.Add(installmentResult);
                }
                payments.Add(paymentResult);
            }

            var accountResult = new AccountResult
            {
                Id = account.Id,
                CPF = account.CPF,
                Agency = account.Agency,
                HolderName = account.HolderName,
                Balance = account.Balance,
                IsActive = account.IsActive,
                Payments = payments
            };

            accountsResult.Add(accountResult);

        }
        return Ok(accountsResult);
    }

    [HttpGet("get/number/{accountNumber}")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAccount(int accountNumber)
    {
        var account = await _manager.getByAccountNumberAsync(accountNumber);
        var payments = new List<PaymentResult>();

        foreach (var payment in account.Payments)
        {
            var paymentResult = new PaymentResult
            {
                Id = payment.Id,
                TransationDate = payment.TransationDate,
                ApprovalDate = payment.ApprovalDate,
                DisapprovalDate = payment.DisapprovalDate,
                Confirmation = payment.Confirmation,
                GrossValue = payment.GrossValue,
                NetValue = payment.NetValue,
                FlatRate = payment.FlatRate,
                CardNumber = payment.CardNumber,
                Installments = new List<InstallmentResult>()
            };

            foreach (var installment in payment.Installments)
            {
                var installmentResult = new InstallmentResult{
                    Id = installment.Id,
                    InstallmentGrossValue = installment.InstallmentGrossValue,
                    InstallmentNetValue = installment.InstallmentNetValue,
                    InstallmentNumber = installment.InstallmentNumber,
                    ReceiptDate = installment.ReceiptDate
                };
                paymentResult.Installments.Add(installmentResult);
            }
                payments.Add(paymentResult);
        }

        var accountResult = new AccountResult
        {
            Id = account.Id,
            CPF = account.CPF,
            Agency = account.Agency,
            HolderName = account.HolderName,
            Balance = account.Balance,
            IsActive = account.IsActive,
            Payments = payments
        };


        if(account == null)
            return NotFound();
        
        return Ok(accountResult);
    }

    [HttpGet("get/cpf/{cpf}")]
    [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCPF(string cpf)
    {
        var account = await _manager.getByCPFAsync(cpf);
        var payments = new List<PaymentResult>();

        foreach (var payment in account.Payments)
        {
            var paymentResult = new PaymentResult
            {
                Id = payment.Id,
                TransationDate = payment.TransationDate,
                ApprovalDate = payment.ApprovalDate,
                DisapprovalDate = payment.DisapprovalDate,
                Confirmation = payment.Confirmation,
                GrossValue = payment.GrossValue,
                NetValue = payment.NetValue,
                FlatRate = payment.FlatRate,
                CardNumber = payment.CardNumber,
                Installments = new List<InstallmentResult>()
            };

            foreach (var installment in payment.Installments)
            {
                var installmentResult = new InstallmentResult{
                    Id = installment.Id,
                    InstallmentGrossValue = installment.InstallmentGrossValue,
                    InstallmentNetValue = installment.InstallmentNetValue,
                    InstallmentNumber = installment.InstallmentNumber,
                    ReceiptDate = installment.ReceiptDate
                };
                paymentResult.Installments.Add(installmentResult);
            }
                payments.Add(paymentResult);
        }

        var accountResult = new AccountResult
        {
            Id = account.Id,
            CPF = account.CPF,
            Agency = account.Agency,
            HolderName = account.HolderName,
            Balance = account.Balance,
            IsActive = account.IsActive,
            Payments = payments
        };


        if(account == null)
            return NotFound();
        
        return Ok(accountResult);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(AccountViewModel person) // mudar account pra result
    {
        var account = await _manager.CreateAccount(person);
        var payments = new List<PaymentResult>();

        foreach (var payment in account.Payments)
        {
            var paymentResult = new PaymentResult
            {
                Id = payment.Id,
                TransationDate = payment.TransationDate,
                ApprovalDate = payment.ApprovalDate,
                DisapprovalDate = payment.DisapprovalDate,
                Confirmation = payment.Confirmation,
                GrossValue = payment.GrossValue,
                NetValue = payment.NetValue,
                FlatRate = payment.FlatRate,
                CardNumber = payment.CardNumber,
                Installments = new List<InstallmentResult>()
            };

            foreach (var installment in payment.Installments)
            {
                var installmentResult = new InstallmentResult{
                    Id = installment.Id,
                    InstallmentGrossValue = installment.InstallmentGrossValue,
                    InstallmentNetValue = installment.InstallmentNetValue,
                    InstallmentNumber = installment.InstallmentNumber,
                    ReceiptDate = installment.ReceiptDate
                };
                paymentResult.Installments.Add(installmentResult);
            }
                payments.Add(paymentResult);
        }

        var accountResult = new AccountResult
        {
            Id = account.Id,
            CPF = account.CPF,
            Agency = account.Agency,
            HolderName = account.HolderName,
            Balance = account.Balance,
            IsActive = account.IsActive,
            Payments = payments
        };

            if(account == null)
                UnprocessableEntity("error");
            return Ok(accountResult);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> MakeInactive(int id) // mudar account pra result
    {
        var account = await _manager.DeleteAccount(id);
        var payments = new List<PaymentResult>();

        foreach (var payment in account.Payments)
        {
            var paymentResult = new PaymentResult
            {
                Id = payment.Id,
                TransationDate = payment.TransationDate,
                ApprovalDate = payment.ApprovalDate,
                DisapprovalDate = payment.DisapprovalDate,
                Confirmation = payment.Confirmation,
                GrossValue = payment.GrossValue,
                NetValue = payment.NetValue,
                FlatRate = payment.FlatRate,
                CardNumber = payment.CardNumber,
                Installments = new List<InstallmentResult>()
            };

            foreach (var installment in payment.Installments)
            {
                var installmentResult = new InstallmentResult{
                    Id = installment.Id,
                    InstallmentGrossValue = installment.InstallmentGrossValue,
                    InstallmentNetValue = installment.InstallmentNetValue,
                    InstallmentNumber = installment.InstallmentNumber,
                    ReceiptDate = installment.ReceiptDate
                };
                paymentResult.Installments.Add(installmentResult);
            }
                payments.Add(paymentResult);
        }

        var accountResult = new AccountResult
        {
            Id = account.Id,
            CPF = account.CPF,
            Agency = account.Agency,
            HolderName = account.HolderName,
            Balance = account.Balance,
            IsActive = account.IsActive,
            Payments = payments
        };

        if(account == null)
            return BadRequest();
        
        return Ok(accountResult);
    }
}
