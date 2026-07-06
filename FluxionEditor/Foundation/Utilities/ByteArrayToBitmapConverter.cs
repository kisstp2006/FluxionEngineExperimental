using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Converts a <c>byte[]</c> to an Avalonia <see cref="Bitmap"/> for use in image bindings.
    /// </summary>
    public class ByteArrayToBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
            {
                using var stream = new MemoryStream(bytes);
                return new Bitmap(stream);
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
