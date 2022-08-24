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
    }
}