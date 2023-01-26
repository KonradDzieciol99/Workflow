using EllipticCurve.Utils;
using Email.Common.Models;
using Email.Views.Emails;
using FluentEmail.Core;
using Microsoft.Extensions.Configuration;

namespace Email
{
    public class EmailSender: IEmailSender
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly string _from;
        private readonly string _verifyEmailUrl;
        public EmailSender(IFluentEmail fluentEmail, IConfiguration configuration)
        {
            this._fluentEmail = fluentEmail;
            var _verifyEmailUrl = configuration["VerifyEmailUrl"];
        }
        public async Task SendConfirmEmailMessage(RegisterEmailBusMessage registerEmailBusMessage)
        {
            var url = $"{this._verifyEmailUrl}?token={registerEmailBusMessage.Token}&email={registerEmailBusMessage.Email}";

            var verifyEmailModel = new VerifyEmail() { Url=url};

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
