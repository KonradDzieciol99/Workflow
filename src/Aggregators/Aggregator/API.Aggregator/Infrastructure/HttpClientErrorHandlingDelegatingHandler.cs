//using API.Aggregator.Domain.Commons.Exceptions;
//using Microsoft.AspNetCore.Mvc;
//using System.Net;
//using System.Text.Json;
//using API.Aggregator.Application.Commons.Exceptions;

//namespace API.Aggregator.Infrastructure;

//public class HttpClientErrorHandlingDelegatingHandler : DelegatingHandler
//{
//    public HttpClientErrorHandlingDelegatingHandler() { }

//    protected override async Task<HttpResponseMessage> SendAsync(
//        HttpRequestMessage request,
//        CancellationToken cancellationToken
//    )
//    {
//        var response = await base.SendAsync(request, cancellationToken);

//        if (!response.IsSuccessStatusCode)
//        {
//            var responseString = await response.Content.ReadAsStringAsync();
//            var result = JsonSerializer.Deserialize<ValidationProblemDetails>(
//                responseString,
//                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
//            );

//            throw response.StatusCode switch
//            {
//                HttpStatusCode.BadRequest
//                    => result.Errors is null
//                        ? new AggregatorDomainException(result.Detail, new BadRequestException(""))
//                        : new AggregatorDomainException(
//                            result.Detail,
//                            new ValidationException(result.Errors)
//                        ),
//                HttpStatusCode.Unauthorized
//                    => new AggregatorDomainException(result.Detail, new UnauthorizedException("")),
//                HttpStatusCode.Forbidden
//                    => new AggregatorDomainException(result.Detail, new ForbiddenAccessException()),
//                HttpStatusCode.NotFound
//                    => new AggregatorDomainException(result.Detail, new NotFoundException("")),
//                _ => new HttpRequestException(responseString),
//            };
//        }

//        return response;
//    }
//}
