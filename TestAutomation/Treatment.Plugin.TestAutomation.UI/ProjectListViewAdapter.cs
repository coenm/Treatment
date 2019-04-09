namespace Treatment.Plugin.TestAutomation.UI
{
    using Helpers.Guards;
    using JetBrains.Annotations;
    using Treatment.TestAutomation.Contract.Interfaces.Treatment;
    using Treatment.UI.UserControls;

    internal class ProjectListViewAdapter : IProjectListView
    {
        [NotNull] private readonly ProjectListView item;

        public ProjectListViewAdapter([NotNull] ProjectListView item)
        {
            Guard.NotNull(item, nameof(item));
            this.item = item;
        }

        public bool IsEnabled => item.IsEnabled;

        public double Width => item.Width;

        public double Height => item.Height;
    }
}