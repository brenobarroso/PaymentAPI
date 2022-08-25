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
            if(!ModelState.IsValid)
                return BadRequest();
            
            if(payment.CardNumber.IndexOf("5999") == 0)
            {
                var reprovedTransaction = new Payment{
                    TransationDate = DateTime.UtcNow,
                    ApprovalDate = null,
                    DisapprovalDate = DateTime.UtcNow,
                    Confirmation = false,
                    GrossValue = payment.GrossValue,
                    NetValue = payment.NetValue,
                    FlatRate = payment.FlatRate,
                    CardNumber = payment.CardNumber
                };

                await _context.Payments.AddAsync(reprovedTransaction);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {

                var approvedTransation = new Payment{
                    TransationDate = DateTime.UtcNow,
                    ApprovalDate = DateTime.UtcNow,
                    DisapprovalDate = null,
                    Confirmation = true,
                    FlatRate = payment.FlatRate,
                    GrossValue = payment.GrossValue,
                    NetValue = payment.GrossValue - payment.FlatRate,
                    CardNumber = payment.CardNumber
                };

                await _context.Payments.AddAsync(approvedTransation);
                await _context.SaveChangesAsync();

                return Ok();

            }
        }  
    }

    public class ValueAttribute : ValidationAttribute // Validação se o valor bruto for passado mas for negativo.
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return (float) value < 0 || (float) value == null 
                ? new ValidationResult(ErrorMessage="Valor Negativo") 
                : ValidationResult.Success;
        }
    }

    public class CardNumberAttribute : ValidationAttribute // Validação se possui caracter vazio
    {
        public string EmptyChar { get; set; }

        public CardNumberAttribute (string eChar)
        {
            EmptyChar = eChar;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return ((string) value).Contains(EmptyChar)
                ? new ValidationResult("Atenção! Padrão reconhecido e/ou espaço em branco.")
                : ValidationResult.Success;
        }
    }
}