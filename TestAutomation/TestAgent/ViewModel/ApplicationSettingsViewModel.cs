namespace TestAgent.ViewModel
{
    using JetBrains.Annotations;
    using TestAgent.Model.Configuration;
    using Treatment.Helpers.Guards;
    using Wpf.Framework.EntityEditor.ViewModel;
    using Wpf.Framework.ViewModel;

    public class ApplicationSettingsViewModel : ViewModelBase, IEntityEditorViewModel<TestAgentApplicationSettings>
    {
        [CanBeNull] private TestAgentApplicationSettings entity;

        [UsedImplicitly]
        public ApplicationSettingsViewModel()
        {
        }

        public string Executable
        {
            get => Properties.Get(string.Empty);
            set => Properties.Set(value);
        }

        public void Initialize(TestAgentApplicationSettings testAgentApplicationSettings)
        {
            Guard.NotNull(testAgentApplicationSettings, nameof(testAgentApplicationSettings));
            entity = testAgentApplicationSettings;

            Executable = testAgentApplicationSettings.Executable;
        }

        public void SaveToEntity()
        {
            if (entity == null)
                return;

            entity.Executable = Executable;
        }
    }
}
