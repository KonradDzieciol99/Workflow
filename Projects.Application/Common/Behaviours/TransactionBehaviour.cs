using Azure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Projects.Application.Common.Interfaces;

namespace Projects.Application.Common.Behaviours;
public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventService _integrationEventService;

    public TransactionBehaviour(IUnitOfWork unitOfWork,IIntegrationEventService integrationEventService)
    {
        this._unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(IUnitOfWork));
        this._integrationEventService = integrationEventService ?? throw new ArgumentException(nameof(IIntegrationEventService));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        try
        {
            if (_unitOfWork.HasActiveTransaction())
                return await next();
            

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                response = await next();

                await _unitOfWork.CommitTransactionAsync(transaction);
            }

            await _integrationEventService.PublishEventsThroughEventBusAsync();

            return response;
        }
        catch (Exception ex)
        {
            _unitOfWork.RollbackTransaction(); // ef core zrobi to automatycznie wiec chyba nie potrzebne
            throw;
        }
    }
}
