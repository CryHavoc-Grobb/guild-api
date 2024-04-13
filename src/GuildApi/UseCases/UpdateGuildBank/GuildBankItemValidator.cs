using FluentValidation;

namespace GuildApi.UseCases.UpdateGuildBank;

internal sealed class UpdateBankRequestValidator : AbstractValidator<UpdateBankRequest>
{
    public UpdateBankRequestValidator()
    {
        RuleFor(r => r.Items)
            .NotNull().NotEmpty()
            .ForEach(r => r.SetValidator(new GuildBankItemValidator()));
    }
}

internal sealed class GuildBankItemValidator : AbstractValidator<GuildBankItem>
{
    public GuildBankItemValidator()
    {
        RuleFor(i => i.Id)
            .GreaterThan(0)
            .WithMessage("Invalid item ID");

        RuleFor(i => i.Name)
            .NotNull().NotEmpty()
            .MaximumLength(75);

        RuleFor(i => i.Count)
            .GreaterThan(0)
            .WithMessage("Invalid quantity");
    }
}