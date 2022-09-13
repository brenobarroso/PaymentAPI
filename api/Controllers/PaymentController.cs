using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;
using PaymentAPI.Validations;
using System;
using api.Models;

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
        return Ok(payments);
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
