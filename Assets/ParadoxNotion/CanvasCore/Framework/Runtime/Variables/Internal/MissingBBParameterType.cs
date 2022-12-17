using ParadoxNotion.Serialization;
using UnityEngine;

namespace NodeCanvas.Framework.Internal
{
    public class MissingBBParameterType : BBParameter<object>, IMissingRecoverable
    {
        [SerializeField] private string _missingType;

        [SerializeField] private string _recoveryState;

        string IMissingRecoverable.missingType
        {
            get => _missingType;
            set => _missingType = value;
        }

        string IMissingRecoverable.recoveryState
        {
            get => _recoveryState;
            set => _recoveryState = value;
        }
    }
}