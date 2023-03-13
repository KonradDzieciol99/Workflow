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
using EmailSender;
using MessageBus.Events;

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

            var registerEmailBusMessage = JsonSerializer.Deserialize<NewUserRegistrationEvent>(body);

            if (registerEmailBusMessage is null)
            {
                throw new ArgumentNullException("registerEmailBusMessage is empty");
            }
            //var c = new EmailSender();
            await _emailSender.SendConfirmEmailMessage(registerEmailBusMessage);
            await args.CompleteMessageAsync(args.Message);

        }
    }
}
