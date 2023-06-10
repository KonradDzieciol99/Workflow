using EmailSender.Views.Emails;
using FluentEmail.Core;
using MessageBus.Events;
using System.Net;

namespace EmailSender
{
    public class Sender : ISender
    {
        private readonly string _verifyEmailUrl;
        private readonly IFluentEmail _fluentEmail;

        public Sender(IFluentEmail fluentEmail, IConfiguration configuration)
        {
            _verifyEmailUrl = configuration["VerifyEmailUrl"];
            this._fluentEmail = fluentEmail;
        }
        public async Task CreateConfirmEmailMessage(NewUserRegistrationEvent registerEmailBusMessage)
        {

            ///////////////////WebUtility.UrlEncode!!!!!!!!!!
            var url = $"{_verifyEmailUrl}?token={WebUtility.UrlEncode(registerEmailBusMessage.Token)}&email={registerEmailBusMessage.UserEmail}";

            var verifyEmailModel = new VerifyEmail() { Url = url };

            var currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);

            var email = _fluentEmail.To(registerEmailBusMessage.UserEmail)
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
