using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Simple file-based serializer using <see cref="DataContractSerializer"/>.
    /// </summary>
    public static class Serializer
    {
        /// <summary>Writes an object graph to the specified file.</summary>
        public static void ToFile<T>(T instance, string filePath)
        {
            Debug.Assert(!string.IsNullOrEmpty(filePath), "filePath cannot be null or empty.");

            try
            {
                using var fs = new FileStream(filePath, FileMode.Create);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fs, instance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to serialize to '{filePath}': {ex.Message}");
                MessageBox.Error(ex.Message);
                // TODO: Log to file
            }
        }

        /// <summary>Reads an object graph from the specified file.</summary>
        internal static T? FromFile<T>(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                return (T?)serializer.ReadObject(fs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to deserialize '{filePath}': {ex.Message}");
                MessageBox.Error(ex.Message);
                // TODO: Log to file
                return default;
            }
        }
    }
}
