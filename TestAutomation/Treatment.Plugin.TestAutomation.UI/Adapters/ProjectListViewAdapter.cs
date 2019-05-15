namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.UserControls;

    internal class ProjectListViewAdapter : IProjectListView
    {
        [NotNull] private readonly ProjectListView item;
        [NotNull] private readonly IEventPublisher eventPublisher;

        public ProjectListViewAdapter([NotNull] ProjectListView item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            Guid = Guid.NewGuid();

            this.item = item;
            this.eventPublisher = eventPublisher;

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IProjectListView));
        }

        // todo not sure is we need this here.
        public event EventHandler<PositionUpdated> PositionUpdated;

        public Guid Guid { get; }

        public ListViewAdapter Listview { get; private set; }

        public void Dispose()
        {
            Listview?.Dispose();
        }

        public void Initialize()
        {
            Listview = new ListViewAdapter(
                FieldsHelper.FindFieldInUiElementByName<ListView>(item, nameof(Listview)),
                eventPublisher);
            Listview.Initialize();
        }
    }
}
