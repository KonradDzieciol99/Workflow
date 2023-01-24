using Domain.Identity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator(UserManager<AppUser> _userManager)
        {
            RuleFor(v => v.Email)
                .NotNull()
                .MinimumLength(6)
                .EmailAddress();
                
            RuleFor(v => v.Email).MustAsync(async(email, cancellation) =>
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is null){ return false; }
                return true;
            }).WithMessage("Email is Taken");

            RuleFor(v => v.Password).NotNull().MinimumLength(6);
        }
    }
}
