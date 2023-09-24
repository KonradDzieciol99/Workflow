using AutoMapper;
using Chat.Application.Common.Authorization;
using Chat.Application.Common.Authorization.Requirements;
using Chat.Application.Common.Models;
using Chat.Infrastructure.Repositories;
using Chat.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Chat.Application.Messages.Queries;

public record GetMessageThreadQuery(string RecipientEmail, string RecipientId, int Skip, int Take)
    : IAuthorizationRequest<List<MessageDto>>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>()
        {
            new ShareFriendRequestRequirement(RecipientId)
        };
    }
}

public class GetMessageThreadQueryHandler : IRequestHandler<GetMessageThreadQuery, List<MessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetMessageThreadQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper
    )
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this._currentUserService =
            currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        this._mapper = mapper;
    }

    public async Task<List<MessageDto>> Handle(
        GetMessageThreadQuery request,
        CancellationToken cancellationToken
    )
    {
        var messages = await _unitOfWork.MessagesRepository.GetMessageThreadAsync(
            _currentUserService.GetUserEmail(),
            request.RecipientEmail,
            request.Skip,
            request.Take
        );

        var unreadMessages = messages
            .Where(
                m => m.DateRead == null && m.RecipientEmail == _currentUserService.GetUserEmail()
            )
            .ToList();

        if (unreadMessages.Any())
            foreach (var message in unreadMessages)
                message.MarkMessageAsRead();

        if (_unitOfWork.HasChanges())
            await _unitOfWork.Complete();

        return _mapper.Map<List<MessageDto>>(messages);
    }
}
