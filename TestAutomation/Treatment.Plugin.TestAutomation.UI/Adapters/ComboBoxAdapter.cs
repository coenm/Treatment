namespace Treatment.Plugin.TestAutomation.UI.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class ComboBoxAdapter : ITestAutomationComboBox, IComboBox
    {
        [NotNull] private readonly ComboBox item;
        [NotNull] private readonly List<IInitializable> helpers;
        [NotNull] private readonly ControlEventPublisher publisher;

        public ComboBoxAdapter([NotNull] ComboBox item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            Guid = Guid.NewGuid();

            publisher = new ControlEventPublisher(this, Guid, eventPublisher);

            helpers = new List<IInitializable>
                      {
                          new PositionChangedHelper(item, c => PositionUpdated?.Invoke(this, c)),
                          new SizeChangedHelper(item, c => SizeUpdated?.Invoke(this, c)),
                          new EnabledChangedHelper(item, c => IsEnabledChanged?.Invoke(this, c)),
                          new KeyboardFocusHelper(item, c => KeyboardFocusChanged?.Invoke(this, c)),
                          new FocusHelper(
                                          item,
                                          c => FocusableChanged?.Invoke(this, c),
                                          c => GotFocus?.Invoke(this, c),
                                          c => LostFocus?.Invoke(this, c)),
                          new DropDownOpenClosedHelper(
                              item,
                              c => DropDownOpened?.Invoke(this, c),
                              c => DropDownClosed?.Invoke(this, c)),
                          new SelectorSelectionChangedHelper(item, c => SelectionChanged?.Invoke(this, c)),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(IComboBox));
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<FocusableChanged> FocusableChanged;

        public event EventHandler<GotFocus> GotFocus;

        public event EventHandler<LostFocus> LostFocus;

        public event EventHandler<KeyboardFocusChanged> KeyboardFocusChanged;

        public event EventHandler<DropDownOpened> DropDownOpened;

        public event EventHandler<DropDownClosed> DropDownClosed;

        public event EventHandler<SelectionChanged> SelectionChanged;

        public Guid Guid { get; }

        public void Initialize()
        {
            helpers.ForEach(helper => helper.Initialize());
        }

        public void Dispose()
        {
            helpers.ForEach(helper => helper.Dispose());
            publisher.Dispose();
        }
    }
}
