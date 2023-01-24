using Core.Interfaces;
using Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.EventHandlers
{
    internal class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly IEmailService _emailService;

        public UserCreatedEventHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            //_emailService.

            return Task.CompletedTask;
        }
    }
}
