using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using MessageBus.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace SignalR.MessageBus
{
    public class AzureServiceBusConsumer : BackgroundService
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _sendMessageToSignalRQueueName;
        private readonly IDatabase _redisDb;
        private readonly IMessageBus _messageBus;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<MessagesHub> _messagesHubContext;
        private readonly IHubContext<PresenceHub> _presenceHubContext;

        public AzureServiceBusConsumer(IMessageBus messageBus,
            IConnectionMultiplexer connectionMultiplexer,
            IConfiguration configuration,
            IHubContext<MessagesHub> messagesHubContext,
            IHubContext<PresenceHub> presenceHubContext)
        {
            this._messageBus = messageBus;
            this._connectionMultiplexer = connectionMultiplexer;
            _configuration = configuration;
            this._messagesHubContext = messagesHubContext;
            this._presenceHubContext = presenceHubContext;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _sendMessageToSignalRQueueName = _configuration.GetValue<string>("sendMessageToSignalRQueue");
            _redisDb = _connectionMultiplexer.GetDatabase();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(_serviceBusConnectionString);
            var sendMessageToSignalRQueueProcessor = client.CreateProcessor(_sendMessageToSignalRQueueName);
            sendMessageToSignalRQueueProcessor.ProcessMessageAsync += OnMessageToSignalRQueueReceived;
            sendMessageToSignalRQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await sendMessageToSignalRQueueProcessor.StartProcessingAsync();
            return;
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnMessageToSignalRQueueReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var sendMessageToSignalREvent = JsonSerializer.Deserialize<SendMessageToSignalREvent>(body);

            if (sendMessageToSignalREvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }

            var groupName = GetGroupName(sendMessageToSignalREvent.SenderEmail, sendMessageToSignalREvent.RecipientEmail);

            var values = await _redisDb.HashValuesAsync(groupName);
            if (values.Contains(sendMessageToSignalREvent.RecipientEmail))
            {
                sendMessageToSignalREvent.DateRead = DateTime.UtcNow;
                var markChatMessageAsReadEvent = new MarkChatMessageAsReadEvent() { Id = sendMessageToSignalREvent.Id, DateRead = (DateTime)sendMessageToSignalREvent.DateRead };
                await _messageBus.PublishMessage(markChatMessageAsReadEvent, "mark-chat-message-as-read");
            }
                
            await _messagesHubContext.Clients.Group(groupName).SendAsync("NewMessage", sendMessageToSignalREvent);
            await _presenceHubContext.Clients.User(sendMessageToSignalREvent.RecipientId).SendAsync("NewMessageReceived",new { senderEmail = sendMessageToSignalREvent.SenderEmail});

            await args.CompleteMessageAsync(args.Message);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private async Task OnUserRegisterReceived(ProcessMessageEventArgs args)
        {
            //var message = args.Message;
            //var body = Encoding.UTF8.GetString(message.Body);

            //var newUserRegisterCreateUser = JsonSerializer.Deserialize<NewUserRegisterCreateUser>(body);

            //if (newUserRegisterCreateUser is null)
            //{
            //    throw new ArgumentNullException("newUserRegisterCreateUser is empty");
            //}

            //var user = new User()
            //{
            //    Id = newUserRegisterCreateUser.Id,
            //    Email = newUserRegisterCreateUser.Email,
            //    PhotoUrl = newUserRegisterCreateUser.PhotoUrl,
            //};

            //await userRepositorySingleton.AddUser(user);
            //await args.CompleteMessageAsync(args.Message);

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
