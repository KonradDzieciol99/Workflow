using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Chat.Dto;
using Chat.Entity;
using Chat.Persistence;
using Chat.Repositories;
using Mango.MessageBus;
using MediatR;
using MessageBus.Events;
using MessageBus.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Azure.Amqp.Framing;
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMediator _mediator;
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

        public AzureServiceBusConsumer(IServiceScopeFactory serviceScopeFactory, IMediator mediator,IMessageBus messageBus, IMapper mapper,IConfiguration configuration, IUserRepositorySingleton userRepositorySingleton, DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            this._mediator = mediator;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(_serviceBusConnectionString);

            var UserRegisterProcessor = client.CreateProcessor(azureBusTopic, azureBusSubscription);
            UserRegisterProcessor.ProcessMessageAsync += EventHandlerAsync;
            UserRegisterProcessor.ProcessErrorAsync += ErrorHandler;
            await UserRegisterProcessor.StartProcessingAsync();

            var markChatMessageAsReadProcessor = client.CreateProcessor(_markChatMessageAsReadQueueName);
            markChatMessageAsReadProcessor.ProcessMessageAsync += EventHandlerAsync;
            //markChatMessageAsReadProcessor.ProcessMessageAsync += OnMarkChatMessageAsReadReceived;
            markChatMessageAsReadProcessor.ProcessErrorAsync += ErrorHandler;
            await markChatMessageAsReadProcessor.StartProcessingAsync();

            var newOnlineUserQueueProcessor = client.CreateProcessor(_newOnlineUserQueueName);
            newOnlineUserQueueProcessor.ProcessMessageAsync += EventHandlerAsync;
            //newOnlineUserQueueProcessor.ProcessMessageAsync += OnNewOnlineUserQueueReceived;
            newOnlineUserQueueProcessor.ProcessErrorAsync += ErrorHandler;
            await newOnlineUserQueueProcessor.StartProcessingAsync();

            var newOfflineUserTopicProcessor = client.CreateProcessor(_newOfflineUserTopicName, _newOfflineUserTopicChatSubName);
            newOfflineUserTopicProcessor.ProcessMessageAsync += EventHandlerAsync;
            //newOfflineUserTopicProcessor.ProcessMessageAsync += OnNewOfflineUserTopicReceived;
            newOfflineUserTopicProcessor.ProcessErrorAsync += ErrorHandler;
            await newOfflineUserTopicProcessor.StartProcessingAsync();

            return;
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }


        private async Task EventHandlerAsync(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var label = args.Message.ApplicationProperties["Label"] as string;
            if (label == null)
            {
                throw new ArgumentNullException($"Label is empty: {args}");
            }

            if (label == azureBusTopic)
            {
                await SendAsync<NewUserRegisterCreateUser>(body);
            }
            if (label == _markChatMessageAsReadQueueName)
            {
                await SendAsync<MarkChatMessageAsReadEvent>(body);
            }
            if (label == _newOnlineUserQueueName)
            {
               await SendAsync<NewOnlineUserEvent>(body);
            }
            if (label == _newOfflineUserTopicName)
            {
                await SendAsync<NewOfflineUserEvent>(body);
            }

            await args.CompleteMessageAsync(args.Message);

            return;
        }
        private async Task OnNewOfflineUserTopicReceived(ProcessMessageEventArgs args)//OnNewOnline CHAT UserQueueReceived
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newOfflineUserEvent = JsonSerializer.Deserialize<NewOfflineUserEvent>(body);

            if (newOfflineUserEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }

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
        private async Task OnNewOnlineUserQueueReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var newOnlineUserEvent = JsonSerializer.Deserialize<NewOnlineUserEvent>(body);

            if (newOnlineUserEvent is null)
            {
                throw new ArgumentNullException("Message is empty");
            }

            IEnumerable<FriendInvitation> friendsInvitation;
            using (var unitOfWork = new UnitOfWork(new ApplicationDbContext(_dbContextOptions)))
            {
                friendsInvitation = await unitOfWork.FriendInvitationRepository.GetAllFriends(newOnlineUserEvent.NewOnlineUser.UserId);
            }
            if (friendsInvitation == null) { throw new ArgumentNullException("TODO"); }

            var friendsInvitationDtos = _mapper.Map<IEnumerable<FriendInvitation>, IEnumerable<FriendInvitationDto>>(friendsInvitation);
            
            var onlineUsers = friendsInvitationDtos.Select(x => x.InviterUserId == newOnlineUserEvent.NewOnlineUser.UserId ? new SimpleUser() { UserId = x.InvitedUserId, UserEmail = x.InvitedUserEmail } : new SimpleUser() { UserId = x.InviterUserId, UserEmail = x.InviterUserEmail });
            var newOnlineUserWithFriendsEvent = new NewOnlineUserWithFriendsEvent() { NewOnlineUserChatFriends = onlineUsers, NewOnlineUser = new SimpleUser() { UserEmail = newOnlineUserEvent.NewOnlineUser.UserEmail, UserId = newOnlineUserEvent.NewOnlineUser.UserId } };
            await _messageBus.PublishMessage(newOnlineUserWithFriendsEvent, "new-online-user-with-friends-queue");

            await args.CompleteMessageAsync(args.Message);
        }
        private async Task SendAsync<T>(string eventJSON)
        {
            var decodedEvent = JsonSerializer.Deserialize<T>(eventJSON);

            if (decodedEvent is null) { throw new ArgumentNullException($"Message is empty{decodedEvent}"); }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var response = await mediator.Send(decodedEvent);
            }
            return;
        }
    }

}
