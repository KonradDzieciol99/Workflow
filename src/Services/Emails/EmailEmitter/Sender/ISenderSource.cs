namespace EmailEmitter.Sender;

public interface ISenderSource
{
    Task CreateConfirmEmailMessage(string userEmail, string token, string userId);
}

