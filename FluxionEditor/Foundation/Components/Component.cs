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
        protected abstract string UpdateComponents(string PropertyName);
        protected abstract string UpdateMSComponent();

        public void Refresh()
        {
            _enableUpdates = true;
            UpdateMSComponent();
            _enableUpdates = false;

        }


        public MSComponent(MSGameObject mSGameObject) 
        {
            Debug.Assert(mSGameObject?.selectedGameObjects?.Count > 0);
            SelectedComponents = mSGameObject.selectedGameObjects.Select(gameObject=>gameObject.GetComponent<T>()).ToList();

            PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); };
        }
    
    }

}
