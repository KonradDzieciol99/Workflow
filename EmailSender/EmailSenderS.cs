using EmailSender.Views.Emails;
using FluentEmail.Core;
using MessageBus.Events;
using System.Net;

namespace EmailSender
{
    public class EmailSenderS : IEmailSender
    {
        private readonly IFluentEmailFactory _fluentEmailFactory;
        private readonly string _verifyEmailUrl;
        private readonly string _from;

        public EmailSenderS(IFluentEmailFactory fluentEmailFactory, string verifyEmailUrl,string from)
        {
            _fluentEmailFactory = fluentEmailFactory;
            _verifyEmailUrl = verifyEmailUrl;
            this._from = from;
        }
        public async Task SendConfirmEmailMessage(NewUserRegistrationEvent registerEmailBusMessage)
        {


            IFluentEmail _fluentEmail = _fluentEmailFactory.Create();
            ///////////////////WebUtility.UrlEncode!!!!!!!!!!
            var url = $"{_verifyEmailUrl}?token={WebUtility.UrlEncode(registerEmailBusMessage.Token)}&email={registerEmailBusMessage.UserEmail}";

            var verifyEmailModel = new VerifyEmail() { Url = url };

            var currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);

            var email = _fluentEmail.To(registerEmailBusMessage.UserEmail)
                                    .SetFrom(_from)
                                    .Tag("TEST")
                                    .Subject("Workflow Email verification")
                                    .UsingTemplateFromFile($@"{currentLocation}/Views/Emails/VerifyEmail.cshtml", verifyEmailModel);
            await Send(email);

            return;
        }
        private async Task Send(IFluentEmail email)
        {
            var resoult = await email.SendAsync();
            if (!resoult.Successful)
            {
                throw new Exception("Can't send a Email");
            }
            return;
        }
    }
}
