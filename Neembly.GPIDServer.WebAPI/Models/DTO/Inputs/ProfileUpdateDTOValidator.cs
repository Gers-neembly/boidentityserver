using FluentValidation;
using Neembly.BOIDServer.Constants;

namespace Neembly.BOIDServer.WebAPI.Models.DTO.Inputs
{
    public class ProfileUpdateDTOValidator : AbstractValidator<ProfileUpdateDTO>
    {
        #region Constructor
        public ProfileUpdateDTOValidator()
        {
            RuleFor(x => x.BackOfficeUserId).NotNull().NotEmpty().WithMessage(GlobalConstants.ErrUserAccountNotExisting)
                                            .Length(0, 36);
            RuleFor(x => x.BackOfficeUserInfo).NotNull();
        }
        #endregion
    }
}
