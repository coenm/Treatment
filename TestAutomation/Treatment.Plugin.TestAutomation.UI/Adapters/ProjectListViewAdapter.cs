namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Reflection;
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

            this.item = item;
            this.eventPublisher = eventPublisher;

            Listview = new ListViewAdapter(
                FieldsHelper.FindFieldInUiElementByName<ListView>(item, nameof(Listview)),
                eventPublisher);
        }

        public bool IsEnabled => item.IsEnabled;

        public double Width => item.Width;

        public double Height => item.Height;

        public ListViewAdapter Listview { get; }
    }
}
