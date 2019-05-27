namespace Treatment.Core.UseCases.CleanAppConfig
{
    using FluentValidation;

    using JetBrains.Annotations;

    using Treatment.Contract.Commands;
    using Treatment.Helpers.File;

    [UsedImplicitly]
    public class CleanAppConfigCommandHandlerValidator : AbstractValidator<CleanAppConfigCommand>
    {
        public CleanAppConfigCommandHandlerValidator()
        {
            // Check if it is a valid path, not if the path exists.
            RuleFor(x => x.Directory)
                .Must(FileHelper.IsAbsoluteValidPath)
                .WithMessage("Directory is not a valid path.");
        }
    }
}
