﻿using EllipticCurve.Utils;
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
        public EmailSender(IFluentEmail fluentEmail, IConfiguration configuration)
        {
            this._fluentEmail = fluentEmail;
            var VerifyEmailUrl = configuration["VerifyEmailUrl"]; ;
        }
        public async Task SendConfirmEmailMessage(RegisterEmailBusMessage registerEmailBusMessage)
        {
            configuration
            var verifyEmailModel = new VerifyEmail() { Token= registerEmailBusMessage.Token,url=""};

            var currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            
            var email = _fluentEmail.To(registerEmailBusMessage.Email)
                                    .Tag("TEST")
                                    .Subject("Workflow Email verification")
                                    .UsingTemplateFromFile($@"{currentLocation}/Views/Emails/PendingOrderEmail.cshtml", verifyEmailModel);

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