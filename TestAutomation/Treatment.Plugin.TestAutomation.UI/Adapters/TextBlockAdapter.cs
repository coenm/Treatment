namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.TextBlockControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    internal class TextBlockAdapter : ITestAutomationTextBlock, ITextBlock
    {
        [NotNull] private readonly List<IInitializable> helpers;

        public TextBlockAdapter([NotNull] TextBlock item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            Guid = Guid.NewGuid();

            helpers = new List<IInitializable>(4)
                      {
                          new PositionChangedHelper(item, c => PositionUpdated?.Invoke(this, c)),
                          new SizeChangedHelper(item, eventPublisher, Guid),
                          new EnabledChangedHelper(item, c => IsEnabledChanged?.Invoke(this, c)),
                          new TextBlockTextValueChangedHelper(item, eventPublisher, Guid),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(ITextBlock));
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<TextValueChanged> TextValueChanged;

        public Guid Guid { get; }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
        }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());
        }
    }
}
