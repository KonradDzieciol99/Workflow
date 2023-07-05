namespace EmailSender.Sender;

public interface ISender
{
    Task CreateConfirmEmailMessage(string userEmail, string token, string userId);
}

