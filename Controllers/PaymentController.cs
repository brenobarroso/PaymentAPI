using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;

namespace PaymentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentDbContext _context;
        public PaymentController(PaymentDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Payment>> Get() => await _context.Payments.ToListAsync();
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            return payment == null ? NotFound() : Ok(payment);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Transaction(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            if(!ModelState.IsValid)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new {id = payment.Id}, payment);
        }
    }

    public class ValueAttribute : ValidationAttribute // Validação se o valor bruto for passado mas for negativo.
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return (float) value < 0 ? new ValidationResult(ErrorMessage="Valor Negativo") : ValidationResult.Success;
        }
    }

    public class CardNumberWithEmptyCharAttribute : ValidationAttribute // Validação se possui caracter vazio
    {
        public string EmptyChar { get; set; }

        public CardNumberWithEmptyCharAttribute (string eChar)
        {
            EmptyChar = eChar;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return ((string) value).Contains(EmptyChar)? new ValidationResult("Caracter vazio detectado") : ValidationResult.Success;
        }
    }
}