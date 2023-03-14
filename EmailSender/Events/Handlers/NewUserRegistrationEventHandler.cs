using MediatR;
using MessageBus.Events;

namespace EmailSender.Events.Handlers
{
    public class NewUserRegistrationEventHandler : IRequestHandler<NewUserRegistrationEvent>
    {
        private readonly ISender _emailSender;

        public NewUserRegistrationEventHandler(ISender emailSender)
        {
            this._emailSender = emailSender;
        }
        public async Task Handle(NewUserRegistrationEvent request, CancellationToken cancellationToken)
        {
            await _emailSender.CreateConfirmEmailMessage(request);

            await Task.CompletedTask;
            
            return;
        }
    }
}
