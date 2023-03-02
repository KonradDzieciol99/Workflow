using Chat.Entity;
using Chat.Repositories;
using MediatR;
using MessageBus.Events;

namespace Chat.Events.Handlers
{
    public class NewUserRegisterCreateUserHandler : IRequestHandler<NewUserRegisterCreateUser>
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewUserRegisterCreateUserHandler(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task Handle(NewUserRegisterCreateUser request, CancellationToken cancellationToken)
        {
            var user = new User()
            {
                Id = request.Id,
                Email = request.Email,
                PhotoUrl = request.PhotoUrl,
            };

            _unitOfWork.UserRepository.Add(user);

            if (await _unitOfWork.Complete())
            {
                return;
            }

            throw new Exception("User cannot be created");
            //log

        }
    }
}
