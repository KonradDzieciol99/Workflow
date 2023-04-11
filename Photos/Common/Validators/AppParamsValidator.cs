using FluentValidation;
using Photos.Models;

namespace Photos.Common.Validators
{
    public class FileUploadValidator : AbstractValidator<IconUploadRequest>
    {
        public FileUploadValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(1).MaximumLength(30);
            RuleFor(x => x.File).NotEmpty().Must(file =>
            {
                const long MB = 1048576;
                const long _HALF_MB = MB/2;

                if (file.Length > _HALF_MB)
                    return false;

                return true;
            }).WithMessage("Photo cannot exceed 0.5MB"); ;

        }
    }
}
