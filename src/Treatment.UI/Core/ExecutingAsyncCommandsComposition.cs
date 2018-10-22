namespace Treatment.UI.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using JetBrains.Annotations;

    using Nito.Mvvm;

    internal class ExecutingAsyncCommandsComposition : IDisposable
    {
        [NotNull]
        private readonly List<CapturingExceptionAsyncCommand> _commands;

        [NotNull]
        private readonly List<Action<bool>> _actions;

        public ExecutingAsyncCommandsComposition()
        {
            _commands = new List<CapturingExceptionAsyncCommand>();
            _actions = new List<Action<bool>>();
        }

        public void WatchCommand(CapturingExceptionAsyncCommand command)
        {
            if (command == null)
                return;

            _commands.Add(command);
            command.PropertyChanged += CommandOnPropertyChanged;
        }

        public void RegisterAction(Action<bool> func)
        {
            if (func == null)
                return;
            _actions.Add(func);
        }

        public void Dispose()
        {
            foreach (var command in _commands)
                command.PropertyChanged -= CommandOnPropertyChanged;

            _commands.Clear();
            _actions.Clear();
        }

        private void CommandOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(CapturingExceptionAsyncCommand.IsExecuting))
                return;

            IsExecutionChanged();
        }

        private void IsExecutionChanged()
        {
            foreach (var cmd in _commands)
                cmd.OnCanExecuteChanged();

            var isExecuting = _commands.Any(x => x.IsExecuting);
            foreach (var action in _actions)
                IgnoreException(() => action.Invoke(isExecuting));

            foreach (var cmd in _commands)
                cmd.OnCanExecuteChanged();
        }

        private static void IgnoreException(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}