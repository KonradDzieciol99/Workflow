using Microsoft.AspNetCore.Authorization;

namespace Projects.Common
{
    public class AuthorizationFilter<T> : IEndpointFilter where T : class
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthorizationFilter(IAuthorizationService authorizationService)
        {
            this._authorizationService = authorizationService;
        }
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {

            //context.Arguments.SingleOrDefault(x=>x.)
            //var authorizationResult = await _authorizationService.AuthorizeAsync(User, blogPost, "UserIsAuthorPolicy");

            //var forValidation = context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T)) as T;

            //if (forValidation is null)
            //{
            //    return Results.BadRequest();
            //}

            //var validationResult = await _validator.ValidateAsync(forValidation);

            //if (!validationResult.IsValid)
            //{
            //    return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            //}

            var resoult = await next(context);
            //after enpointcall
            return resoult;
        }
    }
}
