namespace Treatment.Core.UseCases.UpdateProjectFiles
{
    using System;

    using FluentValidation;

    using JetBrains.Annotations;

    [UsedImplicitly]
    public class UpdateProjectFilesCommandValidator : AbstractValidator<UpdateProjectFilesCommand>
    {
        public UpdateProjectFilesCommandValidator()
        {
            // Check if it is a valid path, not if the path exists.
            RuleFor(x => x.Directory)
                .Must(BeValidPath)
                .WithMessage("Directory is not a valid path.");
        }

        // taken from stackoverflow.com 422090
        private static bool BeValidPath(string path)
        {
            var isValidUri = Uri.TryCreate(path, UriKind.Absolute, out var pathUri);
            return isValidUri && pathUri != null && pathUri.IsLoopback && pathUri.IsFile;
        }
    }
}