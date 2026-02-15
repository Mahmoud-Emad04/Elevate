using Elevate.Api.Dtos;
using FluentValidation;

namespace Elevate.Api.Validation;

public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid OrderStatus.");
    }
}
