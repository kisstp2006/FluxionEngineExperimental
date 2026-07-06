using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace FluxionEditor.Foundation.Components
{
    
    public class Component : ViewModelBase
    {
        public GameObject owner { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public Component(GameObject owner)
        {
            Debug.Assert(owner != null, "Owner cannot be null");
            this.owner = owner;
        }
    }
}
