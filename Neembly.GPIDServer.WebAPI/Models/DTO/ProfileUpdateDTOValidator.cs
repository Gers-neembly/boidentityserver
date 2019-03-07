using FluentValidation;
using Neembly.BOIDServer.WebAPI.Models.DTO;

public class ProfileUpdateDTOValidator : AbstractValidator<ProfileUpdateDTO>
{
    #region Constructor
    public ProfileUpdateDTOValidator()
    {
        RuleFor(x => x.BackOfficeUserId).NotNull().NotEmpty().Length(0, 25);
        RuleFor(x => x.BackOfficeUserInfo).NotNull();
    }
    #endregion
}