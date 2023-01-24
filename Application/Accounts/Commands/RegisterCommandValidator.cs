using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(v => v.Email).NotNull().MinimumLength(6).EmailAddress();
            RuleFor(v => v.Password).NotNull().MinimumLength(6);
        }
    }
}
