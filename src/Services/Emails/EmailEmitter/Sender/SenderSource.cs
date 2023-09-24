using EmailEmitter.Views.Emails;
using FluentEmail.Core;
using System.Net;

namespace EmailEmitter.Sender;

public class SenderSource : ISenderSource
{
    private readonly string _verifyEmailUrl;
    private readonly IFluentEmail _fluentEmail;

    public SenderSource(IFluentEmail fluentEmail, IConfiguration configuration)
    {
        _verifyEmailUrl = configuration["VerifyEmailUrl"];
        _fluentEmail = fluentEmail;
    }

    public async Task CreateConfirmEmailMessage(string userEmail, string token, string userId)
    {
        var url = $"{_verifyEmailUrl}?token={WebUtility.UrlEncode(token)}&email={userEmail}";

        var verifyEmailModel = new VerifyEmail() { Url = url };

        var currentLocation = Path.GetDirectoryName(
            System.Reflection.Assembly.GetEntryAssembly()?.Location
        );

        var email = _fluentEmail
            .To(userEmail)
            .Tag("TEST")
            .Subject("Workflow Email verification")
            .UsingTemplateFromFile(
                $@"{currentLocation}/Views/Emails/VerifyEmail.cshtml",
                verifyEmailModel
            );

        await Send(email);

        return;
    }

    private async Task Send(IFluentEmail email)
    {
        var resoult = await email.SendAsync();
        if (!resoult.Successful)
            throw new Exception("Can't send a Email");

        return;
    }
}
