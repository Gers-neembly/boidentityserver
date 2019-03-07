using FluentValidation;
using Neembly.BOIDServer.WebAPI.Models.DTO;

public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
{
    #region Constructor
    public RegisterDTOValidator()
    {
        RuleFor(x => x.UserName).NotNull().NotEmpty().Length(0, 20);
        RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotNull().NotEmpty().Length(0, 20);
        RuleFor(x => x.ConfirmPassword).NotNull().NotEmpty().Length(0, 20);
        RuleFor(x => x.OperatorId).NotNull().NotEmpty().GreaterThan(0);
    }
    #endregion
}