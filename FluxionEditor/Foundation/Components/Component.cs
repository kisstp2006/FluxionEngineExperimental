using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace FluxionEditor.Foundation.Components
{
    [DataContract]
    public class Component : ViewModelBase
    {
        public GameObject Owner { get; private set; }

        [DataMember]
        public bool IsEnabled { get; set; } = true;

        /// <summary>Parameterless constructor required by DataContractSerializer.</summary>
        public Component()
        {
        }

        public Component(GameObject owner)
        {
            Debug.Assert(owner != null, "Owner cannot be null");
            Owner = owner;
        }
    }
}
