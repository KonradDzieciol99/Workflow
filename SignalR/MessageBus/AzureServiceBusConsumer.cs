using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace SignalR.MessageBus
{
    public class AzureServiceBusConsumer : BackgroundService
    {
        private readonly string _serviceBusConnectionString;
        private readonly string _sendMessageToSignalRQueueName;
        private readonly string _newOnlineUserWithFriendsQueueName;
        private readonly string _newOnlineMessagesUserWithFriendsQueueName;
        private readonly string _newOfflineUserWithFriendsQueueName;
        private readonly string _friendInvitationAcceptedQueueName;

        //private readonly string _newOnlineUserQueueName;
        private readonly IDatabase _redisDb;
        private readonly IMessageBus _messageBus;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IHubContext<PresenceHub> _presenceHubContext;
        private readonly IHubContext<MessagesHub> _messagesHubContext;

        public AzureServiceBusConsumer(IMessageBus messageBus,
            IConnectionMultiplexer connectionMultiplexer,
            IConfiguration configuration,
            IHubContext<ChatHub> chatHubContext,
            IHubContext<PresenceHub> presenceHubContext,
            IHubContext<MessagesHub> messagesHubContext)
        {
            this._messageBus = messageBus;
            this._connectionMultiplexer = connectionMultiplexer;
            _configuration = configuration;
            this._chatHubContext = chatHubContext;
            this._presenceHubContext = presenceHubContext;
            this._messagesHubContext = messagesHubContext;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            _sendMessageToSignalRQueueName = _configuration.GetValue<string>("sendMessageToSignalRQueue");
            _newOnlineUserWithFriendsQueueName = _configuration.GetValue<string>("newOnlineUserWithFriendsQueue");
            _newOnlineMessagesUserWithFriendsQueueName = _configuration.GetValue<string>("NewOnlineMessagesUserWithFriendsQueue");
            _newOfflineUserWithFriendsQueueName = _configuration.GetValue<string>("NewOfflineUserWithFriendsQueue");
            _friendInvitationAcceptedQueueName = _configuration.GetValue<string>("FriendInvitationAcceptedQueue");

            _redisDb = _connectionMultiplexer.GetDatabase();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(_serviceBusConnectionString);
            var sendMessageToSignalRQueueProcessor = client.CreateProcessor(_sendMessageToSignalRQueueName);
            sendMessageToSignalRQueueProcessor.ProcessMessageAsync += OnMessageToSignalRQueueReceived;
            sendMessageToSignalRQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await sendMessageToSignalRQueueProcessor.StartProcessingAsync();

            var newOnlineUserWithFriendsQueueProcessor = client.CreateProcessor(_newOnlineUserWithFriendsQueueName);//presenceHub
            newOnlineUserWithFriendsQueueProcessor.ProcessMessageAsync += OnNewOnlineUserWithFriendsQueueReceived;
            newOnlineUserWithFriendsQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await newOnlineUserWithFriendsQueueProcessor.StartProcessingAsync();
           
            var newOnlineMessagesUserWithFriendsQueueProcessor = client.CreateProcessor(_newOnlineMessagesUserWithFriendsQueueName);//messagesHub
            newOnlineMessagesUserWithFriendsQueueProcessor.ProcessMessageAsync += OnNewOnlineMessagesUserWithFriendsQueueReceived;
            newOnlineMessagesUserWithFriendsQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await newOnlineMessagesUserWithFriendsQueueProcessor.StartProcessingAsync();

            var newOfflineUserWithFriendsQueueProcessor = client.CreateProcessor(_newOfflineUserWithFriendsQueueName);
            newOfflineUserWithFriendsQueueProcessor.ProcessMessageAsync += OnNewOfflineUserWithFriendsQueueReceived;
            newOfflineUserWithFriendsQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await newOfflineUserWithFriendsQueueProcessor.StartProcessingAsync();

            var friendInvitationAcceptedQueueProcessor = client.CreateProcessor(_friendInvitationAcceptedQueueName);
            friendInvitationAcceptedQueueProcessor.ProcessMessageAsync += OnFriendInvitationAcceptedQueueReceived;
            friendInvitationAcceptedQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await friendInvitationAcceptedQueueProcessor.StartProcessingAsync();

            return;
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        private async Task OnFriendInvitationAcceptedQueueReceived(ProcessMessageEventArgs args)//OnNewOnline CHAT UserQueueReceived
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var friendInvitationAcceptedEvent = JsonSerializer.Deserialize<FriendInvitationAcceptedEvent>(body);

            if (friendInvitationAcceptedEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }

            await _messagesHubContext.Clients.User(friendInvitationAcceptedEvent.UserWhoseInvitationAccepted.UserId).SendAsync("FriendInvitationAccepted", friendInvitationAcceptedEvent.FriendInvitationDto);

            var isOnline = await _redisDb.KeyExistsAsync($"presence-{friendInvitationAcceptedEvent.UserWhoAcceptedInvitation.UserEmail}");
            if (isOnline)
            {
                await _messagesHubContext.Clients.User(friendInvitationAcceptedEvent.UserWhoseInvitationAccepted.UserId).SendAsync("UserIsOnline", friendInvitationAcceptedEvent.UserWhoAcceptedInvitation);
            }

            await args.CompleteMessageAsync(args.Message);
        }
        private async Task OnNewOfflineUserWithFriendsQueueReceived(ProcessMessageEventArgs args)//OnNewOnline CHAT UserQueueReceived
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newOfflineUserWithFriendsEvent = JsonSerializer.Deserialize<NewOfflineUserWithFriendsEvent>(body);

            if (newOfflineUserWithFriendsEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }

            //List<Task<bool>> listOfOnlineUsers = new();
            //foreach (var item in newOfflineUserWithFriendsEvent.UserChatFriends)
            //{
            //    listOfOnlineUsers.Add(_redisDb.KeyExistsAsync($"presence-{item.UserEmail}"));// było UserId
            //}
            //var resoult = await Task.WhenAll(listOfOnlineUsers);

            //List<SimpleUser> onlineUsers = new();
            //for (int i = 0; i < newOfflineUserWithFriendsEvent.UserChatFriends.Count(); i++)
            //{
            //    if (resoult[i])
            //    {
            //        onlineUsers.Add(newOfflineUserWithFriendsEvent.UserChatFriends.ElementAt(i));
            //    }
            //}
            await _messagesHubContext.Clients.Users(newOfflineUserWithFriendsEvent.UserChatFriends.Select(x => x.UserId)).SendAsync("UserIsOffline", newOfflineUserWithFriendsEvent.User);
            await args.CompleteMessageAsync(args.Message);
        }
        private async Task OnNewOnlineMessagesUserWithFriendsQueueReceived(ProcessMessageEventArgs args)//OnNewOnline CHAT UserQueueReceived
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newOnlineMessagesUserWithFriendsEvent = JsonSerializer.Deserialize<NewOnlineMessagesUserWithFriendsEvent>(body);

            if (newOnlineMessagesUserWithFriendsEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }

            List<Task<bool>> listOfOnlineUsers = new();
            foreach (var item in newOnlineMessagesUserWithFriendsEvent.NewOnlineUserChatFriends)
            {
                listOfOnlineUsers.Add(_redisDb.KeyExistsAsync($"presence-{item.UserEmail}"));// było UserId
            }
            var resoult = await Task.WhenAll(listOfOnlineUsers);

            List<SimpleUser> onlineUsers = new();
            for (int i = 0; i < newOnlineMessagesUserWithFriendsEvent.NewOnlineUserChatFriends.Count(); i++)
            {
                if (resoult[i])
                {
                    onlineUsers.Add(newOnlineMessagesUserWithFriendsEvent.NewOnlineUserChatFriends.ElementAt(i));
                }
            }
            await _messagesHubContext.Clients.User(newOnlineMessagesUserWithFriendsEvent.NewOnlineUser.UserId).SendAsync("GetOnlineUsers", onlineUsers);
            await args.CompleteMessageAsync(args.Message);
        }
        private async Task OnNewOnlineUserWithFriendsQueueReceived(ProcessMessageEventArgs args)//OnNewOnline CHAT UserQueueReceived
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newOnlineUserWithFriendsEvent = JsonSerializer.Deserialize<NewOnlineUserWithFriendsEvent>(body);

            if (newOnlineUserWithFriendsEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }

            //var groupName = GetGroupName(sendMessageToSignalREvent.SenderEmail, sendMessageToSignalREvent.RecipientEmail);

            //var values = await _redisDb.setle
            //if (values.Contains(sendMessageToSignalREvent.RecipientEmail))
            //{
            //    sendMessageToSignalREvent.DateRead = DateTime.UtcNow;
            //    var markChatMessageAsReadEvent = new MarkChatMessageAsReadEvent() { Id = sendMessageToSignalREvent.Id, DateRead = (DateTime)sendMessageToSignalREvent.DateRead };
            //    await _messageBus.PublishMessage(markChatMessageAsReadEvent, "mark-chat-message-as-read");
            //}
            List<Task<bool>> listOfOnlineUsers = new();
            foreach (var item in newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends)
            {
                listOfOnlineUsers.Add(_redisDb.KeyExistsAsync($"presence-{item.UserEmail}"));//UserId
            }
            var resoult = await Task.WhenAll(listOfOnlineUsers);

            ///resoult and newOnlineUserEvent.NewOnlineUserChatFriends merge razem !!
            //List<Para> listaPar = listaNapisow.Zip(listaLiczb, (napis, liczba) => new Para(napis, liczba)).ToList();

            List<SimpleUser> onlineUsers = new();
            for (int i = 0; i < newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.Count(); i++)
            {
                if (resoult[i])
                {
                    onlineUsers.Add(newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.ElementAt(i));
                }
            }

            await _messagesHubContext.Clients.Users(newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.Select(x => x.UserId)).SendAsync("UserIsOnline", newOnlineUserWithFriendsEvent.NewOnlineUser);

            /////////await _messagesHubContext.Clients.User(newOnlineUserWithFriendsEvent.NewOnlineUser.UserId).SendAsync("GetOnlineUsers", onlineUsers);

            //await _messagesHubContext.Clients.All.SendAsync("GetOnlineUsers", "sdfsdfsdfs");

            //await _presenceHubContext.Clients.User(sendMessageToSignalREvent.RecipientId).SendAsync("NewMessageReceived", new { senderEmail = sendMessageToSignalREvent.SenderEmail });

            await args.CompleteMessageAsync(args.Message);
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
                
            await _chatHubContext.Clients.Group(groupName).SendAsync("NewMessage", sendMessageToSignalREvent);
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
