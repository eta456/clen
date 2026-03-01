using FluentValidation;
using CleanApi.Api.Dtos;

namespace CleanApi.Api.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StatusName).NotEmpty();
        RuleFor(x => x.TotalAmount).GreaterThan(0);
    }
}

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.NewStatusName).NotEmpty();
    }
}