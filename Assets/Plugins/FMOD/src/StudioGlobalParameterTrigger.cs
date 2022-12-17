using FMOD;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;

namespace FMODUnity
{
    [AddComponentMenu("FMOD Studio/FMOD Studio Global Parameter Trigger")]
    public class StudioGlobalParameterTrigger : EventHandler
    {
        [ParamRef] [FormerlySerializedAs("parameter")]
        public string Parameter;

        public EmitterGameEvent TriggerEvent;

        [FormerlySerializedAs("value")] public float Value;

        private PARAMETER_DESCRIPTION parameterDescription;
        public PARAMETER_DESCRIPTION ParameterDescription => parameterDescription;

        private void Awake()
        {
            if (string.IsNullOrEmpty(parameterDescription.name)) Lookup();
        }

        private RESULT Lookup()
        {
            var result = RuntimeManager.StudioSystem.getParameterDescriptionByName(Parameter, out parameterDescription);
            return result;
        }

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (TriggerEvent == gameEvent) TriggerParameters();
        }

        public void TriggerParameters()
        {
            if (!string.IsNullOrEmpty(Parameter))
            {
                var result = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, Value);
                if (result != RESULT.OK)
                    RuntimeUtils.DebugLogError(string.Format(
                        "[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}", Parameter,
                        result));
            }
        }
    }
}