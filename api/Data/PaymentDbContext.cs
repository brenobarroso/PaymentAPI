using api.Models;
using api.Models.Movements;
using api.Models.Withdraws;
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
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Withdraw> Withdraws { get; set; }
    public DbSet<Movement> Movements { get; set; }
}
