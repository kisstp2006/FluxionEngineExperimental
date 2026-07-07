using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Windows.Input;

namespace FluxionEditor.Foundation.Resources
{
    /// <summary>
    /// Attached behavior that wires <see cref="ICommand"/>s to a TextBox keyboard events.
    /// <list type="bullet">
    ///   <item><b>Command</b> — fires on <b>Enter</b></item>
    ///   <item><b>EscCommand</b> — fires on <b>Escape</b></item>
    /// </list>
    /// Usage:
    /// <code>
    /// &lt;TextBox Classes="EnterCommand"
    ///          Text="{Binding Name}"
    ///          resources:TextBoxCommand.Command="{Binding RenameCommand}"
    ///          resources:TextBoxCommand.EscCommand="{Binding CancelCommand}"/&gt;
    /// </code>
    /// </summary>
    public static class TextBoxCommand
    {
        // ── Enter key ──────────────────────────────────────────

        public static readonly AttachedProperty<ICommand?> CommandProperty =
            AvaloniaProperty.RegisterAttached<TextBox, ICommand?>("Command", typeof(TextBoxCommand));

        public static readonly AttachedProperty<object?> CommandParameterProperty =
            AvaloniaProperty.RegisterAttached<TextBox, object?>("CommandParameter", typeof(TextBoxCommand));

        public static ICommand? GetCommand(TextBox el) => el.GetValue(CommandProperty);
        public static void SetCommand(TextBox el, ICommand? v) => el.SetValue(CommandProperty, v);
        public static object? GetCommandParameter(TextBox el) => el.GetValue(CommandParameterProperty);
        public static void SetCommandParameter(TextBox el, object? v) => el.SetValue(CommandParameterProperty, v);

        // ── Escape key ─────────────────────────────────────────

        public static readonly AttachedProperty<ICommand?> EscCommandProperty =
            AvaloniaProperty.RegisterAttached<TextBox, ICommand?>("EscCommand", typeof(TextBoxCommand));

        public static readonly AttachedProperty<object?> EscCommandParameterProperty =
            AvaloniaProperty.RegisterAttached<TextBox, object?>("EscCommandParameter", typeof(TextBoxCommand));

        public static ICommand? GetEscCommand(TextBox el) => el.GetValue(EscCommandProperty);
        public static void SetEscCommand(TextBox el, ICommand? v) => el.SetValue(EscCommandProperty, v);
        public static object? GetEscCommandParameter(TextBox el) => el.GetValue(EscCommandParameterProperty);
        public static void SetEscCommandParameter(TextBox el, object? v) => el.SetValue(EscCommandParameterProperty, v);

        // ── Wiring ─────────────────────────────────────────────

        private static readonly AttachedProperty<bool> _isWiredProperty =
            AvaloniaProperty.RegisterAttached<TextBox, bool>("_IsWired", typeof(TextBoxCommand));

        static TextBoxCommand()
        {
            CommandProperty.Changed.AddClassHandler<TextBox>(OnCommandChanged);
            EscCommandProperty.Changed.AddClassHandler<TextBox>(OnCommandChanged);
        }

        private static void OnCommandChanged(TextBox tb, AvaloniaPropertyChangedEventArgs e)
        {
            bool hasAny = GetCommand(tb) != null || GetEscCommand(tb) != null;
            bool isWired = tb.GetValue(_isWiredProperty);

            if (hasAny && !isWired)
            {
                tb.SetValue(_isWiredProperty, true);
                tb.KeyDown += OnKeyDown;
            }
            else if (!hasAny && isWired)
            {
                tb.SetValue(_isWiredProperty, false);
                tb.KeyDown -= OnKeyDown;
            }
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            switch (e.Key)
            {
                case Key.Enter:
                    Execute(GetCommand(textBox), GetCommandParameter(textBox) ?? textBox.Text);
                    e.Handled = true;
                    break;

                case Key.Escape:
                    Execute(GetEscCommand(textBox), GetEscCommandParameter(textBox) ?? textBox.Text);
                    e.Handled = true;
                    break;
            }
        }

        private static void Execute(ICommand? command, object? parameter)
        {
            if (command?.CanExecute(parameter) == true)
                command.Execute(parameter);
        }
    }
}
