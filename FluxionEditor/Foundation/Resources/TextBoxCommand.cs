using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System.Windows.Input;

namespace FluxionEditor.Foundation.Resources
{
    /// <summary>
    /// Attached behavior that wires an <see cref="ICommand"/> to a TextBox's Enter key.
    /// Usage in XAML:
    /// <code>
    /// &lt;TextBox Text="{Binding Name}"
    ///          editor:TextBoxCommand.Command="{Binding RenameCommand}"
    ///          editor:TextBoxCommand.CommandParameter="{Binding Text, RelativeSource={RelativeSource Self}}"/&gt;
    /// </code>
    /// </summary>
    public static class TextBoxCommand
    {
        public static readonly AttachedProperty<ICommand?> CommandProperty =
            AvaloniaProperty.RegisterAttached<TextBox, ICommand?>("Command", typeof(TextBoxCommand));

        public static readonly AttachedProperty<object?> CommandParameterProperty =
            AvaloniaProperty.RegisterAttached<TextBox, object?>("CommandParameter", typeof(TextBoxCommand));

        static TextBoxCommand()
        {
            CommandProperty.Changed.AddClassHandler<TextBox>(OnCommandChanged);
        }

        public static ICommand? GetCommand(TextBox element) => element.GetValue(CommandProperty);
        public static void SetCommand(TextBox element, ICommand? value) => element.SetValue(CommandProperty, value);

        public static object? GetCommandParameter(TextBox element) => element.GetValue(CommandParameterProperty);
        public static void SetCommandParameter(TextBox element, object? value) => element.SetValue(CommandParameterProperty, value);

        private static void OnCommandChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.OldValue is ICommand)
                textBox.KeyDown -= OnKeyDown;

            if (e.NewValue is ICommand)
                textBox.KeyDown += OnKeyDown;
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || sender is not TextBox textBox)
                return;

            var command = GetCommand(textBox);
            var parameter = GetCommandParameter(textBox) ?? textBox.Text;

            if (command?.CanExecute(parameter) == true)
                command.Execute(parameter);

            e.Handled = true;
        }
    }
}
