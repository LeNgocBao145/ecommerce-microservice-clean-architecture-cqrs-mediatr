using FluentValidation;

namespace OrderService.Application.Commands.DeleteCart
{
    /// <summary>
    /// Validator for DeleteCartCommand.
    /// Ensures valid user ID for cart deletion.
    /// </summary>
    public class DeleteCartCommandValidator : AbstractValidator<DeleteCartCommand>
    {
        public DeleteCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required.");
        }
    }
}
