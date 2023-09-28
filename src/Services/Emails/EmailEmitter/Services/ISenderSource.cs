namespace EmailEmitter.Services;

public interface ISenderSource
{
    Task CreateConfirmEmailMessage(string userEmail, string token, string userId);
}
