using System.ComponentModel;
using System.Runtime.Serialization;

namespace FluxionEditor
{
    /// <summary>
    /// Base class for all data models. Implements <see cref="INotifyPropertyChanged"/>
    /// and supports <see cref="DataContractSerializer"/> with circular references.
    /// </summary>
    [DataContract(IsReference = true)]
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the given property name.
        /// </summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
