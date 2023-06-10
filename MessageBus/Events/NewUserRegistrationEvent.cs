namespace MessageBus.Events
{
    public class NewUserRegistrationEvent : IntegrationEvent
    {
        public string UserEmail { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
