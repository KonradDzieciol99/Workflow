using EllipticCurve.Utils;
using Email.Common.Models;
using Email.Views.Emails;
using FluentEmail.Core;
using FluentEmail.Razor;
using Microsoft.Extensions.Configuration;

namespace Email
{
    public class EmailSender: IEmailSender
    {
        private readonly IFluentEmailFactory _fluentEmailFactory;
        private readonly string _verifyEmailUrl;

        public EmailSender(IFluentEmailFactory fluentEmailFactory, string verifyEmailUrl)
        {
            this._fluentEmailFactory = fluentEmailFactory;
            this._verifyEmailUrl = verifyEmailUrl;
        }
        public async Task SendConfirmEmailMessage(NewUserRegisterEmail registerEmailBusMessage)
        {


            IFluentEmail _fluentEmail = _fluentEmailFactory.Create();


            var url = $"{this._verifyEmailUrl}?token={registerEmailBusMessage.Token}&email={registerEmailBusMessage.Email}";

            var verifyEmailModel = new VerifyEmail() { Url = url };

            var currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);

            var email = _fluentEmail.To(registerEmailBusMessage.Email)
                                    .Tag("TEST")
                                    .Subject("Workflow Email verification")
                                    .UsingTemplateFromFile($@"{currentLocation}/Views/Emails/VerifyEmail.cshtml", verifyEmailModel);

            await this.Send(email);

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
