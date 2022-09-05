using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI;
using PaymentAPI.Controllers;
using PaymentAPI.Data;
using PaymentAPI.Models;
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


            var payment = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654785698745"
            };

            var payment2 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654787498745"
            };

            var payment3 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023054785698025"
            };

            var payment4 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "102365478569825"
            };

            var payment5 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654485698025"
            };

            await _context.AddAsync(payment);
            await _context.AddAsync(payment2);
            await _context.AddAsync(payment3);
            await _context.AddAsync(payment4);
            await _context.AddAsync(payment5);


            //Act

            var result = (OkObjectResult)await paymentAPIController.Get();

            // Assert

            Assert.Equal(200, result.StatusCode);

        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task ShouldListAnExistentId(int id)
        {
            // Arrange

            var paymentController = new PaymentController(_context);

            var payment = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654785698745"
            };

            var payment2 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654787498745"
            };

            var payment3 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023054785698025"
            };

            var payment4 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "102365478569825"
            };

            var payment5 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654485698025"
            };

            await _context.AddAsync(payment);
            await _context.AddAsync(payment2);
            await _context.AddAsync(payment3);
            await _context.AddAsync(payment4);
            await _context.AddAsync(payment5);

            //Act

            var result = (OkObjectResult)await paymentController.GetById(id);

            // Assert

            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [InlineData(500)]
        [InlineData(50)]
        [InlineData(90)]
        [InlineData(6)]
        [InlineData(30)]
        [InlineData(10)]
        public async Task ShouldListAnNotExistentId(int id)
        {
            // Arrange

            var paymentController = new PaymentController(_context);

            var payment = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654785698745"
            };

            var payment2 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654787498745"
            };

            var payment3 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023054785698025"
            };

            var payment4 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "102365478569825"
            };

            var payment5 = new Payment
            {
                GrossValue = 5000f,
                CardNumber = "1023654485698025"
            };

            await _context.AddAsync(payment);
            await _context.AddAsync(payment2);
            await _context.AddAsync(payment3);
            await _context.AddAsync(payment4);
            await _context.AddAsync(payment5);

            //Act

            var result = (NotFoundResult)await paymentController.GetById(id);

            // Assert

            Assert.Equal(404, result.StatusCode);
        }

        [Theory]
        [InlineData(5000f, "5630125478536540")]
        public async Task TransactionShouldBeApproved(int grossValue, string cardNumber)
        {
            // Arrange
            var paymentController = new PaymentController(_context);

            var viewModel = new Payment
            {
                GrossValue = grossValue,
                CardNumber = cardNumber
            };

            // Act

            var result = (OkResult)await paymentController.Transaction(viewModel);

            var payments = await _context.Payments.ToListAsync();

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Single(payments);
            Assert.Equal(viewModel.GrossValue, payments.First().GrossValue);
            Assert.Equal(DateTime.UtcNow.Date, payments.First().TransationDate.Date);
        }

        [Theory]
        [InlineData(5000f, "5999125478536540")]
        public async Task TransactionShouldBeReproved(int grossValue, string cardNumber)
        {
            // Arrange

            var paymentController = new PaymentController(_context);

            var payment = new Payment
            {
                GrossValue = grossValue,
                CardNumber = cardNumber
            };

            // Act

            var result = (OkResult)await paymentController.Transaction(payment);

            // Assert

            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        // [InlineData(0f, "0000000000000000")]
        // [InlineData(-100f, "0000000000000000")]
        // [InlineData(null, "0000000000000000")]
        [InlineData(1f, "0000")]
        [InlineData(1f, "00000000000000000")]
        [InlineData(1f, "000000000000000 0")]
        [InlineData(1f, "000000000000000a")]
        [InlineData(1f, "0000000000000!00")]
        [InlineData(1f, "01254703h!023654")]
        public async Task TransactionCannotBeMade(int grossValue, string cardNumber)
        {
            // Arrange

            var paymentController = new PaymentController(_context);

            var payment = new Payment
            {
                GrossValue = grossValue,
                CardNumber = cardNumber
            };

            // Act

            var result = (BadRequestResult)await paymentController.Transaction(payment);

            // Assert

            Assert.Equal(400, result.StatusCode);
        }
    }
}