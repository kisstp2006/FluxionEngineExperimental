using System;
using System.Windows.Input;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Cross-platform RelayCommand implementation for Avalonia.
    /// Does NOT depend on WPF's CommandManager.RequerySuggested.
    /// Use <see cref="NotifyCanExecuteChanged"/> to manually refresh CanExecute bindings.
    /// </summary>
    public class RelayCommand : CommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Creates a new RelayCommand.
        /// </summary>
        /// <param name="execute">The action to execute when the command is invoked.</param>
        /// <param name="canExecute">Optional predicate that determines whether the command can execute. Defaults to always <c>true</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute"/> is null.</exception>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (() => true);
        }

        /// <inheritdoc />
        public override bool CanExecute(object? parameter)
        {
            return _canExecute();
        }

        /// <inheritdoc />
        public override void Execute(object? parameter)
        {
            _execute();
        }
    }

    /// <summary>
    /// Cross-platform generic RelayCommand implementation for Avalonia.
    /// Does NOT depend on WPF's CommandManager.RequerySuggested.
    /// Use <see cref="NotifyCanExecuteChanged"/> to manually refresh CanExecute bindings.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T?> _execute;
        private readonly Predicate<T?> _canExecute;

        /// <summary>
        /// Creates a new generic RelayCommand.
        /// </summary>
        /// <param name="execute">The action to execute when the command is invoked.</param>
        /// <param name="canExecute">Optional predicate that determines whether the command can execute. Defaults to always <c>true</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute"/> is null.</exception>
        public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (_ => true);
        }

        /// <inheritdoc />
        public override bool CanExecute(object? parameter)
        {
            return _canExecute(parameter is T typedParam ? typedParam : default);
        }

        /// <inheritdoc />
        public override void Execute(object? parameter)
        {
            _execute(parameter is T typedParam ? typedParam : default);
        }
    }
}
