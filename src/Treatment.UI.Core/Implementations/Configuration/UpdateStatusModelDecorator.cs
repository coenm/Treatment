namespace Treatment.UI.Core.Implementations.Configuration
{
    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.UI.Core.Model;

    [UsedImplicitly]
    internal class UpdateStatusModelDecorator : IConfigFilenameProvider
    {
        [NotNull] private readonly IConfigFilenameProvider decoratee;
        [NotNull] private readonly IStatusFullModel statusModel;

        public UpdateStatusModelDecorator(
            [NotNull] IConfigFilenameProvider decoratee,
            [NotNull] IStatusFullModel statusModel)
        {
            Guard.NotNull(decoratee, nameof(decoratee));
            Guard.NotNull(statusModel, nameof(statusModel));
            this.decoratee = decoratee;
            this.statusModel = statusModel;
        }

        public string Filename
        {
            get
            {
                var value = decoratee.Filename;
                statusModel.SetConfigFilename(value);
                return value;
            }
        }
    }
}
