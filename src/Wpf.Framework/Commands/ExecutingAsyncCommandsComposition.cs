namespace Wpf.Framework.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using JetBrains.Annotations;
    using Nito;

    public class ExecutingAsyncCommandsComposition : IDisposable
    {
        [NotNull] private readonly List<CapturingExceptionAsyncCommand> commands;
        [NotNull] private readonly List<Action<bool>> actions;

        public ExecutingAsyncCommandsComposition()
        {
            commands = new List<CapturingExceptionAsyncCommand>();
            actions = new List<Action<bool>>();
        }

        public void WatchCommand(CapturingExceptionAsyncCommand command)
        {
            if (command == null)
                return;

            commands.Add(command);
            command.PropertyChanged += CommandOnPropertyChanged;
        }

        public void RegisterAction(Action<bool> func)
        {
            if (func == null)
                return;
            actions.Add(func);
        }

        public void Dispose()
        {
            foreach (var command in commands)
                command.PropertyChanged -= CommandOnPropertyChanged;

            commands.Clear();
            actions.Clear();
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

        private void CommandOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(CapturingExceptionAsyncCommand.IsExecuting))
                return;

            IsExecutionChanged();
        }

        private void IsExecutionChanged()
        {
            foreach (var cmd in commands)
                cmd.OnCanExecuteChanged();

            var isExecuting = commands.Any(x => x.IsExecuting);

            foreach (var action in actions)
                IgnoreException(() => action.Invoke(isExecuting));

            foreach (var cmd in commands)
                cmd.OnCanExecuteChanged();
        }
    }
}
