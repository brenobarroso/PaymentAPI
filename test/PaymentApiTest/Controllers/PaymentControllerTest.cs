using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using Xunit;


namespace PaymentApiTest.Controllers
{
    public class PaymentControllerTest
    {
        protected readonly PaymentDbContext _context;
        public PaymentControllerTest()
        {
            var options = new DbContextOptionsBuilder<PaymentDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PaymentDbContext(options);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task ShouldListAll()
        {
            // Arrange

            var paymentAPIController = new PaymentController(_context);

            //Act

            var result = (OkObjectResult)await paymentAPIController.Get();

            // Assert

            Assert.Equal(200, result.StatusCode);
            
        }
    }
}