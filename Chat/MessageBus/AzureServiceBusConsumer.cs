using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Chat.Dto;
using Chat.Entity;
using Chat.Persistence;
using Chat.Repositories;
using Mango.MessageBus;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Chat.MessageBus
{
    public class AzureServiceBusConsumer : BackgroundService
    {
        private readonly string _serviceBusConnectionString;
        private readonly string azureBusTopic;
        private readonly string azureBusSubscription;
        private readonly string _markChatMessageAsReadQueueName;
        private readonly IMessageBus _messageBus;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserRepositorySingleton userRepositorySingleton;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly string _newOnlineUserQueueName;
        private readonly string _newOfflineUserTopicName;
        private readonly string _newOfflineUserTopicChatSubName;
        private readonly string _friendInvitationAcceptedQueueName;

        //private ServiceBusProcessor UserRegisterProcessor;

        public AzureServiceBusConsumer(IMessageBus messageBus, IMapper mapper,IConfiguration configuration, IUserRepositorySingleton userRepositorySingleton, DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            this._messageBus = messageBus;
            this._mapper = mapper;
            _configuration = configuration;
            this.userRepositorySingleton = userRepositorySingleton;
            this._dbContextOptions = dbContextOptions;
            _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString"); ;
            azureBusTopic = _configuration.GetValue<string>("AzureBusTopic");
            azureBusSubscription = _configuration.GetValue<string>("AzureBusSubscription");
            _markChatMessageAsReadQueueName = _configuration.GetValue<string>("markChatMessageAsReadQueue");
            _newOnlineUserQueueName = _configuration.GetValue<string>("newOnlineUserQueue");
            _newOfflineUserTopicName = _configuration.GetValue<string>("newOfflineUserTopic");
            _newOfflineUserTopicChatSubName = _configuration.GetValue<string>("newOfflineUserTopicChatSub");
            _friendInvitationAcceptedQueueName = _configuration.GetValue<string>("FriendInvitationAcceptedQueue");

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
            var client = new ServiceBusClient(_serviceBusConnectionString);
            var UserRegisterProcessor = client.CreateProcessor(azureBusTopic, azureBusSubscription);
            UserRegisterProcessor.ProcessMessageAsync += OnUserRegisterReceived;
            UserRegisterProcessor.ProcessErrorAsync += ErrorHandler;
            await UserRegisterProcessor.StartProcessingAsync();

            var markChatMessageAsReadProcessor = client.CreateProcessor(_markChatMessageAsReadQueueName);
            markChatMessageAsReadProcessor.ProcessMessageAsync += OnMarkChatMessageAsReadReceived;
            markChatMessageAsReadProcessor.ProcessErrorAsync += ErrorHandler;
            await markChatMessageAsReadProcessor.StartProcessingAsync();

            var newOnlineUserQueueProcessor = client.CreateProcessor(_newOnlineUserQueueName);
            newOnlineUserQueueProcessor.ProcessMessageAsync += OnNewOnlineUserQueueReceived;
            newOnlineUserQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await newOnlineUserQueueProcessor.StartProcessingAsync();


            var newOfflineUserTopicProcessor = client.CreateProcessor(_newOfflineUserTopicName, _newOfflineUserTopicChatSubName);
            newOfflineUserTopicProcessor.ProcessMessageAsync += OnNewOfflineUserTopicReceived;
            newOfflineUserTopicProcessor.ProcessErrorAsync += ErrorHandler;
            await newOfflineUserTopicProcessor.StartProcessingAsync();

            //testc 
            //var newOfflineUserTopicProcessor = client.CreateProcessor(_newOfflineUserTopicName, _newOfflineUserTopicChatSubName);
            //newOfflineUserTopicProcessor.ProcessMessageAsync += OnNewOfflineUserTopicReceived;
            //newOfflineUserTopicProcessor.ProcessErrorAsync += ErrorHandler;
            //await newOfflineUserTopicProcessor.StartProcessingAsync();


            return;
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        /*
         * można zrobić tak że wszystkie procesory dodają sie do tej samej medoty i poniej dobiero w zależności od rodzaju będa inne handlery j
         * tak jak w openAi
         * 
         */

        private async Task OnNewOfflineUserTopicReceived(ProcessMessageEventArgs args)//OnNewOnline CHAT UserQueueReceived
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newOfflineUserEvent = JsonSerializer.Deserialize<NewOfflineUserEvent>(body);

            if (newOfflineUserEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }
            //////

            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            //if (userEmail is null || userId is null)
            //{
            //    return BadRequest("User cannot be identified.");
            //}
            IEnumerable<FriendInvitation> friendsInvitation;
            using (var unitOfWork = new UnitOfWork(new ApplicationDbContext(_dbContextOptions)))
            {
                friendsInvitation = await unitOfWork.FriendInvitationRepository.GetAllFriends(newOfflineUserEvent.User.UserId);
            }
            if (friendsInvitation == null) { throw new ArgumentNullException("TODO"); }

            var friendsInvitationDtos = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<FriendInvitationDto>>(friendsInvitation);
            //
            var users = friendsInvitationDtos.Select(x => x.InviterUserId == newOfflineUserEvent.User.UserId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
            
            var newOfflineUserWithFriendsEvent = new NewOfflineUserWithFriendsEvent() { UserChatFriends = users, User = new SimpleUser() { UserEmail = newOfflineUserEvent.User.UserEmail, UserId = newOfflineUserEvent.User.UserId } };
            await _messageBus.PublishMessage(newOfflineUserWithFriendsEvent, "new-offline-user-with-friends-queue");


            ///////

            //List<Task<bool>> listOfOnlineUsers = new();
            //foreach (var item in newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends)
            //{
            //    listOfOnlineUsers.Add(_redisDb.KeyExistsAsync($"presence-{item.UserId}"));
            //}
            //var resoult = await Task.WhenAll(listOfOnlineUsers);

            //List<SimpleUser> onlineUsers = new();
            //for (int i = 0; i < newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.Count(); i++)
            //{
            //    if (resoult[i])
            //    {
            //        onlineUsers.Add(newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.ElementAt(i));
            //    }
            //}

            //await _messagesHubContext.Clients.Users(newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.Select(x => x.UserId)).SendAsync("UserIsOnline", newOnlineUserWithFriendsEvent.NewOnlineUser);
            //await _messagesHubContext.Clients.User(newOnlineUserWithFriendsEvent.NewOnlineUser.UserId).SendAsync("GetOnlineUsers", onlineUsers);
            //await _messagesHubContext.Clients.All.SendAsync("GetOnlineUsers", "sdfsdfsdfs");

            //await _presenceHubContext.Clients.User(sendMessageToSignalREvent.RecipientId).SendAsync("NewMessageReceived", new { senderEmail = sendMessageToSignalREvent.SenderEmail });

            await args.CompleteMessageAsync(args.Message);
        }


        private async Task OnMarkChatMessageAsReadReceived(ProcessMessageEventArgs args)
        {
            var busMessage = args.Message;
            var body = Encoding.UTF8.GetString(busMessage.Body);

            var newMarkChatMessageAsReadEvent = JsonSerializer.Deserialize<MarkChatMessageAsReadEvent>(body);

            if (newMarkChatMessageAsReadEvent is null)
            {
                throw new ArgumentNullException("newMarkChatMessageAsReadEvent is empty");
            }

            using (var unitOfWork = new UnitOfWork(new ApplicationDbContext(_dbContextOptions)))
            {
                var message = await unitOfWork.MessageRepository.GetOneAsync(x => x.Id == newMarkChatMessageAsReadEvent.Id);
                
                if (message is null)
                    throw new ArgumentNullException($"I can't find the chat message from event:{newMarkChatMessageAsReadEvent}");

                message.DateRead=newMarkChatMessageAsReadEvent.DateRead;

                if (!unitOfWork.HasChanges())
                    throw new Exception("Failed to mark as read");
                if (!await unitOfWork.Complete())
                    throw new Exception("Failed to mark as read");
            }

            await args.CompleteMessageAsync(args.Message);

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
                Email = newUserRegisterCreateUser.Email,
                PhotoUrl = newUserRegisterCreateUser.PhotoUrl,
            };

            await userRepositorySingleton.AddUser(user);
            await args.CompleteMessageAsync(args.Message);

        }

        private async Task OnNewOnlineUserQueueReceived(ProcessMessageEventArgs args)//OnNewOnline CHAT UserQueueReceived
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newOnlineUserEvent = JsonSerializer.Deserialize<NewOnlineUserEvent>(body);

            if (newOnlineUserEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }
            //////

            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            //if (userEmail is null || userId is null)
            //{
            //    return BadRequest("User cannot be identified.");
            //}
            IEnumerable<FriendInvitation> friendsInvitation;
            using (var unitOfWork = new UnitOfWork(new ApplicationDbContext(_dbContextOptions)))
            {
                friendsInvitation = await unitOfWork.FriendInvitationRepository.GetAllFriends(newOnlineUserEvent.NewOnlineUser.UserId);
            }
            if (friendsInvitation == null) { throw new ArgumentNullException("TODO"); }

            var friendsInvitationDtos = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<FriendInvitationDto>>(friendsInvitation);
            //
            var onlineUsers = friendsInvitationDtos.Select(x => x.InviterUserId == newOnlineUserEvent.NewOnlineUser.UserId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
            var newOnlineUserWithFriendsEvent = new NewOnlineUserWithFriendsEvent() { NewOnlineUserChatFriends = onlineUsers, NewOnlineUser = new SimpleUser() { UserEmail = newOnlineUserEvent.NewOnlineUser.UserEmail, UserId = newOnlineUserEvent.NewOnlineUser.UserId } };
            await _messageBus.PublishMessage(newOnlineUserWithFriendsEvent, "new-online-user-with-friends-queue");


            ///////

            //List<Task<bool>> listOfOnlineUsers = new();
            //foreach (var item in newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends)
            //{
            //    listOfOnlineUsers.Add(_redisDb.KeyExistsAsync($"presence-{item.UserId}"));
            //}
            //var resoult = await Task.WhenAll(listOfOnlineUsers);

            //List<SimpleUser> onlineUsers = new();
            //for (int i = 0; i < newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.Count(); i++)
            //{
            //    if (resoult[i])
            //    {
            //        onlineUsers.Add(newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.ElementAt(i));
            //    }
            //}

            //await _messagesHubContext.Clients.Users(newOnlineUserWithFriendsEvent.NewOnlineUserChatFriends.Select(x => x.UserId)).SendAsync("UserIsOnline", newOnlineUserWithFriendsEvent.NewOnlineUser);
            //await _messagesHubContext.Clients.User(newOnlineUserWithFriendsEvent.NewOnlineUser.UserId).SendAsync("GetOnlineUsers", onlineUsers);
            //await _messagesHubContext.Clients.All.SendAsync("GetOnlineUsers", "sdfsdfsdfs");

            //await _presenceHubContext.Clients.User(sendMessageToSignalREvent.RecipientId).SendAsync("NewMessageReceived", new { senderEmail = sendMessageToSignalREvent.SenderEmail });

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
