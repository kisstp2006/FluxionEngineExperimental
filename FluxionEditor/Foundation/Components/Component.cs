using System.Diagnostics;
using System.Runtime.Serialization;

namespace FluxionEditor.Foundation.Components
{
    /// <summary>
    /// Base class for all components that can be attached to a <see cref="GameObject"/>.
    /// </summary>
    [DataContract]
    public class Component : ViewModelBase
    {
        // ── Owner ──

        public GameObject Owner { get; private set; } = null!;

        // ── State ──

        [DataMember]
        public bool IsEnabled { get; set; } = true;

        // ── Constructors ──

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
