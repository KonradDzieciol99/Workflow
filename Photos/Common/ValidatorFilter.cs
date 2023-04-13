using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace Photos.Common
{
    public class ValidatorFilter<T> : IEndpointFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public ValidatorFilter(IValidator<T> validator)
        {
            _validator = validator;
        }
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {

            var forValidation = context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T)) as T;

            if (forValidation is null)
            {
                return Results.BadRequest();
            }

            var validationResult = await _validator.ValidateAsync(forValidation);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var resoult = await next(context);
            //after enpointcall
            return resoult;
        }
    }
}
