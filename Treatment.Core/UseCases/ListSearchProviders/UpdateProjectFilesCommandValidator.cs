namespace Treatment.Core.UseCases.ListSearchProviders
{
    using FluentValidation;

    using JetBrains.Annotations;

    [UsedImplicitly]
    public class ListSearchProvidersCommandHandlerValidator : AbstractValidator<ListSearchProvidersCommand>
    {
        public ListSearchProvidersCommandHandlerValidator()
        {
            // no validation required.
            // todo remove this class and make it work with DIP
        }
    }
}