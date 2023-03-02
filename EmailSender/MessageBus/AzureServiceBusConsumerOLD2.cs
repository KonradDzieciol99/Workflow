using Azure.Messaging.ServiceBus;
using MessageBus.Events;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace EmailSender.MessageBus
{
    public class AzureServiceBusConsumerOLD2 : BackgroundService
    {
        private readonly string serviceBusConnectionString;
        private readonly string NewUserRegisterTopic;
        private readonly string NewUserRegisterSubscription;
        private readonly IConfiguration _configuration;
        //private readonly IMessageBus _messageBus;
        private readonly IEmailSender _emailSender;
        private ServiceBusProcessor UserRegisterProcessor;

        public AzureServiceBusConsumerOLD2(IConfiguration configuration, IEmailSender emailSender)
        {
            _configuration = configuration;
            //_messageBus = messageBus;
            _emailSender = emailSender;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString"); ;
            NewUserRegisterTopic = _configuration.GetValue<string>("NewUserRegisterTopic");
            NewUserRegisterSubscription = _configuration.GetValue<string>("NewUserRegisterSubscription");


            //var test = client.CreateReceiver(NewUserRegisterTopic);
            //var asd =


            //UserRegisterProcessor = client.CreateProcessor(NewUserRegisterTopic, NewUserRegisterSubscription);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = new ServiceBusClient(serviceBusConnectionString);
            UserRegisterProcessor = client.CreateProcessor(NewUserRegisterTopic, NewUserRegisterSubscription);
            UserRegisterProcessor.ProcessMessageAsync += OnUserRegisterReceived;
            UserRegisterProcessor.ProcessErrorAsync += ErrorHandler;
            await UserRegisterProcessor.StartProcessingAsync();

            return;
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

            await _emailSender.SendConfirmEmailMessage(registerEmailBusMessage);
            await args.CompleteMessageAsync(args.Message);

        }
    }
}
