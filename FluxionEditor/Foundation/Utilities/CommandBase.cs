using System;
using System.Windows.Input;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Base class for Avalonia-compatible <see cref="ICommand"/> implementations.
    /// Provides a <see cref="NotifyCanExecuteChanged"/> helper (no WPF dependency).
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public abstract bool CanExecute(object? parameter);
        public abstract void Execute(object? parameter);

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> so the UI re-evaluates
        /// <see cref="CanExecute"/> bindings.
        /// </summary>
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
