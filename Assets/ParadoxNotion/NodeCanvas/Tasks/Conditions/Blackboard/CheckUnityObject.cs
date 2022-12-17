using System;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Object = UnityEngine.Object;

namespace NodeCanvas.Tasks.Conditions
{
    [Category("✫ Blackboard")]
    [Obsolete("Use CheckVariable(T)")]
    public class CheckUnityObject : ConditionTask
    {
        [BlackboardOnly] public BBParameter<Object> valueA;

        public BBParameter<Object> valueB;

        protected override string info => valueA + " == " + valueB;

        protected override bool OnCheck()
        {
            return valueA.value == valueB.value;
        }
    }
}