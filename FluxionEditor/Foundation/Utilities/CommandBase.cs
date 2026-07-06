using System;
using System.Windows.Input;

namespace FluxionEditor.Foundation.Utilities
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public abstract bool CanExecute(object? parameter);
        public abstract void Execute(object? parameter);

        /// <summary>Call this when the return value of <see cref="CanExecute"/> might have changed.</summary>
        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
