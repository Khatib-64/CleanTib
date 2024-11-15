namespace CleanTib.Application.Demo;

public class DemooCommand : ICommand<string>
{
    public string? TestStringTrim { get; set; }
}

public class DemooCommandValidator : AbstractValidator<DemooCommand>
{
    public DemooCommandValidator()
    {
        RuleFor(x => x.TestStringTrim)
            .NotEmpty()
            .NotNull()
            .WithMessage("TestStringTrim must not be null");
    }

}

public class DemooCommandHandler : ICommandHandler<DemooCommand, string>
{
    public async Task<string> Handle(DemooCommand command, CancellationToken cancellationToken)
    {
        return await Task.FromResult(command.TestStringTrim);
    }
}