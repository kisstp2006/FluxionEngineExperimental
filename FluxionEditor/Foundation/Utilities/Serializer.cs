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
            try
            {
                using var fs = new FileStream(filePath, FileMode.Create);
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, instance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Error(ex.Message);


                //TODO : Log the exception to a file or logging system
            }

        }
    }
}
