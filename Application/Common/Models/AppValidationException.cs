using FluentValidation.Results;
using System.Runtime.Serialization;

namespace Application.Common.Models
{
    public class AppValidationException:Exception
    {
        public AppValidationException(IEnumerable<ValidationFailure> failures) : base(CreateString(failures))
        {
        }

        private static string CreateString(IEnumerable<ValidationFailure> failures)
        {
           var Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

            return String.Join(Environment.NewLine, Errors.Select(x=> String.Join(" ",x.Value)));
        }
    }
}
