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
    private readonly PaymentDbContext _context;
    public PaymentController(PaymentDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var payments = await _context.Payments.ToListAsync();
        return Ok(payments);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
            return NotFound();
        else
            return Ok(payment);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Transaction(PaymentViewModel viewModel)
    {
        var result = TransactionsManager.Validation(viewModel);

        if (result.sucess == true)
        {
            await _context.Payments.AddAsync(result.payment);
            await _context.SaveChangesAsync();

            return Ok();
        }

        return BadRequest();
    }
}
