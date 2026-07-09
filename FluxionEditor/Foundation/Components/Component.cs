using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace FluxionEditor.Foundation.Components
{
    /// <summary>
    /// Base class for all components that can be attached to a <see cref="GameObject"/>.
    /// </summary>
    /// 

    interface IMSComponent { }


    [DataContract]
    abstract public class Component : ViewModelBase
    {
        internal abstract IMSComponent GetMultiSelectionComponent(MSObject msObject);


        // ── Owner ──
        [DataMember]
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

    abstract class MSComponent<T> :ViewModelBase, IMSComponent where T : Component 
    {
        private bool _enableUpdates=true;
        public List<T> SelectedComponents { get; private set; }
        protected abstract bool UpdateComponents(string propertyName);
        protected abstract bool UpdateMSComponent();

        public void Refresh()
        {
            // Disable write-back while reading values FROM the components,
            // then re-enable so user edits propagate again (same pattern
            // as MSObject.Refresh).
            _enableUpdates = false;
            UpdateMSComponent();
            _enableUpdates = true;
        }


        public MSComponent(MSObject msObject)
        {
            Debug.Assert(msObject?.selectedGameObjects?.Count > 0);
            SelectedComponents = msObject.selectedGameObjects.Select(gameObject=>gameObject.GetComponent<T>()).ToList();

            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); };
        }
    
    }

}
