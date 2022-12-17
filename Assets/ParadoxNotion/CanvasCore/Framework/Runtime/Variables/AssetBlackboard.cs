using System;
using System.Collections.Generic;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Serialization;
using ParadoxNotion.Services;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeCanvas.Framework
{
    [CreateAssetMenu(menuName = "ParadoxNotion/CanvasCore/Blackboard Asset")]
    public class AssetBlackboard : ScriptableObject, ISerializationCallbackReceiver, IGlobalBlackboard
    {
        [SerializeField] private string _serializedBlackboard;
        [SerializeField] private List<Object> _objectReferences;
        [SerializeField] private string _UID = Guid.NewGuid().ToString();
        [NonSerialized] private BlackboardSource _blackboard = new();


        //...
        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= PlayModeChange;
            EditorApplication.playModeStateChanged += PlayModeChange;
#endif
            if (Threader.applicationIsPlaying)
            {
                this.InitializePropertiesBinding(null, false);
#if UNITY_EDITOR
                bindingInit = true;
#endif
            }
        }

        private void OnValidate()
        {
            identifier = name;
        }

        public event Action<Variable> onVariableAdded;
        public event Action<Variable> onVariableRemoved;

        ///----------------------------------------------------------------------------------------------

        Dictionary<string, Variable> IBlackboard.variables
        {
            get => _blackboard.variables;
            set => _blackboard.variables = value;
        }

        Object IBlackboard.unityContextObject => this;
        IBlackboard IBlackboard.parent => null;
        Component IBlackboard.propertiesBindTarget => null;
        string IBlackboard.independantVariablesFieldName => null;

        void IBlackboard.TryInvokeOnVariableAdded(Variable variable)
        {
            if (onVariableAdded != null) onVariableAdded(variable);
        }

        void IBlackboard.TryInvokeOnVariableRemoved(Variable variable)
        {
            if (onVariableRemoved != null) onVariableRemoved(variable);
        }

        [field: NonSerialized] public string identifier { get; private set; }

        public string UID => _UID;

        ///----------------------------------------------------------------------------------------------
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            SelfSerialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            SelfDeserialize();
        }

        ///----------------------------------------------------------------------------------------------
        private void SelfSerialize()
        {
            _objectReferences = new List<Object>();
            _serializedBlackboard = JSONSerializer.Serialize(typeof(BlackboardSource), _blackboard, _objectReferences);
        }

        private void SelfDeserialize()
        {
            _blackboard = JSONSerializer.Deserialize<BlackboardSource>(_serializedBlackboard, _objectReferences);
            if (_blackboard == null) _blackboard = new BlackboardSource();
        }

        [ContextMenu("Show Json")]
        private void ShowJson()
        {
            JSONSerializer.ShowData(_serializedBlackboard, name);
        }

        public override string ToString()
        {
            return identifier;
        }

        ///----------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private string tempJson;

        private List<Object> tempObjects;
        private bool bindingInit;

        //...
        private void PlayModeChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                tempJson = _serializedBlackboard;
                tempObjects = _objectReferences;
                if (!bindingInit) this.InitializePropertiesBinding(null, false);
            }

            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                _serializedBlackboard = tempJson;
                _objectReferences = tempObjects;
                bindingInit = false;
                SelfDeserialize();
            }
        }
#endif
    }
}