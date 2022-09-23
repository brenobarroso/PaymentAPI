using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Models;
using api.Models;
using api.Interfaces;

namespace PaymentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{   
    private readonly ITransactionsManager _manager;

    public PaymentController(ITransactionsManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var payments = await _manager.getAllAsync();
        var result = new List<PaymentResult>();
        
        foreach (Payment payment in payments)
        {
            
            var paymentResult = new PaymentResult{
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
            result.Add(paymentResult);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _manager.getByIdAsync(id);

        if (payment == null)
            return NotFound();
        else
            return Ok(payment);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Transaction(PaymentViewModel viewModel)
    {
        var result = await _manager.CreatAsync(viewModel);

        if (result.sucess)
            return Ok(result.payment);

        return UnprocessableEntity("payment reproved");
    }
}