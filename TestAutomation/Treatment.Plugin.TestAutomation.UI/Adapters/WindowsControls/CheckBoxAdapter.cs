namespace Treatment.Plugin.TestAutomation.UI.Adapters.WindowsControls
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    using JetBrains.Annotations;
    using Treatment.Helpers.Guards;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.CheckBox;
    using Treatment.Plugin.TestAutomation.UI.Adapters.Helpers.FrameworkElementControl;
    using Treatment.Plugin.TestAutomation.UI.Infrastructure;
    using Treatment.Plugin.TestAutomation.UI.Interfaces;
    using Treatment.TestAutomation.Contract.Interfaces.Events.Element;
    using Treatment.TestAutomation.Contract.Interfaces.Framework;

    public class CheckBoxAdapter : ITestAutomationCheckBox, ICheckBox
    {
        [NotNull] private readonly CheckBox item;
        [NotNull] private readonly List<IInitializable> helpers;
        [NotNull] private readonly ControlEventPublisher publisher;

        public CheckBoxAdapter([NotNull] CheckBox item, [NotNull] IEventPublisher eventPublisher)
        {
            Guard.NotNull(item, nameof(item));
            Guard.NotNull(eventPublisher, nameof(eventPublisher));

            this.item = item;
            Guid = Guid.NewGuid();

            publisher = new ControlEventPublisher(this, Guid, eventPublisher);

            helpers = new List<IInitializable>
                      {
                          new LoadedUnLoadedHelper(
                              item,
                              c => OnLoaded?.Invoke(this, c),
                              c => OnUnLoaded?.Invoke(this, c)),
                          new PositionChangedHelper(item, c => PositionUpdated?.Invoke(this, c)),
                          new SizeChangedHelper(item, c => SizeUpdated?.Invoke(this, c)),
                          new EnabledChangedHelper(item, c => IsEnabledChanged?.Invoke(this, c)),
                          new KeyboardFocusHelper(item, c => KeyboardFocusChanged?.Invoke(this, c)),
                          new FocusHelper(
                              item,
                              c => FocusableChanged?.Invoke(this, c),
                              c => GotFocus?.Invoke(this, c),
                              c => LostFocus?.Invoke(this, c)),
                          new CheckBoxOnCheckedChangedHelper(
                              item,
                              c => OnChecked?.Invoke(this, c),
                              c => OnUnChecked?.Invoke(this, c)),
                      };

            eventPublisher.PublishNewControlCreatedAsync(Guid, typeof(ICheckBox));
        }

        public event EventHandler<PositionUpdated> PositionUpdated;

        public event EventHandler<IsEnabledChanged> IsEnabledChanged;

        public event EventHandler<SizeUpdated> SizeUpdated;

        public event EventHandler<FocusableChanged> FocusableChanged;

        public event EventHandler<GotFocus> GotFocus;

        public event EventHandler<LostFocus> LostFocus;

        public event EventHandler<KeyboardFocusChanged> KeyboardFocusChanged;

        public event EventHandler<SelectionChanged> SelectionChanged;

        public event EventHandler<OnChecked> OnChecked;

        public event EventHandler<OnUnChecked> OnUnChecked;

        public event EventHandler<OnLoaded> OnLoaded;

        public event EventHandler<OnUnLoaded> OnUnLoaded;

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
