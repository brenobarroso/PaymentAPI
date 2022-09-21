using api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models;

namespace PaymentAPI.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        :base(options)
    {
        
    }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<Installment> Installments { get; set; }
}
