using FluentValidation;

namespace Neembly.BOIDServer.WebAPI.Models.DTO
{
    public class LoginDTOValidator : AbstractValidator<LoginDTO>
    {
        #region Constructor
        public LoginDTOValidator()
        {
            RuleFor(x => x.Password).NotNull().NotEmpty().Length(0, 20);
            RuleFor(x => x.Email).NotNull().NotEmpty().Length(0, 80);
            RuleFor(x => x.OperatorId).NotNull().NotEmpty().GreaterThan(0);
        }
        #endregion
    }
}

