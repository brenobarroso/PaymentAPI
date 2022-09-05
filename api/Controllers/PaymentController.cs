using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Models;
using System;

namespace PaymentAPI.Controllers
{
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
            return payment == null ? NotFound() : Ok(payment);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Transaction(Payment payment)
        {
            // Teste para saber se os modelos passados estão OK.
            if(!ModelState.IsValid)
                return BadRequest();

            // Teste para saber se número do cartão tem 16 dígitos.
            if(payment.CardNumber.Length != 16)
                return BadRequest();

            // Teste para saber se todos os 16 caracteres digitados são números.
            if(!(payment.CardNumber.All(char.IsDigit)))
                return BadRequest();


            // IF -> transação reprovada por prefixo :: ELSE-> transação aprovada
            if(payment.CardNumber.IndexOf("5999") == 0)
            {
                string fourLastDigitsOfCardReproved = payment.CardNumber.Substring(payment.CardNumber.Length - 4);

                var reprovedTransaction = new Payment{
                    TransationDate = DateTime.UtcNow,
                    ApprovalDate = null,
                    DisapprovalDate = DateTime.UtcNow,
                    Confirmation = false,
                    GrossValue = payment.GrossValue,
                    NetValue = payment.NetValue,
                    FlatRate = payment.FlatRate,
                    CardNumber = fourLastDigitsOfCardReproved
                };

                await _context.Payments.AddAsync(reprovedTransaction);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                string fourLastDigitsOfCardApproved = payment.CardNumber.Substring(payment.CardNumber.Length - 4);

                var approvedTransation = new Payment{
                    TransationDate = DateTime.UtcNow,
                    ApprovalDate = DateTime.UtcNow,
                    DisapprovalDate = null,
                    Confirmation = true,
                    FlatRate = payment.FlatRate,
                    GrossValue = payment.GrossValue,
                    NetValue = payment.GrossValue - payment.FlatRate,
                    CardNumber = fourLastDigitsOfCardApproved
                };

                await _context.Payments.AddAsync(approvedTransation);
                await _context.SaveChangesAsync();

                return Ok();

            }
        }  
    }

    // Validação se o valor bruto da transação existe e não é negativo.
    public class ValueAttribute : ValidationAttribute // Validação se o valor bruto for passado mas for negativo.
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return (float) value < 0 || (float) value == null 
                ? new ValidationResult(ErrorMessage="Valor Negativo") 
                : ValidationResult.Success;
        }
    }

    // Validação se existe espaços em branco.
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
                ? new ValidationResult("Atenção! Espaço em  detectado.")
                : ValidationResult.Success;
        }
    }
}