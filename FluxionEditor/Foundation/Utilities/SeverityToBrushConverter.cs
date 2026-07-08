using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Converts <see cref="SeverityLevel"/> to an Avalonia <see cref="IBrush"/> for log line coloring.
    /// </summary>
    public class SeverityToBrushConverter : IValueConverter
    {
        public static readonly SeverityToBrushConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not SeverityLevel level)
                return Brushes.Gray;

            return level switch
            {
                SeverityLevel.Error   => Brushes.Red,
                SeverityLevel.Warning => Brushes.Orange,
                SeverityLevel.Info    => Brushes.LightGreen,
                _                     => Brushes.Gray,
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
