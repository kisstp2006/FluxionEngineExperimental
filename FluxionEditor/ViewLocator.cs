using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FluxionEditor.ViewModels;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FluxionEditor
{
    /// <summary>
    /// Resolves a View for a given ViewModel by convention:
    /// "FooViewModel" → "FooView".
    /// </summary>
    [RequiresUnreferencedCode(
        "Default implementation of ViewLocator involves reflection which may be trimmed away.",
        Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
    public class ViewLocator : IDataTemplate
    {
        public Control? Build(object? param)
        {
            if (param is null)
                return null;

            // Replace "ViewModel" with "View" in the type name
            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
                return (Control)Activator.CreateInstance(type)!;

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
