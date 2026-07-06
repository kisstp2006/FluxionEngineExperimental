using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using FluxionEditor.Foundation;
using System.Xml.Serialization;

namespace FluxionEditor.Foundation.Utilities
{
    public static class Serializer
    {

        public static void ToFile<T>(T instance, string filePath)
        {
            Debug.Assert(!string.IsNullOrEmpty(filePath), "filePath cannot be null or empty.");
            try
            {
                using var fs = new FileStream(filePath, FileMode.Create);
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, instance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to serialize to '{filePath}': {ex.Message}");
                MessageBox.Error(ex.Message);


                //TODO : Log the exception to a file or logging system
            }

        }

        internal static T FromFile<T>(string filePath)
        {
            try
            {
                using var fs = new FileStream(filePath, FileMode.Open);
                var serializer = new XmlSerializer(typeof(T));
                T instance = (T)serializer.Deserialize(fs);

                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to deserialize '{filePath}': {ex.Message}");
                MessageBox.Error(ex.Message);


                //TODO : Log the exception to a file or logging system
                return default(T);
            }
        }
    }
}
