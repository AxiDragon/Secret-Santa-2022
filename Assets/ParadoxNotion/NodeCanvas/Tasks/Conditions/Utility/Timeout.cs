using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("✫ Utility")]
    [Description("Will return true after a specific amount of time has passed and false while still counting down")]
    public class Timeout : ConditionTask
    {
        private float currentTime;

        public BBParameter<float> timeout = 1f;

        protected override string info => string.Format("Timeout {0}/{1}", currentTime.ToString("0.00"), timeout);

        protected override void OnEnable()
        {
            MonoManager.current.onLateUpdate += MoveNext;
        }

        protected override void OnDisable()
        {
            MonoManager.current.onLateUpdate -= MoveNext;
        }

        private void MoveNext()
        {
            currentTime += Time.deltaTime;
            currentTime = Mathf.Min(currentTime, timeout.value);
        }

        protected override bool OnCheck()
        {
            if (currentTime >= timeout.value)
            {
                currentTime = 0;
                return true;
            }

            return false;
        }
    }
}