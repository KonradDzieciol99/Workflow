using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using MessageBus;
using Microsoft.Extensions.Configuration;
using Socjal.API.Entity;
using Socjal.API.Repositories;

namespace Socjal.API.MessageBus
{
    public class AzureServiceBusConsumer : BackgroundService
    {
        private readonly string serviceBusConnectionString;
        private readonly string azureBusTopic;
        private readonly string azureBusSubscription;
        private readonly IConfiguration _configuration;
        private readonly IUserRepositorySingleton userRepositorySingleton;
        private ServiceBusProcessor UserRegisterProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IUserRepositorySingleton userRepositorySingleton)
        {
            _configuration = configuration;
            this.userRepositorySingleton = userRepositorySingleton;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString"); ;
            azureBusTopic = _configuration.GetValue<string>("AzureBusTopic");
            azureBusSubscription = _configuration.GetValue<string>("AzureBusSubscription");
        }

        //public async Task Start()
        //{
        //    UserRegisterProcessor.ProcessMessageAsync += OnUserRegisterReceived;
        //    UserRegisterProcessor.ProcessErrorAsync += ErrorHandler;
        //    await UserRegisterProcessor.StartProcessingAsync();
        //}
        //public async Task Stop()
        //{
        //    await UserRegisterProcessor.StopProcessingAsync();
        //    await UserRegisterProcessor.DisposeAsync();
        //}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(serviceBusConnectionString);
            UserRegisterProcessor = client.CreateProcessor(azureBusTopic, azureBusSubscription);
            UserRegisterProcessor.ProcessMessageAsync += OnUserRegisterReceived;
            UserRegisterProcessor.ProcessErrorAsync += ErrorHandler;
            await UserRegisterProcessor.StartProcessingAsync();
            return;
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnUserRegisterReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newUserRegisterCreateUser = JsonSerializer.Deserialize<NewUserRegisterCreateUser>(body);

            if (newUserRegisterCreateUser is null)
            {
                throw new ArgumentNullException("newUserRegisterCreateUser is empty");
            }

            var user = new User()
            {
                Id = newUserRegisterCreateUser.Id,
                Email= newUserRegisterCreateUser.Email,
                PhotoUrl= newUserRegisterCreateUser.PhotoUrl,
            };

            await userRepositorySingleton.AddUser(user);
            await args.CompleteMessageAsync(args.Message);

        }
        //private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        //{
        //    var message = args.Message;
        //    var body = Encoding.UTF8.GetString(message.Body);

        //    CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

        //    OrderHeader orderHeader = new()
        //    {
        //        UserId = checkoutHeaderDto.UserId,
        //        FirstName = checkoutHeaderDto.FirstName,
        //        LastName = checkoutHeaderDto.LastName,
        //        OrderDetails = new List<OrderDetails>(),
        //        CardNumber = checkoutHeaderDto.CardNumber,
        //        CouponCode = checkoutHeaderDto.CouponCode,
        //        CVV = checkoutHeaderDto.CVV,
        //        DiscountTotal = checkoutHeaderDto.DiscountTotal,
        //        Email = checkoutHeaderDto.Email,
        //        ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
        //        OrderTime = DateTime.Now,
        //        OrderTotal = checkoutHeaderDto.OrderTotal,
        //        PaymentStatus = false,
        //        Phone = checkoutHeaderDto.Phone,
        //        PickupDateTime = checkoutHeaderDto.PickupDateTime
        //    };
        //    foreach (var detailList in checkoutHeaderDto.CartDetails)
        //    {
        //        OrderDetails orderDetails = new()
        //        {
        //            ProductId = detailList.ProductId,
        //            ProductName = detailList.Product.Name,
        //            Price = detailList.Product.Price,
        //            Count = detailList.Count
        //        };
        //        orderHeader.CartTotalItems += detailList.Count;
        //        orderHeader.OrderDetails.Add(orderDetails);
        //    }

        //    await _orderRepository.AddOrder(orderHeader);


        //    PaymentRequestMessage paymentRequestMessage = new()
        //    {
        //        Name = orderHeader.FirstName + " " + orderHeader.LastName,
        //        CardNumber = orderHeader.CardNumber,
        //        CVV = orderHeader.CVV,
        //        ExpiryMonthYear = orderHeader.ExpiryMonthYear,
        //        OrderId = orderHeader.OrderHeaderId,
        //        OrderTotal = orderHeader.OrderTotal,
        //        Email = orderHeader.Email
        //    };

        //    try
        //    {
        //        await _messageBus.PublishMessage(paymentRequestMessage, orderPaymentProcessTopic);
        //        await args.CompleteMessageAsync(args.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        throw;
        //    }

        //}

        //private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        //{
        //    var message = args.Message;
        //    var body = Encoding.UTF8.GetString(message.Body);

        //    UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

        //    await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
        //    await args.CompleteMessageAsync(args.Message);

        //}
    }

}
