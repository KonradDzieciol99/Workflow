using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mango.MessageBus;
using System.Net.Http.Json;
using System.Text.Json;
using Email.Common.Models;
using EmailSender;

namespace EmailSender.MessageBus
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string NewUserRegisterTopic;
        private readonly string NewUserRegisterSubscription;
        private readonly IConfiguration _configuration;
        ///private readonly IMessageBus _messageBus;
        private readonly IEmailSender _emailSender;
        private ServiceBusProcessor UserRegisterProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, IEmailSender emailSender)
        {
            _configuration = configuration;
            //_messageBus = messageBus;
            _emailSender = emailSender;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString"); ;
            NewUserRegisterTopic = _configuration.GetValue<string>("NewUserRegisterTopic");
            NewUserRegisterSubscription = _configuration.GetValue<string>("NewUserRegisterSubscription");

            var client = new ServiceBusClient(serviceBusConnectionString);

            UserRegisterProcessor = client.CreateProcessor(NewUserRegisterTopic, NewUserRegisterSubscription);
        }

        public async Task Start()
        {
            UserRegisterProcessor.ProcessMessageAsync += OnUserRegisterReceived;
            UserRegisterProcessor.ProcessErrorAsync += ErrorHandler;
            await UserRegisterProcessor.StartProcessingAsync();
        }
        public async Task Stop()
        {
            await UserRegisterProcessor.StopProcessingAsync();
            await UserRegisterProcessor.DisposeAsync();
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

            var registerEmailBusMessage = JsonSerializer.Deserialize<NewUserRegisterEmail>(body);

            if (registerEmailBusMessage is null)
            {
                throw new ArgumentNullException("registerEmailBusMessage is empty");
            }
            //var c = new EmailSender();
            await _emailSender.SendConfirmEmailMessage(registerEmailBusMessage);
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
